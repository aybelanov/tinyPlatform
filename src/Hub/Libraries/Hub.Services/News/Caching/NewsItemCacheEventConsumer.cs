using Hub.Services.News;
using Hub.Core.Domain.News;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.News.Caching
{
   /// <summary>
   /// Represents a news item cache event consumer
   /// </summary>
   public partial class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <param name="entityEventType">Entity event type</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(NewsItem entity, EntityEventType entityEventType)
      {
         if (entityEventType == EntityEventType.Delete)
            await RemoveByPrefixAsync(AppNewsDefaults.NewsCommentsNumberPrefix, entity);

         await base.ClearCacheAsync(entity, entityEventType);
      }
   }
}