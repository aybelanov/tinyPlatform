using System.Threading.Tasks;
using Hub.Core.Caching;
using Hub.Core.Domain.Directory;
using Hub.Services.Caching;

namespace Hub.Services.Directory.Caching
{
   /// <summary>
   /// Represents a state province cache event consumer
   /// </summary>
   public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
   {
      /// <summary>
      /// Clear cache by entity event type
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <param name="entityEventType">Entity event type</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(StateProvince entity, EntityEventType entityEventType)
      {
         await RemoveByPrefixAsync(AppEntityCacheDefaults<StateProvince>.Prefix);
      }
   }
}
