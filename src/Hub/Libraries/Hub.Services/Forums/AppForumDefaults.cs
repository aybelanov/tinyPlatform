using Hub.Core.Caching;

namespace Hub.Services.Forums
{
   /// <summary>
   /// Represents default values related to forums services
   /// </summary>
   public static partial class AppForumDefaults
   {
      #region Caching defaults

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : forum group ID
      /// </remarks>
      public static CacheKey ForumByForumGroupCacheKey => new("Hub.forum.byforumgroup.{0}");

      #endregion
   }
}