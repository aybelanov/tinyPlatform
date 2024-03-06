using Hub.Core.Domain.Users;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Users.Caching
{
   /// <summary>
   /// Represents a user user role mapping cache event consumer
   /// </summary>
   public partial class UserDeviceMappingCacheEventConsumer : CacheEventConsumer<UserDevice>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(UserDevice entity)
      {
         await RemoveByPrefixAsync(AppUserServicesDefaults.UserDevicePrefix);
      }
   }
}