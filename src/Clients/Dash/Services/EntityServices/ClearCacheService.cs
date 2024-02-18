using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Represents a clear cache service class
/// </summary>
public class ClearCacheService 
{
   #region fields

   private readonly ILogger<ClearCacheService> _logger;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="logger"></param>
   /// <param name="staticCacheManager"></param>
   public ClearCacheService(ILogger<ClearCacheService> logger, IStaticCacheManager staticCacheManager)
   {
      _logger = logger;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Clear monitor instance cache 
   /// </summary>
   /// <returns></returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task MonitorClearCache()
   {
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Monitor>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
   }

   /// <summary>
   /// Clear widget instance cache 
   /// </summary>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   public async Task WidgetClearCache()
   {

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Widget>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);

   }

   /// <summary>
   /// Clear device instance cache 
   /// </summary>
   /// <returns></returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeviceClearCache()
   {
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Device>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Sensor>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);

   }

   /// <summary>
   /// Clear sensor instance cache 
   /// </summary>
   /// <returns></returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task SensorClearCache()
   {
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Sensor>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);

   }

   /// <summary>
   /// Clear sensor instance cache 
   /// </summary>
   /// <returns></returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task PresentationClearCache()
   {
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);
   }

   #endregion
}
