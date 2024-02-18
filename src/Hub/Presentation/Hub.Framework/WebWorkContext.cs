using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Hub.Core.Http;
using Hub.Core.Security;
using Hub.Services.Authentication;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.ScheduleTasks;
using Hub.Services.Users;
using Hub.Web.Framework.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Shared.Clients.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Framework;

/// <summary>
/// Represents work context for web application
/// </summary>
public partial class WebWorkContext : IWorkContext
{
   #region Fields

   private readonly CookieSettings _cookieSettings;
   private readonly CurrencySettings _currencySettings;
   private readonly IAuthenticationService _authenticationService;
   private readonly ICurrencyService _currencyService;
   private readonly IUserService _userService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly ILanguageService _languageService;
   private readonly IUserAgentHelper _userAgentHelper;
   private readonly IWebHelper _webHelper;
   private readonly LocalizationSettings _localizationSettings;

   private User _cachedUser;
   private User _originalUserIfImpersonated;
   private Language _cachedLanguage;
   private Currency _cachedCurrency;
   private Device _cachedDevice;
   private string _cachedConncetionId;

   #endregion

   #region Ctor

   /// <summary> IoC Ctor </summary>
   public WebWorkContext(CookieSettings cookieSettings,
       CurrencySettings currencySettings,
       IAuthenticationService authenticationService,
       ICurrencyService currencyService,
       IUserService userService,
       IGenericAttributeService genericAttributeService,
       IHttpContextAccessor httpContextAccessor,
       ILanguageService languageService,
       IUserAgentHelper userAgentHelper,
       IWebHelper webHelper,
       LocalizationSettings localizationSettings)
   {
      _cookieSettings = cookieSettings;
      _currencySettings = currencySettings;
      _authenticationService = authenticationService;
      _currencyService = currencyService;
      _userService = userService;
      _genericAttributeService = genericAttributeService;
      _httpContextAccessor = httpContextAccessor;
      _languageService = languageService;
      _userAgentHelper = userAgentHelper;
      _webHelper = webHelper;
      _localizationSettings = localizationSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Get app user cookie
   /// </summary>
   /// <returns>String value of cookie</returns>
   protected virtual string GetGuestUserCookie()
   {
      var cookieName = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.GuestUserCookie}";
      return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
   }

   /// <summary>
   /// GetTable app user cookie
   /// </summary>
   /// <param name="userGuid">Guid of the user</param>
   protected virtual void SetUserCookie(Guid userGuid)
   {
      if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
         return;

      //delete current cookie value
      var cookieName = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.GuestUserCookie}";
      _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

      //get date of cookie expiration
      var cookieExpires = _cookieSettings.UserCookieExpires;
      var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

      //if passed guid is empty set cookie as expired
      if (userGuid == Guid.Empty)
         cookieExpiresDate = DateTime.Now.AddMonths(-1);

      //set new cookie value
      var options = new CookieOptions
      {
         HttpOnly = true,
         Expires = cookieExpiresDate,
         Secure = _webHelper.IsCurrentConnectionSecured()
      };

