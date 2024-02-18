using Hub.Core.Domain.Media;
using Hub.Services.Caching;

namespace Hub.Services.Media.Caching
{
   /// <summary>
   /// Represents a download cache event consumer
   /// </summary>
   public partial class DownloadCacheEventConsumer : CacheEventConsumer<Download>
   {
   }
}
