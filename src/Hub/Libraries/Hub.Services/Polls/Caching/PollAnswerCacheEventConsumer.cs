using Hub.Core.Domain.Polls;
using Hub.Services.Caching;

namespace Hub.Services.Polls.Caching
{
   /// <summary>
   /// Represents a poll answer cache event consumer
   /// </summary>
   public partial class PollAnswerCacheEventConsumer : CacheEventConsumer<PollAnswer>
   {
   }
}