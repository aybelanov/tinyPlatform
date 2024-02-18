using Hub.Core.Domain.Security;
using Hub.Services.Caching;

namespace Hub.Services.Security.Caching
{
   /// <summary>
   /// Represents a permission record-user role mapping cache event consumer
   /// </summary>
   public partial class PermissionRecordUserRoleMappingCacheEventConsumer : CacheEventConsumer<PermissionRecordUserRole>
   {
   }
}