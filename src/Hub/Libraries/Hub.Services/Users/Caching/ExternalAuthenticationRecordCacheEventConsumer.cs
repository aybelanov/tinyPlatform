using Hub.Core.Domain.Users;
using Hub.Services.Caching;

namespace Hub.Services.Users.Caching
{
   /// <summary>
   /// Represents an external authentication record cache event consumer
   /// </summary>
   public partial class ExternalAuthenticationRecordCacheEventConsumer : CacheEventConsumer<ExternalAuthenticationRecord>
   {
   }
}
