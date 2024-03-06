using Hub.Core.Caching;
using Hub.Core.Domain.Topics;

namespace Hub.Services.Topics
{
   /// <summary>
   /// Represents default values related to topic services
   /// </summary>
   public static partial class AppTopicDefaults
   {
      #region Caching defaults

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : show hidden?
      /// {1} : include in top menu?
      /// </remarks>
      public static CacheKey TopicsAllCacheKey => new("Hub.topic.all.{0}-{1}", AppEntityCacheDefaults<Topic>.AllPrefix);

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : show hidden?
      /// {1} : include in top menu?
      /// {2} : user role IDs hash
      /// </remarks>
      public static CacheKey TopicsAllWithACLCacheKey => new("Hub.topic.all.withacl.{0}-{1}-{2}", AppEntityCacheDefaults<Topic>.AllPrefix);

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : topic system name
      /// {1} : user roles Ids hash
      /// </remarks>
      public static CacheKey TopicBySystemNameCacheKey => new("Hub.topic.bysystemname.{0}-{1}", TopicBySystemNamePrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      /// <remarks>
      /// {0} : topic system name
      /// </remarks>
      public static string TopicBySystemNamePrefix => "Hub.topic.bysystemname.{0}";

      #endregion
   }
}