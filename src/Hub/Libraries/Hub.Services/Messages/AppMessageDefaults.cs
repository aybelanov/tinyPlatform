using Hub.Core.Caching;
using Hub.Core.Domain.Messages;

namespace Hub.Services.Messages
{
   /// <summary>
   /// Represents default values related to messages services
   /// </summary>
   public static partial class AppMessageDefaults
   {
      /// <summary>
      /// Gets a key for notifications list from TempDataDictionary
      /// </summary>
      public static string NotificationListKey => "NotificationList";

      #region Caching defaults

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// </remarks>
      public static CacheKey MessageTemplatesAllCacheKey => new("App.messagetemplate.all", AppEntityCacheDefaults<MessageTemplate>.AllPrefix);

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : template name
      /// </remarks>
      public static CacheKey MessageTemplatesByNameCacheKey => new("App.messagetemplate.byname.{0}", MessageTemplatesByNamePrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      /// <remarks>
      /// {0} : template name
      /// </remarks>
      public static string MessageTemplatesByNamePrefix => "App.messagetemplate.byname.{0}";

      #endregion
   }
}