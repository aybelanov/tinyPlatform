using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Configuration;
using Hub.Core.Domain.Common;
using Hub.Core.Infrastructure;
using Hub.Core.Rss;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Logging;
using Hub.Web.Areas.Admin.Infrastructure.Cache;
using Hub.Web.Areas.Admin.Models.Home;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the home models factory implementation
/// </summary>
public partial class HomeModelFactory : IHomeModelFactory
{
   #region Fields

   private readonly AdminAreaSettings _adminAreaSettings;
   private readonly ICommonModelFactory _commonModelFactory;
   private readonly ILogger _logger;
   private readonly ISettingService _settingService;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IWorkContext _workContext;
   private readonly AppHttpClient _appHttpClient;

   #endregion

   #region Ctor

   public HomeModelFactory(AdminAreaSettings adminAreaSettings,
       ICommonModelFactory commonModelFactory,
       ILogger logger,
       ISettingService settingService,
       IStaticCacheManager staticCacheManager,
       IWorkContext workContext,
       AppHttpClient appHttpClient)
   {
      _adminAreaSettings = adminAreaSettings;
      _commonModelFactory = commonModelFactory;
      _logger = logger;
      _settingService = settingService;
      _staticCacheManager = staticCacheManager;
      _workContext = workContext;
      _appHttpClient = appHttpClient;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare dashboard model
   /// </summary>
   /// <param name="model">Dashboard model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the dashboard model
   /// </returns>
   public virtual async Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      //prepare nested search models
      await _commonModelFactory.PreparePopularSearchTermSearchModelAsync(model.PopularSearchTerms);

      return model;
   }

   /// <summary>
   /// Prepare tinyPlatform news model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the tinyPlatform news model
   /// </returns>
   public virtual async Task<TinyPlatformNewsModel> PrepareTinyPlatformNewsModelAsync()
   {
      var model = new TinyPlatformNewsModel
      {
         HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
      };

      try
      {
         //try to get news RSS feed
         var rssData = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.OfficialNewsModelKey), async () =>
         {
            try
            {
               // TODO Add news
               //return await _appHttpClient.GetNewsRssAsync();
               return await Task.FromResult(new RssFeed(new(Singleton<AppSettings>.Instance.Get<HostingConfig>().HubHostUrl)));
            }
            catch (AggregateException exception)
            {
               //rethrow actual excepion
               throw exception.InnerException;
            }
         });

         for (var i = 0; i < rssData.Items.Count; i++)
         {
            var item = rssData.Items.ElementAt(i);
            var newsItem = new TinyPlatformNewsDetailsModel
            {
               Title = item.TitleText,
               Summary = XmlHelper.XmlDecode(item.Content?.Value ?? string.Empty),
               Url = item.Url.OriginalString,
               PublishDate = item.PublishDate
            };
            model.Items.Add(newsItem);

            //has new items?
            if (i != 0)
               continue;

            var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
            if (_adminAreaSettings.LastNewsTitleAdminArea == newsItem.Title)
               continue;

            _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
            await _settingService.SaveSettingAsync(_adminAreaSettings);

            //new item
            if (!firstRequest)
               model.HasNewItems = true;
         }
      }
      catch (Exception ex)
      {
         await _logger.ErrorAsync("No access to the news. Developer web site is not available.", ex);
      }

      return model;
   }

   #endregion
}