using Shared.Common;

namespace Clients.Dash.Caching;

/// <summary>
/// Represents default values related to caching entities
/// </summary>
public static partial class CacheDefaults<TEntity> where TEntity : BaseEntity
{
   /// <summary>
   /// Gets an entity type name used in cache keys
   /// </summary>
   public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

   /// <summary>
   /// Gets a key for caching entity by identifier
   /// </summary>
   /// <remarks>
   /// {0} : entity id
   /// </remarks>
   public static CacheKey ByIdCacheKey => new($"Dash.{EntityTypeName}.byid.{{0}}", ByIdPrefix, Prefix);

   /// <summary>
   /// Gets a key for caching entities by identifiers
   /// </summary>
   /// <remarks>
   /// {0} : entity ids
   /// </remarks>
   public static CacheKey ByIdsCacheKey => new($"Dash.{EntityTypeName}.byids.{{0}}", ByIdsPrefix, Prefix);

   /// <summary>
   /// Gets a key for caching all entities
   /// </summary>
   public static CacheKey AllCacheKey => new($"Dash.{EntityTypeName}.all.", AllPrefix, Prefix);

   /// <summary>
   /// Gets a key for caching by dynamic filter
   /// </summary>
   /// <remarks>
   /// {0} : cache set index
   /// {1} : dynamic filter
   /// </remarks>
   public static CacheKey ByDynamicFilterCacheKey => new($"Dash.{EntityTypeName}.bydynamicfilter.{{0}}.{{1}}", ByDynamicFilterPrefix, Prefix);

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string Prefix => $"Dash.{EntityTypeName}.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string ByIdPrefix => $"Dash.{EntityTypeName}.byid.{{0}}";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string ByDynamicFilterPrefix => $"Dash.{EntityTypeName}.bydynamicfilter.{{0}}.";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string ByIdsPrefix => $"Dash.{EntityTypeName}.byids.{{0}}";

   /// <summary>
   /// Gets a key pattern to clear cache
   /// </summary>
   public static string AllPrefix => $"Dash.{EntityTypeName}.all.";
}