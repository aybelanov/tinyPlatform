using Hub.Core.Configuration;
using Shared.Clients;
using Shared.Common;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Hub.Core.Caching;

/// <summary>
/// Represents the default cache key service implementation
/// </summary>
public abstract partial class CacheKeyService
{
   #region Constants

   /// <summary>
   /// Gets an algorithm used to create the hash value of identifiers need to cache
   /// </summary>
   private string HashAlgorithm => "SHA1";

   #endregion

   #region Fields
   /// <summary>
   /// Application settings
   /// </summary>
   protected readonly AppSettings _appSettings;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="appSettings"></param>
   protected CacheKeyService(AppSettings appSettings)
   {
      _appSettings = appSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare the cache key prefix
   /// </summary>
   /// <param name="prefix">Cache key prefix</param>
   /// <param name="prefixParameters">Parameters to create cache key prefix</param>
   protected virtual string PrepareKeyPrefix(string prefix, params object[] prefixParameters)
   {
      return prefixParameters?.Any() ?? false
          ? string.Format(prefix, prefixParameters.Select(CreateCacheKeyParameters).ToArray())
          : prefix;
   }

   /// <summary>
   /// Create the hash value of the passed identifiers
   /// </summary>
   /// <param name="ids">Collection of identifiers</param>
   /// <returns>String hash value</returns>
   protected virtual string CreateIdsHash(IEnumerable<long> ids)
   {
      var identifiers = ids.ToList();

      if (!identifiers.Any())
         return string.Empty;

      var identifiersString = string.Join(", ", identifiers.OrderBy(id => id));
      return HashHelper.CreateHash(Encoding.UTF8.GetBytes(identifiersString), HashAlgorithm);
   }

   /// <summary>
   /// Converts an object to cache parameter
   /// </summary>
   /// <param name="parameter">Object to convert</param>
   /// <returns>Cache parameter</returns>
   protected virtual object CreateCacheKeyParameters(object parameter)
   {
      object res = parameter switch
      {
         null => "null",
         IEnumerable<long> ids => CreateIdsHash(ids),
         IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(entity => entity.Id)),
         BaseEntity entity => entity.Id,
         decimal param => param.ToString(CultureInfo.InvariantCulture),
         DynamicFilter filter => HashHelper.CreateHash(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(filter)), HashAlgorithm),
         _ => parameter
      };

      return res;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Create a copy of cache key and fills it by passed parameters
   /// </summary>
   /// <param name="cacheKey">Initial cache key</param>
   /// <param name="cacheKeyParameters">Parameters to create cache key</param>
   /// <returns>Cache key</returns>
   public virtual CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters)
   {
      return cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
   }

   /// <summary>
   /// Create a copy of cache key using the default cache time and fills it by passed parameters
   /// </summary>
   /// <param name="cacheKey">Initial cache key</param>
   /// <param name="cacheKeyParameters">Parameters to create cache key</param>
   /// <returns>Cache key</returns>
   public virtual CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters)
   {
      var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);

      key.CacheTime = _appSettings.Get<CacheConfig>().DefaultCacheTime;

      return key;
   }

   /// <summary>
   /// Create a copy of cache key using the short cache time and fills it by passed parameters
   /// </summary>
   /// <param name="cacheKey">Initial cache key</param>
   /// <param name="cacheKeyParameters">Parameters to create cache key</param>
   /// <returns>Cache key</returns>
   public virtual CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] cacheKeyParameters)
   {
      var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);

      key.CacheTime = _appSettings.Get<CacheConfig>().ShortTermCacheTime;

      return key;
   }

   #endregion
}