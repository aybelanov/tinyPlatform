using Hub.Core.Caching;
using Hub.Core.Domain.Configuration;

namespace Hub.Services.Configuration
{
   /// <summary>
   /// Represents default values related to settings
   /// </summary>
   public static partial class AppSettingsDefaults
   {
      #region Caching defaults

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      public static CacheKey SettingsAllAsDictionaryCacheKey => new("App.setting.all.dictionary.", AppEntityCacheDefaults<Setting>.Prefix);

      #endregion
   }
}