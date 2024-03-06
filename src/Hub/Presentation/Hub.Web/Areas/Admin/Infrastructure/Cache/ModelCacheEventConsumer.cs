using Hub.Core.Caching;
using Hub.Core.Domain.Configuration;
using Hub.Core.Events;
using Hub.Services.Events;
using Hub.Services.Plugins;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Infrastructure.Cache
{
   /// <summary>
   /// Model cache event consumer (used for caching of presentation layer models)
   /// </summary>
   public partial class ModelCacheEventConsumer :
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,

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

      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
      {
         //clear models which depend on settings
         await _staticCacheManager.RemoveAsync(AppModelCacheDefaults.OfficialNewsModelKey); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
      }

      /// <summary>
      /// Handle plugin updated event
      /// </summary>
      /// <param name="eventMessage">Event</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
      {
         await _staticCacheManager.RemoveByPrefixAsync(AppPluginDefaults.AdminNavigationPluginsPrefix);
      }

      #endregion
   }
}