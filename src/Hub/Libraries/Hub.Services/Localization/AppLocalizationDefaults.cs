using Hub.Core.Caching;
using Hub.Core.Domain.Localization;

namespace Hub.Services.Localization;

/// <summary>
/// Represents default values related to localization services
/// </summary>
public static partial class AppLocalizationDefaults
{
   #region Locales

   /// <summary>
   /// Gets a prefix of locale resources for the admin area
   /// </summary>
   public static string AdminLocaleStringResourcesPrefix => "Admin.";

   /// <summary>
   /// Gets a prefix of locale resources for enumerations 
   /// </summary>
   public static string EnumLocaleStringResourcesPrefix => "Enums.";

   /// <summary>
   /// Gets a prefix of locale resources for permissions 
   /// </summary>
   public static string PermissionLocaleStringResourcesPrefix => "Permission.";

   /// <summary>
   /// Gets a prefix of locale resources for plugin friendly names 
   /// </summary>
   public static string PluginNameLocaleStringResourcesPrefix => "Plugins.FriendlyName.";

   #endregion

   #region Caching defaults

   #region Languages

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : show hidden records?
   /// </remarks>
   public static CacheKey LanguagesAllCacheKey => new("App.language.all.{0}", AppEntityCacheDefaults<Language>.AllPrefix);

   #endregion

   #region Locales

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static CacheKey LocaleStringResourcesAllPublicCacheKey => new("App.localestringresource.bylanguage.public.{0}", AppEntityCacheDefaults<LocaleStringResource>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static CacheKey LocaleStringResourcesAllAdminCacheKey => new("App.localestringresource.bylanguage.admin.{0}", AppEntityCacheDefaults<LocaleStringResource>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static CacheKey LocaleStringResourcesAllCacheKey => new("App.localestringresource.bylanguage.{0}", AppEntityCacheDefaults<LocaleStringResource>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// {1} : resource key
   /// </remarks>
   public static CacheKey LocaleStringResourcesByNameCacheKey => new("App.localestringresource.byname.{0}-{1}", LocaleStringResourcesByNamePrefix, AppEntityCacheDefaults<LocaleStringResource>.Prefix);

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static string LocaleStringResourcesByNamePrefix => "App.localestringresource.byname.{0}";

   #endregion

   #region Localized properties

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// {1} : entity ID
   /// {2} : locale key group
   /// {3} : locale key
   /// </remarks>
   public static CacheKey LocalizedPropertyCacheKey => new("App.localizedproperty.value.{0}-{1}-{2}-{3}");

   #endregion

   #endregion
}