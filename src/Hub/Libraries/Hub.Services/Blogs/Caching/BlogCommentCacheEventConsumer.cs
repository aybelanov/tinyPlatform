﻿using Hub.Core.Domain.Blogs;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Blogs.Caching
{
   /// <summary>
   /// Represents a blog comment cache event consumer
   /// </summary>
   public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(BlogComment entity)
      {
         await RemoveByPrefixAsync(AppBlogsDefaults.BlogCommentsNumberPrefix, entity.BlogPostId);
      }
   }
}