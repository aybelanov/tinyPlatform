using Hub.Core.Caching;
using Hub.Core.Domain.Configuration;
using Hub.Core.Events;
using Hub.Services.Cms;
using Hub.Services.Events;
using Hub.Services.Plugins;
using System.Threading.Tasks;

namespace Hub.Web.Infrastructure.Cache
{
   /// <summary>
   /// Model cache event consumer (used for caching of presentation layer models)
   /// </summary>
   public partial class ModelCacheEventConsumer :
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //plugins
        IConsumer<PluginUpdatedEvent>
   {
      #region Fields

      private readonly IStaticCacheManager _staticCacheManager;

      #endregion

      #region Ctor

      public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
      {
         _staticCacheManager = staticCacheManager;
      }

      #endregion

      #region Methods

      #region Setting

      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
      {
         //clear models which depend on settings
         await _staticCacheManager.RemoveByPrefixAsync(AppModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
         await _staticCacheManager.RemoveByPrefixAsync(AppModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
         await _staticCacheManager.RemoveByPrefixAsync(AppModelCacheDefaults.AppLogoPathPrefixCacheKey); //depends on AppInfoSettings.LogoPictureId
      }

      #endregion

      #region Plugin

      /// <summary>
      /// Handle plugin updated event
      /// </summary>
      /// <param name="eventMessage">Event message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
      {
         if (eventMessage?.Plugin?.Instance<IWidgetPlugin>() != null)
            await _staticCacheManager.RemoveByPrefixAsync(AppModelCacheDefaults.WidgetPrefixCacheKey);
      }

      #endregion

      #endregion
   }
}