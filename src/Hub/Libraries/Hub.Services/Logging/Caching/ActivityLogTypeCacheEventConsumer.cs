using Hub.Core.Domain.Logging;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Logging.Caching
{
   /// <summary>
   /// Represents a activity log type cache event consumer
   /// </summary>
   public partial class ActivityLogTypeCacheEventConsumer : CacheEventConsumer<ActivityLogType>
   {
   }
}
