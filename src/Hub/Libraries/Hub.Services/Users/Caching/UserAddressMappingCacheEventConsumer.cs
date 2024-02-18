using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Services.Caching;
using Hub.Services.Users;

namespace Hub.Services.Users.Caching
{
   /// <summary>
   /// Represents a user address mapping cache event consumer
   /// </summary>
   public partial class UserAddressMappingCacheEventConsumer : CacheEventConsumer<UserAddress>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(UserAddress entity)
      {
         await RemoveByPrefixAsync(AppUserServicesDefaults.UserAddressesPrefix);
      }
   }
}