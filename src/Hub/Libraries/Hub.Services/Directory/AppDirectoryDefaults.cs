using Hub.Core.Caching;
using Hub.Core.Domain.Directory;

namespace Hub.Services.Directory;

/// <summary>
/// Represents default values related to directory services
/// </summary>
public static partial class AppDirectoryDefaults
{
   #region Caching defaults

   #region Countries

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : Two letter ISO code
   /// </remarks>
   public static CacheKey CountriesByTwoLetterCodeCacheKey => new("App.country.bytwoletter.{0}", AppEntityCacheDefaults<Country>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : Two letter ISO code
   /// </remarks>
   public static CacheKey CountriesByThreeLetterCodeCacheKey => new("App.country.bythreeletter.{0}", AppEntityCacheDefaults<Country>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// {1} : show hidden records?
   /// </remarks>
   public static CacheKey CountriesAllCacheKey => new("App.country.all.{0}-{1}", AppEntityCacheDefaults<Country>.Prefix);

   #endregion

   #region Currencies

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : show hidden records?
   /// </remarks>
   public static CacheKey CurrenciesAllCacheKey => new("App.currency.all.{0}", AppEntityCacheDefaults<Currency>.AllPrefix);

   #endregion

   #region States and provinces

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : country ID
   /// {1} : language ID
   /// {2} : show hidden records?
   /// </remarks>
   public static CacheKey StateProvincesByCountryCacheKey => new("App.stateprovince.bycountry.{0}-{1}-{2}", AppEntityCacheDefaults<StateProvince>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : show hidden records?
   /// </remarks>
   public static CacheKey StateProvincesAllCacheKey => new("App.stateprovince.all.{0}", AppEntityCacheDefaults<StateProvince>.Prefix);

   /// <summary>
   /// Gets a key for caching
   /// </summary>
   /// <remarks>
   /// {0} : abbreviation
   /// {1} : country ID
   /// </remarks>
   public static CacheKey StateProvincesByAbbreviationCacheKey => new("App.stateprovince.byabbreviation.{0}-{1}", AppEntityCacheDefaults<StateProvince>.Prefix);

   #endregion

   #endregion
}