      _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, userGuid.ToString(), options);
   }

   /// <summary>
   /// GetTable language culture cookie
   /// </summary>
   /// <param name="language">Language</param>
   protected virtual void SetLanguageCookie(Language language)
   {
      if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
         return;

      //delete current cookie value
      var cookieName = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.CultureCookie}";
      _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

      if (string.IsNullOrEmpty(language?.LanguageCulture))
         return;

      //set new cookie value
      var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language.LanguageCulture));
      var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
      _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, value, options);
   }

   /// <summary>
   /// Get language from the request
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the found language
   /// </returns>
   protected virtual async Task<Language> GetLanguageFromRequestAsync()
   {
      string cultureName;
      if (_httpContextAccessor.HttpContext?.User?.HasClaim(x => x.Type == "client_id" && x.Value == AuthDefaults.ClientApp) ?? false)
      {
         cultureName = _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.FirstOrDefault() ?? "en-US"; 
      }
      else 
      {
         var requestCultureFeature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
         if (requestCultureFeature is null)
            return null;

         //whether we should detect the current language by user settings
         if (requestCultureFeature.Provider is not AppSeoUrlCultureProvider && !_localizationSettings.AutomaticallyDetectLanguage)
            return null;

         //get request culture
         if (requestCultureFeature.RequestCulture is null)
            return null;

         cultureName = requestCultureFeature.RequestCulture.Culture.Name;
      }

      //try to get language by culture name
      var requestLanguage = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault(language =>
          language.LanguageCulture.Equals(cultureName, StringComparison.InvariantCultureIgnoreCase));

      return requestLanguage;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets the current user
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task<User> GetCurrentUserAsync()
   {
      //whether there is a cached value
      if (_cachedUser != null)
         return _cachedUser;

      await SetCurrentUserAsync();

      return _cachedUser;
   }


   /// <summary>
   /// Sets the current user
   /// </summary>
   /// <param name="user">Current user</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SetCurrentUserAsync(User user = null)
   {
      if (user == null)
      {
         //check whether request is made by a background (schedule) task, in this case return built-in user record for background task
         if (_httpContextAccessor.HttpContext?.Request?.Path.Equals(new PathString($"/{AppTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase) ?? true)
            user = await _userService.GetOrCreateBackgroundTaskUserAsync();

         //check whether request is made by a search engine, in this case return built-in user record for search engines
         if ((user == null || user.IsDeleted || !user.IsActive || user.RequireReLogin) && _userAgentHelper.IsSearchEngine())
               user = await _userService.GetOrCreateSearchEngineUserAsync();

         //try to get registered user
         if (user == null || user.IsDeleted || !user.IsActive || user.RequireReLogin)
         {
            user = await _authenticationService.GetAuthenticatedUserAsync();

            // only admins can impersonate other users
            if (user != null && !user.IsDeleted && user.IsActive && !user.RequireReLogin && await _userService.IsAdminAsync(user))
            {
               //get impersonate user if required
               var impersonatedUserId = await _genericAttributeService.GetAttributeAsync<int?>(user, AppUserDefaults.ImpersonatedUserIdAttribute);
               if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
               {
                  var impersonatedUser = await _userService.GetUserByIdAsync(impersonatedUserId.Value);
                  if (impersonatedUser != null && !impersonatedUser.IsDeleted && impersonatedUser.IsActive && !impersonatedUser.RequireReLogin)
                  {
                     //set impersonated user
                     _originalUserIfImpersonated = user;
                     user = impersonatedUser;
                  }
               }
            }
         }

         // try to get guest user
         if (user == null || user.IsDeleted || !user.IsActive || user.RequireReLogin)
         {
            var userCookie = GetGuestUserCookie();
            if (Guid.TryParse(userCookie, out var userGuid))
            {
               // get user from cookie (should not be registered)
               var userByCookie = await _userService.GetUserByGuidAsync(userGuid);
               if (userByCookie != null && !await _userService.IsRegisteredAsync(userByCookie))
                  user = userByCookie;
            }
         }

         // create guest if not exists
         if (user == null || user.IsDeleted || !user.IsActive || user.RequireReLogin)
            user = await _userService.InsertGuestUserAsync();
      }

      if (!user.IsDeleted && user.IsActive && !user.RequireReLogin)
      {
         // set user cookie.
         SetUserCookie(user.UserGuid);

         //cache the found user
         _cachedUser = user;
      }
   }


   /// <summary>
   /// Gets the current user's signalr connection identifier
   /// that saved in the request headers
   /// </summary>
   /// <returns>SignalR connection identifier</returns>
   public virtual async Task<string> GetCurrentConncetionIdAsync()
   {
      //whether there is a cached value
      if (_cachedConncetionId != null)
         return _cachedConncetionId;

      await SetCurrentConncetionIdAsync();

      return _cachedConncetionId;
   }


   /// <summary>
   /// Sets the current user's signalr connection identifier
   /// that saved in the request headers
   /// </summary>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   private Task SetCurrentConncetionIdAsync()
   {
      if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(SignalRDefaults.SignalrConnectionIdHeader, out var conncetionId) ?? false)
         _cachedConncetionId = conncetionId;

      return Task.CompletedTask;
   }


   /// <summary>
   /// Sets current user working language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SetWorkingLanguageAsync(Language language)
   {
      //save passed language identifier
      var user = await GetCurrentUserAsync();
      await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.LanguageIdAttribute, language?.Id ?? 0);

      //set cookie
      SetLanguageCookie(language);

      //then reset the cached value
      _cachedLanguage = null;
   }


   /// <summary>
   /// Gets current user working language
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task<Language> GetWorkingLanguageAsync()
   {
      //whether there is a cached value
      if (_cachedLanguage != null)
         return _cachedLanguage;

      var user = await GetCurrentUserAsync();

      //whether we should detect the language from the request
      var detectedLanguage = await GetLanguageFromRequestAsync();

      //get current saved language identifier
      var currentLanguageId = await _genericAttributeService
          .GetAttributeAsync<int>(user, AppUserDefaults.LanguageIdAttribute);

      //if the language is detected we need to save it
      if (detectedLanguage != null)
      {   //save the detected language identifier if it differs from the current one
         if (detectedLanguage.Id != currentLanguageId)
            await SetWorkingLanguageAsync(detectedLanguage);
      }
      else
      {
         var allPlatformLanguages = await _languageService.GetAllLanguagesAsync();

         //check user language availability
         detectedLanguage = allPlatformLanguages.FirstOrDefault(language => language.Id == currentLanguageId);

         //it not found, then try to get the default language
         detectedLanguage ??= allPlatformLanguages.FirstOrDefault();

         //if the default language for the current platform not found, then try to get the first one
         detectedLanguage ??= allPlatformLanguages.FirstOrDefault();

         //if there are no languages for the current platform try to get the first one regardless of the platform
         detectedLanguage ??= (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();

         SetLanguageCookie(detectedLanguage);
      }

      //cache the found language
      _cachedLanguage = detectedLanguage;

      return _cachedLanguage;
   }


   /// <summary>
   /// Gets current user working currency
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task<Currency> GetWorkingCurrencyAsync()
   {
      //whether there is a cached value
      if (_cachedCurrency != null)
         return _cachedCurrency;

      var adminAreaUrl = $"{_webHelper.GetAppLocation()}admin";

      //return primary platform currency when we're in admin area/mode
      if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
      {
         var primaryPlatformCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryPlatformCurrencyId);
         if (primaryPlatformCurrency != null)
         {
            _cachedCurrency = primaryPlatformCurrency;
            return primaryPlatformCurrency;
         }
      }

      var user = await GetCurrentUserAsync();

      //find a currency previously selected by a user
      var userCurrencyId = await _genericAttributeService
          .GetAttributeAsync<int>(user, AppUserDefaults.CurrencyIdAttribute);

      var allPlatformCurrencies = await _currencyService.GetAllCurrenciesAsync();

      //check user currency availability
      var userCurrency = allPlatformCurrencies.FirstOrDefault(currency => currency.Id == userCurrencyId);
      if (userCurrency == null)
      {
         //it not found, then try to get the default currency for the current language (if specified)
         var language = await GetWorkingLanguageAsync();
         userCurrency = allPlatformCurrencies
             .FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
      }

      //if the default currency for the current platform not found, then try to get the first one
      if (userCurrency == null)
         userCurrency = allPlatformCurrencies.FirstOrDefault();

      //if there are no currencies for the current platform try to get the first one regardless of the platform
      if (userCurrency == null)
         userCurrency = (await _currencyService.GetAllCurrenciesAsync()).FirstOrDefault();

      //cache the found currency
      _cachedCurrency = userCurrency;

      return _cachedCurrency;
   }

   /// <summary>
   /// Sets current user working currency
   /// </summary>
   /// <param name="currency">Currency</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SetWorkingCurrencyAsync(Currency currency)
   {
      //save passed currency identifier
      var user = await GetCurrentUserAsync();
      await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CurrencyIdAttribute, currency?.Id ?? 0);

      //then reset the cached value
      _cachedCurrency = null;
   }

   /// <summary>
   /// Gets the current device
   /// </summary>
   /// <returns>Device entity</returns>
   public async Task<Device> GetCurrentDeviceAsync()
   {
      //whether there is a cached value
      if (_cachedDevice != null)
         return _cachedDevice;

      await SetCurrentDeviceAsync();

      return _cachedDevice;
   }

   /// <summary>
   /// Sets the current device to the current context
   /// </summary>
   /// <returns></returns>
   public async Task SetCurrentDeviceAsync(Device device = null)
   {
      if (device == null)
      {
         device = await _authenticationService.GetAuthenticatedDeviceAsync();
         
         if (device == null || device.IsDeleted || !device.IsActive)
            return;
      }

      _cachedDevice = device; 
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets or sets value indicating whether we're in admin area
   /// </summary>
   public virtual bool IsAdmin { get; set; }

   /// <summary>
   /// Gets the original user (in case the current one is impersonated)
   /// </summary>
   public virtual User OriginalUserIfImpersonated => _originalUserIfImpersonated;

   #endregion
}