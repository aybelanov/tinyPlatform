using Hub.Core.Domain.Forums;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Forums.Caching
{
   /// <summary>
   /// Represents a forum group cache event consumer
   /// </summary>
   public partial class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(ForumGroup entity)
      {
         await RemoveAsync(AppForumDefaults.ForumByForumGroupCacheKey, entity);
      }
   }
}
