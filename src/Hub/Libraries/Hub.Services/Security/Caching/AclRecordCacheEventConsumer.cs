using System.Threading.Tasks;
using Hub.Core.Domain.Security;
using Hub.Services.Caching;
using Hub.Services.Security;

namespace Hub.Services.Security.Caching
{
   /// <summary>
   /// Represents a ACL record cache event consumer
   /// </summary>
   public partial class AclRecordCacheEventConsumer : CacheEventConsumer<AclRecord>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(AclRecord entity)
      {
         await RemoveAsync(AppSecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
         await RemoveAsync(AppSecurityDefaults.EntityAclRecordExistsCacheKey, entity.EntityName);
      }
   }
}
