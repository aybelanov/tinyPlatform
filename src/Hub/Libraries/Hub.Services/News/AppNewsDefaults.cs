using Hub.Core.Caching;

namespace Hub.Services.News
{
   /// <summary>
   /// Represents default values related to orders services
   /// </summary>
   public static partial class AppNewsDefaults
   {
      #region Caching defaults

      /// <summary>
      /// Key for number of news comments
      /// </summary>
      /// <remarks>
      /// {0} : news item ID
      /// {1} : are only approved comments?
      /// </remarks>
      public static CacheKey NewsCommentsNumberCacheKey => new("Hub.newsitem.comments.number.{0}-{1}", NewsCommentsNumberPrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      /// <remarks>
      /// {0} : news item ID
      /// </remarks>
      public static string NewsCommentsNumberPrefix => "Hub.newsitem.comments.number.{0}";

      #endregion
   }
}