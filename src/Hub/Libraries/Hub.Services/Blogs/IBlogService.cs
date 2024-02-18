using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Blogs;

namespace Hub.Services.Blogs
{
   /// <summary>
   /// Blog service interface
   /// </summary>
   public partial interface IBlogService
   {
      #region Blog posts

      /// <summary>
      /// Deletes a blog post
      /// </summary>
      /// <param name="blogPost">Blog post</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteBlogPostAsync(BlogPost blogPost);

      /// <summary>
      /// Gets a blog post
      /// </summary>
      /// <param name="blogPostId">Blog post identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the blog post
      /// </returns>
      Task<BlogPost> GetBlogPostByIdAsync(long blogPostId);

      /// <summary>
      /// Gets all blog posts
      /// </summary>
      /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
      /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
      /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <param name="showHidden">A value indicating whether to show hidden records</param>
      /// <param name="title">Filter by blog post title</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the blog posts
      /// </returns>
      Task<IPagedList<BlogPost>> GetAllBlogPostsAsync(long languageId = 0, DateTime? dateFrom = null, DateTime? dateTo = null,
          int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null);

      /// <summary>
      /// Gets all blog posts
      /// </summary>
      /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
      /// <param name="tag">Tag</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <param name="showHidden">A value indicating whether to show hidden records</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the blog posts
      /// </returns>
      Task<IPagedList<BlogPost>> GetAllBlogPostsByTagAsync(long languageId = 0, string tag = "",
          int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

      /// <summary>
      /// Gets all blog post tags
      /// </summary>
      /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
      /// <param name="showHidden">A value indicating whether to show hidden records</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the blog post tags
      /// </returns>
      Task<IList<BlogPostTag>> GetAllBlogPostTagsAsync(long languageId, bool showHidden = false);

      /// <summary>
      /// Inserts a blog post
      /// </summary>
      /// <param name="blogPost">Blog post</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertBlogPostAsync(BlogPost blogPost);

      /// <summary>
      /// Updates the blog post
      /// </summary>
      /// <param name="blogPost">Blog post</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateBlogPostAsync(BlogPost blogPost);

      /// <summary>
      /// Returns all posts published between the two dates.
      /// </summary>
      /// <param name="blogPosts">Source</param>
      /// <param name="dateFrom">Date from</param>
      /// <param name="dateTo">Date to</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the filtered posts
      /// </returns>
      Task<IList<BlogPost>> GetPostsByDateAsync(IList<BlogPost> blogPosts, DateTime dateFrom, DateTime dateTo);

      /// <summary>
      /// Parse tags
      /// </summary>
      /// <param name="blogPost">Blog post</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the ags
      /// </returns>
      Task<IList<string>> ParseTagsAsync(BlogPost blogPost);

      /// <summary>
      /// Get a value indicating whether a blog post is available now (availability dates)
      /// </summary>
      /// <param name="blogPost">Blog post</param>
      /// <param name="dateTime">Datetime to check; pass null to use current date</param>
      /// <returns>Result</returns>
      bool BlogPostIsAvailable(BlogPost blogPost, DateTime? dateTime = null);

      #endregion

      #region Blog comments

      /// <summary>
      /// Gets all comments
      /// </summary>
      /// <param name="userId">User identifier; 0 to load all records</param>
      /// <param name="blogPostId">Blog post ID; 0 or null to load all records</param>
      /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
      /// <param name="fromUtc">Item creation from; null to load all records</param>
      /// <param name="toUtc">Item creation to; null to load all records</param>
      /// <param name="commentText">Search comment text; null to load all records</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the comments
      /// </returns>
      Task<IList<BlogComment>> GetAllCommentsAsync(long userId = 0, long? blogPostId = null,
          bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null);

      /// <summary>
      /// Gets a blog comment
      /// </summary>
      /// <param name="blogCommentId">Blog comment identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the blog comment
      /// </returns>
      Task<BlogComment> GetBlogCommentByIdAsync(long blogCommentId);

      /// <summary>
      /// Get blog comments by identifiers
      /// </summary>
      /// <param name="commentIds">Blog comment identifiers</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the blog comments
      /// </returns>
      Task<IList<BlogComment>> GetBlogCommentsByIdsAsync(long[] commentIds);

      /// <summary>
      /// Get the count of blog comments
      /// </summary>
      /// <param name="blogPost">Blog post</param>
      /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of blog comments
      /// </returns>
      Task<int> GetBlogCommentsCountAsync(BlogPost blogPost, bool? isApproved = null);

      /// <summary>
      /// Deletes a blog comment
      /// </summary>
      /// <param name="blogComment">Blog comment</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteBlogCommentAsync(BlogComment blogComment);

      /// <summary>
      /// Deletes blog comments
      /// </summary>
      /// <param name="blogComments">Blog comments</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteBlogCommentsAsync(IList<BlogComment> blogComments);

      /// <summary>
      /// Inserts a blog comment
      /// </summary>
      /// <param name="blogComment">Blog comment</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertBlogCommentAsync(BlogComment blogComment);

      /// <summary>
      /// Update a blog comment
      /// </summary>
      /// <param name="blogComment">Blog comment</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateBlogCommentAsync(BlogComment blogComment);

      #endregion
   }
}
