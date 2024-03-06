using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Monitors;
using Clients.Dash.Services.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;
using Monitor = Clients.Dash.Domain.Monitor;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents the monitor etity service
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class MonitorGrpcService(MonitorRpc.MonitorRpcClient grpcClient,
   ILogger<MonitorGrpcService> logger,
   PermissionService permissionService,
   IStaticCacheManager staticCacheManager,
   IMemoryCache memoryCache,
   ClearCacheService clearCacheService) : IMonitorService
{
   #region Fields

   private readonly ILogger<MonitorGrpcService> _logger = logger;
   private readonly IStaticCacheManager _staticCacheManager = staticCacheManager;
   private readonly IMemoryCache _memoryCache = memoryCache;
   private readonly MonitorRpc.MonitorRpcClient _grpcClient = grpcClient;
   private readonly ClearCacheService _clearCacheService = clearCacheService;
   private readonly PermissionService _permissionService = permissionService;

   private static readonly SemaphoreSlim semaphore = new(1, 1);

   #endregion

   #region Ctor

   #endregion

   #region Methods

   #region Get

   /// <summary>
   /// Gets a monitor view
   /// </summary>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns>Monitor view</returns>
   public async Task<MonitorView> GetMonitorViewAsync(long monitorId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<MonitorView>.ByIdCacheKey, monitorId);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<MonitorView> acquire()
      {
         var proto = await _grpcClient.GetMonitorViewAsync(new() { Id = monitorId });
         var view = Auto.Mapper.Map<MonitorView>(proto);

         return view;
      }
   }

   /// <summary>
   /// Get all monitors (for admins only) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>All monitor entity collection</returns>
   public async Task<IFilterableList<Monitor>> GetAllMonitorsAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByDynamicFilterCacheKey, "all", filter);

      async Task<FilterableList<Monitor>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetAllMonitorsAsync(filterProto);

         var monitors = Auto.Mapper.Map<FilterableList<Monitor>>(query.Monitors);
         monitors.TotalCount = query.TotalCount ?? 0;

         foreach (var monitor in monitors)
         {
            var monitorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByIdCacheKey, monitor.Id);
            await _staticCacheManager.SetAsync(monitorCacheKey, monitor);
         }

         return monitors;
      }

      var result = await _staticCacheManager.GetAsync(cacheKey, acquire);
      return result;
   }


   /// <summary>
   /// Gets all monitor entities of the current user
   /// </summary>
   /// <returns>Monitor collection</returns>
   public async Task<IFilterableList<Monitor>> GetOwnMonitorsAsync(DynamicFilter filter)
   {
      try
      {
         await semaphore.WaitAsync();

         var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByDynamicFilterCacheKey, "own", filter);

         async Task<FilterableList<Monitor>> acquire()
         {
            var filterProto = Auto.Mapper.Map<FilterProto>(filter);
            var query = await _grpcClient.GetOwnMonitorsAsync(filterProto);

            var monitors = Auto.Mapper.Map<FilterableList<Monitor>>(query.Monitors);
            monitors.TotalCount = query.TotalCount ?? 0;

            foreach (var monitor in monitors)
            {
               var monitorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByIdCacheKey, monitor.Id);
               await _staticCacheManager.SetAsync(monitorCacheKey, monitor);
            }

            return monitors;
         }

         return await _staticCacheManager.GetAsync(cacheKey, acquire);
      }
      finally
      {
         semaphore.Release();
      }
   }


   /// <summary>
   /// Gets all monitor entities of the current user
   /// </summary>
   /// <returns>Monitor collection</returns>
   public async Task<IFilterableList<Monitor>> GetSharedMonitorsAsync(DynamicFilter filter)
   {
      try
      {
         await semaphore.WaitAsync();

         var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByDynamicFilterCacheKey, "shared", filter);

         Func<Task<FilterableList<Monitor>>> acquire = async () =>
         {
            var filterProto = Auto.Mapper.Map<FilterProto>(filter);
            var query = await _grpcClient.GetSharedMonitorsAsync(filterProto);

            var monitors = Auto.Mapper.Map<FilterableList<Monitor>>(query.Monitors);
            monitors.TotalCount = query.TotalCount ?? 0;

            foreach (var monitor in monitors)
            {
               var monitorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByIdCacheKey, monitor.Id);
               await _staticCacheManager.SetAsync(monitorCacheKey, monitor);
            }

            return monitors;
         };

         return await _staticCacheManager.GetAsync(cacheKey, acquire);
      }
      finally
      {
         semaphore.Release();
      }
   }

   /// <summary>
   /// Gets a monitor entity by the identifier
   /// </summary>
   /// <param name="id">Identifier</param>
   /// <returns>Monitor instance</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task<Monitor> GetByIdAsync(long id)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByIdCacheKey, id);

      Func<Task<Monitor>> acquire = async () =>
      {
         var filter = new DynamicFilter() { Query = $"query => query.Where(x => x.Id == {id}).Take(1)" };
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);

         var query = await _permissionService.IsAdminModeAsync()
         ? await _grpcClient.GetAllMonitorsAsync(filterProto)
         : await _grpcClient.GetOwnMonitorsAsync(filterProto);

         var monitor = Auto.Mapper.Map<List<Monitor>>(query.Monitors).FirstOrDefault();

         return monitor;
      };

      return await _staticCacheManager.GetAsync(cacheKey, acquire);
   }

   #endregion

   #region Update

   /// <summary>
   /// Updates a monitor entity from the monitor model
   /// </summary>
   /// <param name="model">Monitor model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task UpdateAsync(MonitorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var proto = Auto.Mapper.Map<MonitorProto>(model);
      var reply = await _grpcClient.UpdateAsync(proto);

      // update models
      var monitor = Auto.Mapper.Map<Monitor>(reply);
      Auto.Mapper.Map(monitor, model);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.ByIdPrefix, model.Id);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByIdCacheKey, model.Id);
      if (_memoryCache.TryGetValue(cacheKey.Key, out object value) && value is not null && value is Monitor savedMonitor)
         Auto.Mapper.Map(model, savedMonitor);
   }

   /// <summary>
   /// Updates a shared device entity 
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task UpdateSharedAsync(MonitorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var proto = Auto.Mapper.Map<MonitorProto>(model);
      _ = await _grpcClient.UpdateSharedAsync(proto);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Monitor>.ByDynamicFilterPrefix, "shared");
   }

   #endregion

   #region Insert

   /// <summary>
   /// Adds a monitor entity by the sensor model
   /// </summary>
   /// <param name="model">Monitor model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task InsertAsync(MonitorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);

      var proto = Auto.Mapper.Map<MonitorProto>(model);
      var reply = await _grpcClient.InsertAsync(proto);

      // update models
      var monitor = Auto.Mapper.Map<Monitor>(reply);
      Auto.Mapper.Map(monitor, model);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Monitor>.ByDynamicFilterPrefix, "own");
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Monitor>.ByDynamicFilterPrefix, "all");
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByIdCacheKey, monitor.Id);
      await _staticCacheManager.SetAsync(cacheKey, monitor);
   }

   #endregion

   #region Delete

   /// <summary>
   /// Deletes a monitor entity by the sensor model
   /// </summary>
   /// <param name="model">Monitor model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeleteAsync(MonitorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      await _grpcClient.DeleteAsync(new() { Id = model.Id });

      await _clearCacheService.MonitorClearCache();
   }

   /// <summary>
   /// Deletes a device entity by the sensor model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeleteSharedAsync(MonitorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var deviceProto = Auto.Mapper.Map<MonitorProto>(model);
      _ = await _grpcClient.DeleteSharedAsync(deviceProto);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Monitor>.ByDynamicFilterPrefix, "shared");
   }

   #endregion

   #region Mapping

   /// <summary>
   /// Shares a monitor to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns></returns>
   public async Task ShareMonitorAsync(string userName, long monitorId)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(userName);
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);

      await _grpcClient.ShareMonitorAsync(new() { EntityId = monitorId, UserName = userName });
   }

   /// <summary>
   /// Unshares a monitor to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns></returns>
   public async Task UnshareMonitorAsync(string userName, long monitorId)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(userName);
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);

      await _grpcClient.UnshareMonitorAsync(new() { EntityId = monitorId, UserName = userName });
   }


   #endregion

   #endregion
}
