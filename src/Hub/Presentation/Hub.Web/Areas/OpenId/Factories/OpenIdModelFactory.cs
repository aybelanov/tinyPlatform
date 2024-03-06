using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Core.Http;
using Hub.Services.Authentication;
using Hub.Services.Devices;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Areas.OpenId.Domain;
using Hub.Web.Areas.OpenId.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Clients.Configuration;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.OpenId.Factories;

/// <summary>
/// Represents an openid model factory interface implementation
/// </summary>
public class OpenIdModelFactory : IOpenIdModelFactory
{
   #region field

   private readonly AppSettings _appSettings;
   private readonly IWorkContext _workContext;
   private readonly IUserService _userService;
   private readonly IHubDeviceService _deviceService;
   private readonly UserSettings _userSettiings;
   private readonly DeviceSettings _deviceSettings;
   private readonly IPermissionService _permissionService;
   private readonly IDeviceRegistrationService _deviceRegistrationService;
   private readonly IWebHelper _webHelper;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public OpenIdModelFactory(AppSettings appSettings,
      IWorkContext workContext,
      IUserService userService,
      IHubDeviceService deviceService,
      IWebHelper webHelper,
      UserSettings userSettiings,
      DeviceSettings deviceSettings,
      IPermissionService permissionService,
      IDeviceRegistrationService deviceRegistrationService)
   {
      _appSettings = appSettings;
      _workContext = workContext;
      _userService = userService;
      _deviceService = deviceService;
      _userSettiings = userSettiings;
      _permissionService = permissionService;
      _deviceRegistrationService = deviceRegistrationService;
      _webHelper = webHelper;
      _deviceSettings = deviceSettings;
   }


   #endregion

   #region Methods

