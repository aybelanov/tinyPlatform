using Hub.Core.Domain.Users;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Users.Caching
{
   /// <summary>
   /// Represents a user role cache event consumer
   /// </summary>
   public partial class UserRoleCacheEventConsumer : CacheEventConsumer<UserRole>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(UserRole entity)
      {
         await RemoveByPrefixAsync(AppUserServicesDefaults.UserRolesBySystemNamePrefix);
         await RemoveByPrefixAsync(AppUserServicesDefaults.UserUserRolesPrefix);
      }
   }
}
