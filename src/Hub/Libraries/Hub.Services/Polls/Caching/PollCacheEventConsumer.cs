using Hub.Core.Domain.Polls;
using Hub.Services.Caching;

namespace Hub.Services.Polls.Caching
{
   /// <summary>
   /// Represents a poll cache event consumer
   /// </summary>
   public partial class PollCacheEventConsumer : CacheEventConsumer<Poll>
   {
   }
}