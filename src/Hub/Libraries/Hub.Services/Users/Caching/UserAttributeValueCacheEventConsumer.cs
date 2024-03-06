using Hub.Core.Domain.Users;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Users.Caching
{
   /// <summary>
   /// Represents a user attribute value cache event consumer
   /// </summary>
   public partial class UserAttributeValueCacheEventConsumer : CacheEventConsumer<UserAttributeValue>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(UserAttributeValue entity)
      {
         await RemoveAsync(AppUserServicesDefaults.UserAttributeValuesByAttributeCacheKey, entity.UserAttributeId);
      }
   }
}