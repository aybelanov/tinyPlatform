using System.Threading.Tasks;
using Hub.Core.Domain.Common;
using Hub.Services.Caching;
using Hub.Services.Common;

namespace Hub.Services.Common.Caching
{
   /// <summary>
   /// Represents a generic attribute cache event consumer
   /// </summary>
   public partial class GenericAttributeCacheEventConsumer : CacheEventConsumer<GenericAttribute>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(GenericAttribute entity)
      {
         await RemoveAsync(HubCommonDefaults.GenericAttributeCacheKey, entity.EntityId, entity.KeyGroup);
      }
   }
}
