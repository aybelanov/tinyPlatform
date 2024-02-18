using System.Threading.Tasks;
using Hub.Core.Domain.Common;
using Hub.Services.Caching;
using Hub.Services.Common;

namespace Hub.Services.Common.Caching
{
   /// <summary>
   /// Represents a address attribute value cache event consumer
   /// </summary>
   public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(AddressAttributeValue entity)
      {
         await RemoveAsync(HubCommonDefaults.AddressAttributeValuesByAttributeCacheKey, entity.AddressAttributeId);
      }
   }
}
