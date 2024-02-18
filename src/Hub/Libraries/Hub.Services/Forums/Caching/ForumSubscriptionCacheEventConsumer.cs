using Hub.Core.Domain.Forums;
using Hub.Services.Caching;

namespace Hub.Services.Forums.Caching
{
   /// <summary>
   /// Represents a forum subscription cache event consumer
   /// </summary>
   public partial class ForumSubscriptionCacheEventConsumer : CacheEventConsumer<ForumSubscription>
   {
   }
}
