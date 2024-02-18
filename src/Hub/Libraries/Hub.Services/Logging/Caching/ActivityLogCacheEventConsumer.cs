using Hub.Core.Domain.Logging;
using Hub.Services.Caching;

namespace Hub.Services.Logging.Caching
{
   /// <summary>
   /// Represents an activity log cache event consumer
   /// </summary>
   public partial class ActivityLogCacheEventConsumer : CacheEventConsumer<ActivityLog>
   {
   }
}