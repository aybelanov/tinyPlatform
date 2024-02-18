using System.Threading.Tasks;
using Hub.Services.Blogs;
using Hub.Core.Domain.Blogs;
using Hub.Services.Caching;

namespace Hub.Services.Blogs.Caching
{
   /// <summary>
   /// Represents a blog post cache event consumer
   /// </summary>
   public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(BlogPost entity)
      {
         await RemoveByPrefixAsync(AppBlogsDefaults.BlogTagsPrefix);
      }
   }
}