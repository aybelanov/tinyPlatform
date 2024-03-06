using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Services.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hub.Services.Common
{
   /// <summary>
   /// Represents the HTTP client to request tinyPlatform official site
   /// </summary>
   public partial class AppHttpClient
   {
      #region Fields

      private readonly AdminAreaSettings _adminAreaSettings;
      private readonly HttpClient _httpClient;
      private readonly IHttpContextAccessor _httpContextAccessor;
      private readonly ILanguageService _languageService;
      private readonly IWebHelper _webHelper;
      private readonly IWorkContext _workContext;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      public AppHttpClient(AdminAreaSettings adminAreaSettings,
            HttpClient client,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IWebHelper webHelper,
            IWorkContext workContext)
      {
         //configure client
         client.BaseAddress = new Uri("https://www.tinyplat.com/");
         client.Timeout = TimeSpan.FromSeconds(5);
         client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"service-{AppVersion.CURRENT_VERSION}");

         _adminAreaSettings = adminAreaSettings;
         _httpClient = client;
         _httpContextAccessor = httpContextAccessor;
         _languageService = languageService;
         _webHelper = webHelper;
         _workContext = workContext;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Check whether the site is available
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the asynchronous task whose result determines that request is completed
      /// </returns>
      public virtual async Task PingAsync()
      {
         await _httpClient.GetStringAsync("/");
      }

      /// <summary>
      /// Get a response regarding available categories of marketplace extensions
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the asynchronous task whose result contains the result string
      /// </returns>
      public virtual async Task<string> GetExtensionsCategoriesAsync()
      {
         //prepare URL to request
         var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
         var url = string.Format(HubCommonDefaults.AppExtensionsCategoriesPath, language).ToLowerInvariant();

         //get XML response
         return await _httpClient.GetStringAsync(url);
      }

      /// <summary>
      /// Get a response regarding available versions of marketplace extensions
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the asynchronous task whose result contains the result string
      /// </returns>
      public virtual async Task<string> GetExtensionsVersionsAsync()
      {
         //prepare URL to request
         var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
         var url = string.Format(HubCommonDefaults.AppExtensionsVersionsPath, language).ToLowerInvariant();

         //get XML response
         return await _httpClient.GetStringAsync(url);
      }

      /// <summary>
      /// Get a response regarding marketplace extensions
      /// </summary>
      /// <param name="categoryId">Category identifier</param>
      /// <param name="versionId">Version identifier</param>
      /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
      /// <param name="searchTerm">Search term</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the asynchronous task whose result contains the result string
      /// </returns>
      public virtual async Task<string> GetExtensionsAsync(long categoryId = 0,
          int versionId = 0, int price = 0, string searchTerm = null,
          int pageIndex = 0, int pageSize = int.MaxValue)
      {
         //prepare URL to request
         var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
         var url = string.Format(HubCommonDefaults.AppExtensionsPath,
             categoryId, versionId, price, WebUtility.UrlEncode(searchTerm), pageIndex, pageSize, language).ToLowerInvariant();

         //get XML response
         return await _httpClient.GetStringAsync(url);
      }

      #endregion
   }
}