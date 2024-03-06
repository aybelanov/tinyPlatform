using Hub.Core.Domain.Common;
using Hub.Services.Caching;
using Hub.Services.Users;
using System.Threading.Tasks;

namespace Hub.Services.Common.Caching
{
   /// <summary>
   /// Represents a address cache event consumer
   /// </summary>
   public partial class AddressCacheEventConsumer : CacheEventConsumer<Address>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(Address entity)
      {
         await RemoveByPrefixAsync(AppUserServicesDefaults.UserAddressesPrefix);
      }
   }
}
