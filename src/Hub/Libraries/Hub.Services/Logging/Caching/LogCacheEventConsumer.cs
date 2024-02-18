using Hub.Core.Domain.Logging;
using Hub.Services.Caching;

namespace Hub.Services.Logging.Caching
{
   /// <summary>
   /// Represents a log cache event consumer
   /// </summary>
   public partial class LogCacheEventConsumer : CacheEventConsumer<Log>
   {
   }
}