   /// <summary>
   /// Prepare an access token and id token
   /// </summary>
   /// <param name="subjectId">Subject identifier</param>
   /// <returns></returns>
   public async Task<ClientTokenInfoModel> PrepareClientTokensAsync(AuthCode code)
   {
      var user = await _userService.GetUserByGuidAsync(Guid.Parse(code.SubjectId));
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessToClient, user))
         throw new AppException("permission denied");

      var roles = await _userService.GetUserRolesAsync(user);
      var permissions = await _permissionService.GetUserPermissionsAsync(user);

      var accesTokenScope = permissions
         .Where(x => x.Category != "Client" && x.Category != "Device")
         .Select(x => x.SystemName)
         .ToList();

      var commonScope = new[] { "openid", "profile", "offline_access", "Hub.WebAPI", "Hub.SignalR" };
      accesTokenScope.AddRange(commonScope);

      var issuer = _appSettings.Get<HostingConfig>().HubHostUrl.Trim('/');
      var now = DateTime.UtcNow;

      var accessTokenDescriptor = new SecurityTokenDescriptor()
      {
         Issuer = issuer,
         Audience = AuthDefaults.ClientApp,
         NotBefore = now,
         IssuedAt = now,
         Expires = now.AddSeconds(AuthDefaults.ClientJWTlifetime),
         TokenType = "at+jwt",
         SigningCredentials = new SigningCredentials(
            EncryptionHelper.RsaSecurityKey,
            SecurityAlgorithms.RsaSha256),

         //https://datatracker.ietf.org/doc/html/rfc7518#section-4.1
         EncryptingCredentials = new EncryptingCredentials(
            EncryptionHelper.SymmetricSecurityKey,
            SecurityAlgorithms.Aes256KeyWrap,
            SecurityAlgorithms.Aes256CbcHmacSha512),

         Claims = new Dictionary<string, object>()
         {
            //JwtRegisteredClaimNames.Jti
            { "jti", Convert.ToHexString(Guid.NewGuid().ToByteArray())},
            { "client_id", AuthDefaults.ClientApp },
            { "sub", user.UserGuid.ToString() },
            { "email", user.Email },
            { "name", _userSettiings.UsernamesEnabled ? user.Username : user.Email },
            { "role", roles.Select(x=>x.SystemName).ToList() },
            { "scope", accesTokenScope },
         }
      };

      var securityHandler = new JwtSecurityTokenHandler();
      var securityToken = securityHandler.CreateJwtSecurityToken(accessTokenDescriptor);
      var accessToken = securityHandler.WriteToken(securityToken);

      var idTokenScope = permissions.Where(x => x.Category == "Client").Select(x => x.SystemName).ToList();
      idTokenScope.AddRange(commonScope);

      // https://openid.net/specs/openid-connect-core-1_0.html#toc
      var idTokenDescription = new SecurityTokenDescriptor()
      {
         Issuer = issuer,
         Audience = AuthDefaults.ClientApp,
         NotBefore = now,
         IssuedAt = now,
         Expires = now.AddSeconds(AuthDefaults.ClientJWTlifetime),
         SigningCredentials = new SigningCredentials(EncryptionHelper.RsaSecurityKey, SecurityAlgorithms.RsaSha256),
         Claims = new Dictionary<string, object>()
         {
            { "jti", Convert.ToHexString(Guid.NewGuid().ToByteArray())},
            { "client_id", AuthDefaults.ClientApp },
            { "sub", user.UserGuid.ToString() },
            { "auth_time", now.ToUnixEpochTime() },
         }
      };

      var handler = new JsonWebTokenHandler();
      var idToken = handler.CreateToken(idTokenDescription);
      //var accessToken = handler.CreateToken(accessTokenDescriptor);

      var clientTokens = new ClientTokenInfoModel()
      {
         AccessToken = accessToken,
         CreatedOn = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
         ExpiresIn = AuthDefaults.ClientJWTlifetime,
         IdToken = idToken,
         TokenType = JwtBearerDefaults.AuthenticationScheme,
         Scope = new[] { "openid", "profile", "offline_access" }
      };

      return clientTokens;
   }


   /// <summary>
   /// Prepares device token
   /// </summary>
   /// <param name="systemName">Device system name</param>
   /// <param name="secret">Device secret (password)</param>
   /// <returns></returns>
   public async Task<DeviceTokenInfoModel> PrepareDeviceTokenAsync(string systemName, string secret)
   {
      var loginResult = await _deviceRegistrationService.ValidateDeviceCredentialsAsync(systemName, secret);

      if (loginResult == DeviceLoginResults.Successful)
      {
         var device = await _deviceService.GetDeviceBySystemNameAsync(systemName);
         var permissions = await _permissionService.GetAllPermissionRecordsAsync();

         var accessTokenScope = permissions
         .Where(x => x.Category == "Device")
         .Select(x => x.SystemName)
         .ToList();

         accessTokenScope.Add("Hub.Grpc");

         var issuer = _appSettings.Get<HostingConfig>().HubHostUrl.Trim('/');
         var now = DateTime.UtcNow;

         var accessTokenDescriptor = new SecurityTokenDescriptor()
         {
            Issuer = issuer,
            Audience = AuthDefaults.DispatcherApp,
            NotBefore = now,
            IssuedAt = now,
            Expires = now.AddSeconds(AuthDefaults.ClientJWTlifetime),
            TokenType = "at+jwt",
            SigningCredentials = new SigningCredentials(
               EncryptionHelper.RsaSecurityKey,
               SecurityAlgorithms.RsaSha256),

            //https://datatracker.ietf.org/doc/html/rfc7518#section-4.1
            EncryptingCredentials = new EncryptingCredentials(
               EncryptionHelper.SymmetricSecurityKey,
               SecurityAlgorithms.Aes256KeyWrap,
               SecurityAlgorithms.Aes256CbcHmacSha512),

            Claims = new Dictionary<string, object>()
            {
               { "jti", Convert.ToHexString(Guid.NewGuid().ToByteArray())},
               { "client_id", AuthDefaults.DispatcherApp },
               { "sub", device.Guid.ToString() },
               { "name", device.SystemName },
               { "role", UserDefaults.DevicesRoleName },
               { "scope", accessTokenScope },
            }
         };

         var securityHandler = new JwtSecurityTokenHandler();
         var securityToken = securityHandler.CreateJwtSecurityToken(accessTokenDescriptor);
         var accessToken = securityHandler.WriteToken(securityToken);

         var tokenInfoModel = new DeviceTokenInfoModel()
         {
            AccessToken = accessToken,
            CreatedOn = now.ToUnixEpochTime(),
            ExpiresIn = AuthDefaults.DeviceJWTlifetime,
            TokenType = JwtBearerDefaults.AuthenticationScheme,
         };

         return tokenInfoModel;
      }
      else switch (loginResult)
         {
            case DeviceLoginResults.DeviceDeleted: throw new AppException("Device is deleted.");
            case DeviceLoginResults.DeviceNotExist: throw new AppException("Device does not exist.");
            case DeviceLoginResults.DeviceNotActive: throw new AppException("Device is not active.");
            case DeviceLoginResults.DeviceLockedOut: throw new AppException("Device is locked out.");
            case DeviceLoginResults.WrongPassword: throw new AppException("Wrong credentials.");
            case DeviceLoginResults.UserDeleted: throw new AppException("Devie owner is deleted.");
            case DeviceLoginResults.UserNotExist: throw new AppException("Device owner is not exist.");
            case DeviceLoginResults.UserNotActive: throw new AppException("Device owner is not active");
            case DeviceLoginResults.UserNotRegistered: throw new AppException("Device owner is not registered.");
            case DeviceLoginResults.UserLockedOut: throw new AppException("Device owner is locked out");
            default: throw new AppException("Unknown device login error");
         }
   }


   /// <summary>
   /// Prepares a userinfo model
   /// </summary>
   /// <returns>User info model</returns>
   public async Task<UserInfoModel> PrepareUserInfoModelAsync()
   {
      var user = await _workContext.GetCurrentUserAsync();
      var roles = await _userService.GetUserRolesAsync(user);
      var permissions = await _permissionService.GetUserPermissionsAsync(user, "Client");
      var scope = new List<string>() { "openid", "profile", "offline_access" };
      scope.AddRange(permissions.Select(x => x.SystemName));

      var model = new UserInfoModel()
      {
         UserName = _userSettiings.UsernamesEnabled ? user.Username : user.Email,
         Roles = roles.Select(x => x.Name).ToList(),
         SubjectId = user.UserGuid.ToString(),
         Scope = scope
      };

      return model;
   }


   /// <summary>
   /// Prepares a checksession model
   /// </summary>
   /// <returns></returns>
   public Task<CheckSessionModel> PrepareCheckSessionAsync()
   {
      var model = new CheckSessionModel();
      model.CookieName = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.CheckSessionCookie}";

      return Task.FromResult(model);
   }


   /// <summary>
   /// Prepare an openid cinfiguration
   /// </summary>
   /// <returns></returns>
   public Task<OpenIdConfigModel> PrepareOpenIdConfigAsync()
   {
      var applicationUrl = _appSettings.Get<HostingConfig>().HubHostUrl.Trim('/');

      var model = new OpenIdConfigModel();

      // required
      model.Issuer = applicationUrl;
      model.JwksUri = $"{applicationUrl}/.well-known/openid-configuration/jwks";
      model.AuthorizationEndPoint = $"{applicationUrl}/connect/authorize";
      model.TokenEndPoint = $"{applicationUrl}/connect/token";
      model.CheckSessionIframe = $"{applicationUrl}/connect/checksession";
      model.EndSessionEndPoint = $"{applicationUrl}/connect/endsession";
      model.RevocationEndPoint = $"{applicationUrl}/connect/revocation";
      model.IntrospectionEndPoint = $"{applicationUrl}/connect/introspect";
      model.IdTokenSigningAlgValuesSupported = new[] { "RS256" };
      model.RequestParameterSupported = true;
      model.SubjectTypesSupported = new[] { "public" };
      model.ResponseTypesSupported = new[]
      {
         "code",
         "token",
         "id_token",
         "id_token token",
         "code id_token",
         "code token",
         "code id_token token"
      };

      // recommended
      model.UserInfoEndPoint = $"{applicationUrl}/connect/userinfo";
      model.RegistrationEndpoint = $"{applicationUrl}/register";
      model.ClaimsSupported = new[]
      {
         "sub",
         "name",
         "role"
      };

      model.ScopesSupported = new[]
      {
         "openid",
         "profile",
         "offline_access"
      };

      //optional
      model.GrantTypesSupported = new[]
      {
         "authorization_code",
         "client_credentials",
         "refresh_token",
         "implicit"
      };

      model.ResponseModesSupported = new[]
      {
         "form_post",
         "query",
         "fragment"
      };

      model.TokenEndpointAuthMethodsSupported = new[]
      {
         "client_secret_basic",
         "client_secret_post"
      };

      return Task.FromResult(model);
   }


   /// <summary>
   /// Prepare a JSON Web Key Set
   /// </summary>
   /// <returns></returns>
   public Task<JwksModel> PrepareJwksAsync()
   {
      var rsaParameters = EncryptionHelper.RsaSecurityKey.Rsa.ExportParameters(false);
      var modulus = Convert.ToBase64String(rsaParameters.Modulus);
      var exponent = Convert.ToBase64String(rsaParameters.Exponent);

      // https://datatracker.ietf.org/doc/html/rfc7517#page-8
      // we use only one rsa key but we set key id for future
      var keyid = Convert.ToHexString(Guid.NewGuid().ToByteArray());

      var model = new JwksModel();
      model.Keys.Add(new()
      {
         Algorithm = "RS256",
         KeyId = keyid,
         KeyType = "RSA",
         KeyUse = "sig",
         PublicExponent = exponent,
         Modulus = modulus
      });

      return Task.FromResult(model);
   }

   #endregion
}
