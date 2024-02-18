using Hub.Core.Domain.Forums;
using Hub.Services.Caching;

namespace Hub.Services.Forums.Caching
{
   /// <summary>
   /// Represents a forum post cache event consumer
   /// </summary>
   public partial class ForumPostCacheEventConsumer : CacheEventConsumer<ForumPost>
   {
   }
}
