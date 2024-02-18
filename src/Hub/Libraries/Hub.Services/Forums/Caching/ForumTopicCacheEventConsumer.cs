using Hub.Core.Domain.Forums;
using Hub.Services.Caching;

namespace Hub.Services.Forums.Caching
{
   /// <summary>
   /// Represents a forum topic cache event consumer
   /// </summary>
   public partial class ForumTopicCacheEventConsumer : CacheEventConsumer<ForumTopic>
   {
   }
}
