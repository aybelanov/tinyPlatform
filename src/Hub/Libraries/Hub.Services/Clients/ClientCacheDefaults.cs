using Hub.Core.Caching;
using Shared.Common;

namespace Hub.Services.Clients;

/// <summary>
/// Represents default values related to caching entities
/// </summary>
public static class ClientCacheDefaults<TEntity> where TEntity : BaseEntity
{
   #region Cache

   #region base
   /// <summary>
   /// Gets an entity type name used in cache keys
   /// </summary>
   public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string Prefix => $"App.{EntityTypeName}.";

   /// <summary>
   /// Gets a key pattern to clear cache by the user
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// </remarks>
   public static string UserPrefix => $"App.{EntityTypeName}.byuser.{{0}}.";

   /// <summary>
   /// Gets a key pattern to clear cache by the user
   /// </summary>
   /// <remarks>
   /// {0} : Device id
   /// </remarks>
   public static string DevicePrefix => $"App.{EntityTypeName}.bydevice.{{0}}.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string AllPrefix => $"App.{EntityTypeName}.all.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string ByIdPrefix => $"App.{EntityTypeName}.byid.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string BySystemNamePrefix => $"App.{EntityTypeName}.bysystemname.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string ByGuidPrefix => $"App.{EntityTypeName}.byguid.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string OwnDevicesByUserPrefix => $"App.{EntityTypeName}.owndevices.byuserid.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// {1} : Language id
   /// </remarks>
   public static string UserLocalePrefix => $"App.{EntityTypeName}.locale.{{0}}.{{1}}.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// </remarks>
   public static string UserAllLocalePrefix => $"App.{EntityTypeName}.locale.{{0}}.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// </remarks>
   public static string AllLocalePrefix => $"App.{EntityTypeName}.locale.";

   #endregion

   #region by Entity property

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : Entity id
   /// </remarks>
   public static CacheKey ByIdCacheKey => new($"App.{EntityTypeName}.byid.{{0}}", Prefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// {1} : Entity ids[]
   /// </remarks>
   public static CacheKey ByIdsCacheKey => new($"App.{EntityTypeName}.byids.{{1}}", Prefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : System name
   /// </remarks>
   public static CacheKey BySystemNameCacheKey => new($"App.{EntityTypeName}.bysystemname.{{0}}", Prefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : System name
   /// {1} : Device identifier
   /// </remarks>
   public static CacheKey BySensorSystemNameCacheKey => new($"App.{EntityTypeName}.bysystemname.{{0}}.{{1}}", Prefix);

   /// <summary>
   /// Get all entities
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// {1} : Language id
   /// </remarks>
   public static CacheKey ByUserAndLangCacheKey => new($"App.{EntityTypeName}.byuser.{{0}}.{{1}}.all", UserPrefix);

   /// <summary>
   /// Get all entities
   /// </summary>
   /// <remarks>
   /// {0} : Device id
   /// {1} : Language id
   /// </remarks>
   public static CacheKey ByDeviceAndLangCacheKey => new($"App.{EntityTypeName}.bydevice.{{0}}.{{1}}.all", DevicePrefix);

   /// <summary>
   /// Gets a key for caching by dynamic filter
   /// </summary>
   /// <remarks>
   /// {0} : cache set index
   /// {1} : dynamic filter
   /// </remarks>
   public static CacheKey ByDynamicFilterCacheKey => new($"App.{EntityTypeName}.bydynamicfilter.{{0}}.{{1}}", ByDynamicFilterPrefix, Prefix);

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string ByDynamicFilterPrefix => $"App.{EntityTypeName}.bydynamicfilter.{{0}}.";

   #endregion

   #region by User

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// /// <remarks>
   /// {0} : User Id
   /// </remarks>
   public static CacheKey ByUserIdCacheKey => new($"App.{EntityTypeName}.{{0}}.byuserid", Prefix, UserPrefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// /// <remarks>
   /// {0} : User Id
   /// </remarks>
   public static CacheKey OwnDevicesByUserCacheKey => new($"App.{EntityTypeName}.owndevices.byuserid.{{0}}", Prefix, OwnDevicesByUserPrefix);

   #endregion


   #region by Device

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : device id
   /// </remarks>
   public static CacheKey ByDeviceIdCacheKey => new($"App.{EntityTypeName}.bydeviceid.{{0}}", Prefix);

   #endregion

   #region by Monitor

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// {1} : monitor id
   /// </remarks>
   public static CacheKey ByMonitorIdCacheKey => new($"App.{EntityTypeName}.{{0}}.bymonitorid.{{1}}", Prefix, UserPrefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// {1} : monitor ids[]
   /// </remarks>
   public static CacheKey ByMonitorIdsCacheKey => new($"App.{EntityTypeName}.{{0}}.bymonitorids.{{1}}", Prefix, UserPrefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : User id
   /// </remarks>
   public static CacheKey AllMonitorSensorWidgetCacheKey => new($"App.{EntityTypeName}.{{0}}.AllMonitorSensorWidget", Prefix, UserPrefix);

   #endregion

   #region by Download task

   /// <summary>
   /// Gets a key for caching entities
   /// </summary>
   public static CacheKey ByLastEntityCacheKey => new($"App.{EntityTypeName}.{{0}}.bylastentity", Prefix);

   #endregion

   #endregion
}
