using Hub.Core.Domain.Messages;
using Hub.Services.Caching;

namespace Hub.Services.Messages.Caching
{
   /// <summary>
   /// Represents an queued email cache event consumer
   /// </summary>
   public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
   {
   }
}
