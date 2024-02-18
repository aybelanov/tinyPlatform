using Hub.Core.Caching;

namespace Hub.Services.Blogs
{
   /// <summary>
   /// Represents default values related to blogs services
   /// </summary>
   public static partial class AppBlogsDefaults
   {
      #region Caching defaults

      /// <summary>
      /// Key for number of blog comments
      /// </summary>
      /// <remarks>
      /// {0} : blog post ID
      /// {1} : are only approved comments?
      /// </remarks>
      public static CacheKey BlogCommentsNumberCacheKey => new("Hub.blogcomment.number.{0}-{1}", BlogCommentsNumberPrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      /// <remarks>
      /// {0} : blog post ID
      /// </remarks>
      public static string BlogCommentsNumberPrefix => "Hub.blogcomment.number.{0}";

      /// <summary>
      /// Key for blog tag list model
      /// </summary>
      /// <remarks>
      /// {0} : language ID
      /// {1} : show hidden?
      /// </remarks>
      public static CacheKey BlogTagsCacheKey => new("Hub.blogpost.tags.{0}-{1}", BlogTagsPrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      public static string BlogTagsPrefix => "Hub.blogpost.tags.";

      #endregion
   }
}