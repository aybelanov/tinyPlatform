using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Http;
using Hub.Services.Authentication;
using Hub.Services.Logging;
using Hub.Services.Security;
using Hub.Web.Areas.OpenId.Domain;
using Hub.Web.Areas.OpenId.Factories;
using Hub.Web.Framework;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Clients.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Hub.Web.Areas.OpenId.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
[Area(AreaNames.OpenId)]
[Route("~/connect/[action]")]
[EnableCors(WebFrameworkDefaults.CorsPolicyName)]
public class OpenIdController : Controller
{
   #region fields

   private readonly IDataProtectionProvider _protectionProvider;
   private readonly IAuthenticationService _authenticationService;
   private readonly AppSettings _appSettings;
   private readonly IOpenIdModelFactory _oidFactory;
   private readonly IWorkContext _workContext;
   private readonly IPermissionService _permissionService;
   private readonly IUserActivityService _userActivityService;
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly IWebHelper _webHelper;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public OpenIdController(IDataProtectionProvider protectionProvider,
      IAuthenticationService authenticationService,
      AppSettings appSettings,
      IOpenIdModelFactory oidFactory,
      IWorkContext workContext,
      IPermissionService permissionService,
      IUserActivityService userActivityService,
      IWebHelper webHelper,
      IHttpContextAccessor httpContextAccessor)
   {
      _protectionProvider = protectionProvider;
      _authenticationService = authenticationService;
      _appSettings = appSettings;
      _oidFactory = oidFactory;
      _workContext = workContext;
      _permissionService = permissionService;
      _userActivityService = userActivityService;
      _httpContextAccessor = httpContextAccessor;
      _webHelper = webHelper;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// GetTable app user cookie
   /// </summary>
   /// <param name="hashString">Hash string</param>
   protected virtual void SetCheckSessionCookie(string hashString)
   {
      if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
         return;

      //delete current cookie value
      var cookieName = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.CheckSessionCookie}";
      _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

      //get date of cookie expiration
      var cookieExpires = AuthDefaults.OAuthCodeLifetime + AuthDefaults.ClientJWTlifetime;
      var cookieExpiresDate = DateTime.Now.AddSeconds(cookieExpires);

      //set new cookie value
      var options = new CookieOptions
      {
         HttpOnly = true,
         Expires = cookieExpiresDate,
         Secure = _webHelper.IsCurrentConnectionSecured()
      };

      _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, hashString, options);
   }


   /// <summary>
   /// Validate redicrect url by the pointed authority
   /// </summary>
   /// <param name="uri">Redirect uri</param>
   /// <returns>result</returns>
   private bool ValidateRedirectUri(Uri uri)
   {
      try
      {
         if (uri.IsAbsoluteUri)
         {
            var allowedUrls = _appSettings.Get<SecurityConfig>().CorsOrigins.Select(x => x.Trim('/')).ToList();
            allowedUrls.Add(_appSettings.Get<HostingConfig>().HubHostUrl.Trim('/'));
            return allowedUrls.Contains($"{uri.Scheme}://{uri.Authority}");
         }
         else
            return true;
      }
      catch
      {
         return false;
      }
   }

   /// <summary>
   /// Computes hash for session management
   /// </summary>
   /// <param name="code"></param>
   /// <returns></returns>
   private byte[] ComputeSessionHash(AuthCode code)
   {
      using (var hash256 = SHA256.Create())
      {
         var @string = JsonSerializer.Serialize(new { code.SubjectId, code.Expired, code.ClientId, code.CodeChallenge });
         var hash = hash256.ComputeHash(Encoding.UTF8.GetBytes(@string));

         return hash;
      }
   }

   /// <summary>
   /// Validate core verifier
   /// </summary>
   /// <param name="code">code</param>
   /// <param name="codeVerifier"></param>
   /// <returns>result</returns>
   private bool ValidateCodeVerifier(AuthCode code, string codeVerifier)
   {
      using var sha256 = SHA256.Create();
      var codeChallenge = Base64UrlEncoder.Encode(sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier)));
      return code.CodeChallenge == codeChallenge && code.Expired >= DateTime.UtcNow;
   }

   #endregion

   #region Methods

   [Produces("application/json")]
   [Route("~/.well-known/openid-configuration")]
   public async Task<IActionResult> Metadata()
   {
      var model = await _oidFactory.PrepareOpenIdConfigAsync();
      return Ok(model);
   }


   [Produces("application/json")]
   [Route("~/.well-known/openid-configuration/jwks")]
   public async Task<IActionResult> Jwks()
   {
      var reply = await _oidFactory.PrepareJwksAsync();
      return Ok(reply);
   }


   public async Task<IActionResult> Authorize()
   {
      if (Request.Query.TryGetValue("redirect_uri", out var redirectUri) && redirectUri.Count == 1
         && Uri.TryCreate(redirectUri, UriKind.RelativeOrAbsolute, out var uri)
         && ValidateRedirectUri(uri)
         && Request.Query.TryGetValue("state", out var state) && state.Count == 1)
      {
         if (!(HttpContext.User?.Identity?.IsAuthenticated ?? false))
         {
            if (Request.Query.TryGetValue("prompt", out var prompt) && prompt.Count == 1 && prompt == "none")
               return Redirect($"{redirectUri}?error=login_required&state={state}");
            else
               return Challenge();
         }

         var user = await _workContext.GetCurrentUserAsync();

         if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessToClient, user))
         {
            try
            {
               if (Request.Query.TryGetValue("response_type", out var responseType) && responseType.Count == 1
                  && Request.Query.TryGetValue("client_id", out var clientId) && clientId.Count == 1
                  && Request.Query.TryGetValue("code_challenge", out var codeChallenge) && codeChallenge.Count == 1
                  && Request.Query.TryGetValue("code_challenge_method", out var codeChallengeMethod) && codeChallengeMethod.Count == 1
                  && Request.Query.TryGetValue("scope", out var scope) && scope.Count == 1)
               {
                  var code = new AuthCode()
                  {
                     ClientId = clientId,
                     CodeChallenge = codeChallenge,
                     SubjectId = user.UserGuid.ToString(),
                     CodeChallengeMethod = codeChallengeMethod,
                     RedirectUri = redirectUri,
                     Expired = DateTime.UtcNow.AddSeconds(AuthDefaults.OAuthCodeLifetime)
                  };

                  var protector = _protectionProvider.CreateProtector("openid");
                  var codeString = protector.Protect(JsonSerializer.Serialize(code));

                  //issuer
                  var iss = _appSettings.Get<HostingConfig>().HubHostUrl.TrimEnd('/');
                  var hash = ComputeSessionHash(code);

                  var sessionState = Convert.ToBase64String(hash);
                  SetCheckSessionCookie(sessionState);

                  return Redirect($"{redirectUri}?code={codeString}&state={state}&session_state={sessionState}&iss={HttpUtility.UrlEncode(iss)}");
               }
            }
            catch { }
         }
      }

      //If we got this far, something had gone wrong
      return BadRequest(new { error = "authorize_request_is_invalid" });
   }


   [HttpPost]
   public async Task<IActionResult> Token()
   {
      try
      {
         // error https://github.com/dotnet/aspnetcore/issues/33409
         //var bodyBytes = await Request.Bo.BodyReader.ReadAsync();
         //var bodyContent = Encoding.UTF8.GetString(bodyBytes.Buffer);

         using var reader = new StreamReader(Request.Body, Encoding.UTF8);
         var bodyContent = await reader.ReadToEndAsync();

         var query = bodyContent.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => HttpUtility.UrlDecode(x[1]));

         if (query.TryGetValue("client_id", out var clientId) && query.TryGetValue("grant_type", out var grantType))
         {
            // client authorization
            if (clientId == AuthDefaults.ClientApp
               && grantType == "authorization_code"
               && query.TryGetValue("code", out var code)
               && query.TryGetValue("redirect_uri", out var redirectUri)
               && query.TryGetValue("code_verifier", out var codeVerifier))
            {
               var protector = _protectionProvider.CreateProtector("openid");
               var codeString = protector.Unprotect(code);
               var authCode = JsonSerializer.Deserialize<AuthCode>(codeString);

               if (ValidateCodeVerifier(authCode, codeVerifier))
               {
                  var tokenInfo = await _oidFactory.PrepareClientTokensAsync(authCode);
                  return Ok(tokenInfo);
               }
            }
            // device authorization
            else if (clientId == AuthDefaults.DispatcherApp
               && grantType == "client_credentials"
               && query.TryGetValue("client_secret", out var clientSecret)
               && query.TryGetValue("device_id", out var deviceId))
            {
               var tokenInfo = await _oidFactory.PrepareDeviceTokenAsync(deviceId, clientSecret);
               return Ok(tokenInfo);
            }
         }
      }
      catch (Exception ex)
      {
         return BadRequest(new { error = ex.Message });
      }

      //If we got this far, something had gone wrong
      return BadRequest(new { error = "token_request_is_invalid" });
   }

   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
   [RequiredScope("profile")]
   public async Task<IActionResult> UserInfo()
   {
      var model = await _oidFactory.PrepareUserInfoModelAsync();
      return Ok(model);
   }


   [Authorize(AuthenticationSchemes = AuthDefaults.CookieAuthenticationScheme)]
   public async Task<IActionResult> EndSession()
   {
      await _authenticationService.SignOutAsync();

      if (Request.Query.TryGetValue("id_token_hint", out var tokenString))
      {
         var tokenHadler = new JsonWebTokenHandler();
         var token = tokenHadler.ReadJsonWebToken(tokenString);
         var client = token.Claims.First(x => x.Type == "client_id");
         await _userActivityService.InsertActivityAsync("Client.Logout", $"Client {client} has logged out");
      }

      if (Request.Query.TryGetValue("post_logout_redirect_uri", out var postLogoutRedirectUri)
         && Uri.TryCreate(postLogoutRedirectUri, UriKind.RelativeOrAbsolute, out var uri)
         && ValidateRedirectUri(uri)
         && Request.Query.TryGetValue("state", out var state))
      {
         HttpContext.Response.Cookies.Delete($"{AppCookieDefaults.Prefix}{AppCookieDefaults.CheckSessionCookie}");
         return Redirect($"{postLogoutRedirectUri}?state={state}");
      }

      return RedirectToRoute("HomePage");
   }


   [Authorize]
   public async Task<IActionResult> CheckSession()
   {
      var model = await _oidFactory.PrepareCheckSessionAsync();
      return View(model);
   }

   #endregion
}