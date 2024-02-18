using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Monitors;
using Clients.Dash.Services.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents the monitor etity service
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class PresentationGrpcService(PresentationRpc.PresentationRpcClient grpcClient,
   ILogger<MonitorGrpcService> logger,
   IStaticCacheManager staticCacheManager,
   IMemoryCache memoryCache,
   PermissionService permissionService,
   ClearCacheService clearCacheService) : IPresentationService
{
   private readonly ILogger<MonitorGrpcService> _logger = logger;
   private readonly IStaticCacheManager _staticCacheManager = staticCacheManager;
   private readonly IMemoryCache _memoryCache = memoryCache;
   private readonly PresentationRpc.PresentationRpcClient _grpcClient = grpcClient;
   private readonly ClearCacheService _clearCacheService = clearCacheService;
   private readonly PermissionService _permissionService = permissionService;

   /// <summary>
   /// Gets monitor'presentation by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Monitor presentation entity collection</returns>
   public async Task<IFilterableList<Presentation>> GetPresentationsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter?.MonitorId);
      ArgumentOutOfRangeException.ThrowIfLessThan(filter.MonitorId.Value, 1);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Presentation>.ByDynamicFilterCacheKey, "all", filter);

      Func<Task<IFilterableList<Presentation>>> acquire = async () =>
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var reply = await _grpcClient.GetPresentationsAsync(filterProto);
         var presetnations = Auto.Mapper.Map<FilterableList<Presentation>>(reply.Presentations);
         presetnations.TotalCount = reply.TotalCount ?? 0;

         foreach (var presentation in presetnations)
         {
            var presentationCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Presentation>.ByIdCacheKey, presentation.Id);
            await _staticCacheManager.SetAsync(presentationCacheKey, presentation);
         }

         return presetnations;
      };

      return await _staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets all sensor-to-widget mapping select list by the dynamic filter (for admins only)
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<PresentationSelectItem>> GetAllPresentationSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<PresentationSelectItem>.ByDynamicFilterCacheKey, "all", filter);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<PresentationSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetAllPresentationSelectItemsAsync(filterProto);
         var sensorWidgets = Auto.Mapper.Map<FilterableList<PresentationSelectItem>>(query.Presentations);
         sensorWidgets.TotalCount = query.TotalCount ?? 0;

         foreach (var item in sensorWidgets)
         {
            var ck = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<PresentationSelectItem>.ByIdCacheKey, item.Id);
            await _staticCacheManager.SetAsync(ck, item);
         }

         return sensorWidgets;
      }
   }

   /// <summary>
   /// Gets own (user) sensor-to-widget mapping select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<PresentationSelectItem>> GetOwnPresentationSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<PresentationSelectItem>.ByDynamicFilterCacheKey, "own", filter);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<PresentationSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetOwnPresentationSelectItemsAsync(filterProto);
         var sensorWidgets = Auto.Mapper.Map<FilterableList<PresentationSelectItem>>(query.Presentations);
         sensorWidgets.TotalCount = query.TotalCount ?? 0;

         foreach (var item in sensorWidgets)
         {
            var ck = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<PresentationSelectItem>.ByIdCacheKey, item.Id);
            await _staticCacheManager.SetAsync(ck, item);
         }

         return sensorWidgets;
      }
   }

   /// <summary>
   /// Gets a presentation select item by the identifier 
   /// </summary>
   /// <param name="sensorWidgetId">Sensor widget identifier</param>
   /// <returns>Presentation select item</returns>
   public async Task<PresentationSelectItem> GetPresentationSelectItemAsync(long sensorWidgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorWidgetId, 1);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<PresentationSelectItem>.ByIdCacheKey, sensorWidgetId);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<PresentationSelectItem> acquire()
      {
         var item = (await _permissionService.IsAdminModeAsync()
            ? await _grpcClient.GetAllPresentationSelectItemsAsync(new() { SensorWidgetId = sensorWidgetId })
            : await _grpcClient.GetOwnPresentationSelectItemsAsync(new() { SensorWidgetId = sensorWidgetId }))
            .Presentations
            .FirstOrDefault();

         return Auto.Mapper.Map<PresentationSelectItem>(item);
      }
   }

   /// <summary>
   /// Maps a presentation (sensor-to-widget) to a monitor
   /// </summary>
   /// <param name="model">Monitor mapping model</param>
   /// <returns></returns>
   public async Task MapPresentationToMonitorAsync(PresentationModel model)
   {
      ArgumentNullException.ThrowIfNull(model);

      var proto = Auto.Mapper.Map<PresentationProto>(model);
      var reply = await _grpcClient.MapPresentationAsync(proto);

      // update models
      var presentation = Auto.Mapper.Map<Presentation>(reply);
      Auto.Mapper.Map(presentation, model);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);

      var monitorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<MonitorView>.ByIdCacheKey, model.MonitorId);
      await _staticCacheManager.RemoveAsync(monitorCacheKey, model.MonitorId);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Presentation>.ByIdCacheKey, presentation.Id);
      await _staticCacheManager.SetAsync(cacheKey, presentation);
   }

   /// <summary>
   /// Maps a presentation (sensor-to-widget) to a monitor
   /// </summary>
   /// <param name="model">Monitor mapping model</param>
   /// <returns></returns>
   public async Task UpdateMapPresentationToMonitorAsync(PresentationModel model)
   {
      ArgumentNullException.ThrowIfNull(model);

      var proto = Auto.Mapper.Map<PresentationProto>(model);
      var reply = await _grpcClient.UpdateMapPresentationAsync(proto);

      // update models
      var presentation = Auto.Mapper.Map<Presentation>(reply);
      Auto.Mapper.Map(presentation, model);

      var monitorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<MonitorView>.ByIdCacheKey, model.MonitorId);
      await _staticCacheManager.RemoveAsync(monitorCacheKey, model.MonitorId);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Presentation>.ByIdCacheKey, model.Id);
      if (_memoryCache.TryGetValue(cacheKey.Key, out object value) && value is not null && value is Presentation savedPresentation)
         Auto.Mapper.Map(model, savedPresentation);
   }

   /// <summary>
   /// Unmaps a presentation (sensor-to-widget) from a monitor
   /// </summary>
   /// <param name="model">Monitor mapping model</param>
   /// <returns></returns>
   public async Task UnmapPresentaionFromMonitorAsync(PresentationModel model)
   {
      ArgumentNullException.ThrowIfNull(model);

      await _grpcClient.UnmapPresentationAsync(new() { Id = model.Id });
      await _clearCacheService.PresentationClearCache();
   }

   /// <summary>
   /// Maps a sensor to a widget
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns></returns>
   public async Task MapSensorToWidgetAsync(long sensorId, long widgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(widgetId, 1);

      await _grpcClient.MapSensorToWidgetAsync(new() { SensorId = sensorId, WidgetId = widgetId });

      await _clearCacheService.PresentationClearCache();
   }

   /// <summary>
   /// Maps a sensor to a widget
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns></returns>
   public async Task UnmapSensorFromWidgetAsync(long sensorId, long widgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(widgetId, 1);

      await _grpcClient.UnmapSensorFromWidgetAsync(new() { SensorId = sensorId, WidgetId = widgetId });

      await _clearCacheService.PresentationClearCache();
   }
}
