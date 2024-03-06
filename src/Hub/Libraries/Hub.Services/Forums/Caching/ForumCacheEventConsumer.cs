using Hub.Core.Domain.Forums;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Forums.Caching
{
   /// <summary>
   /// Represents a forum cache event consumer
   /// </summary>
   public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(Forum entity)
      {
         await RemoveAsync(AppForumDefaults.ForumByForumGroupCacheKey, entity.ForumGroupId);
      }
   }
}
