using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Infrastructure.AutoMapper.Extensions;
using Clients.Dash.Models;
using Clients.Dash.Pages.Configuration.Sensors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents the sensor entity service 
/// </summary>
public class SensorGrpcService : ISensorService
{
   #region fields

   private readonly ILogger<SensorGrpcService> _logger;
   private readonly SensorRpc.SensorRpcClient _grpcClient;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly ClearCacheService _clearCacheService;
   private readonly IMemoryCache _memoryCache;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public SensorGrpcService(SensorRpc.SensorRpcClient grpcClient, ILogger<SensorGrpcService> logger, IStaticCacheManager staticCacheManager,
      ClearCacheService clearCacheService, IMemoryCache memoryCache)
   {
      _grpcClient = grpcClient;
      _logger = logger;
      _staticCacheManager = staticCacheManager;
      _clearCacheService = clearCacheService;
      _memoryCache = memoryCache;
   }

   #endregion

   #region Methods

   #region Get

   /// <summary>
   /// Gets all sensor entities of the current user
   /// </summary>
   /// <returns>Sensor collection</returns>
   public async Task<IFilterableList<Sensor>> GetSensorsAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Sensor>.ByDynamicFilterCacheKey, "all", filter);

      Func<Task<FilterableList<Sensor>>> acquire = async () =>
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetSensorsAsync(filterProto);

         var sensors = Auto.Mapper.Map<FilterableList<Sensor>>(query.Sensors);
         sensors.TotalCount = query.TotalCount ?? 0;

         foreach (var sensor in sensors)
         {
            var sensorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Sensor>.ByIdCacheKey, sensor.Id);
            await _staticCacheManager.SetAsync(sensorCacheKey, sensor);
         }

         return sensors;
      };

      return await _staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets a sensor entity by the identifier
   /// </summary>
   /// <param name="id">Identifier</param>
   /// <returns>Sensor instance</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task<Sensor> GetSensorByIdAsync(long id)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Sensor>.ByIdCacheKey, id);

      Func<Task<Sensor>> acquire = async () =>
      {
         var filter = new DynamicFilter() { SensorId = id };
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetSensorsAsync(filterProto);

         var sensor = Auto.Mapper.Map<List<Sensor>>(query.Sensors).FirstOrDefault();

         return sensor;
      };

      return await _staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets sensor entities by the identifiers
   /// </summary>
   /// <param name="ids">Identifiers</param>
   /// <returns>Sensor collection</returns>
   /// <exception cref="ArgumentNullException"></exception>
   public async Task<IFilterableList<Sensor>> GetSensorByIdsAsync(IEnumerable<long> ids)
   {
      ArgumentNullException.ThrowIfNull(ids);

      if (!ids.Any())
         return new FilterableList<Sensor>();

      var filter = new DynamicFilter() { Query = $"query => query.Where(x => @0.Contains(x.Id))", Ids = ids.ToArray() };

      var sensors = await GetSensorsAsync(filter);

      foreach (var sensor in sensors)
      {
         var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Sensor>.ByIdCacheKey, sensor.Id);
         await _staticCacheManager.SetAsync(cacheKey, sensor);
      }

      return sensors;
   }

   /// <summary>
   /// Get sensor select item list for all devices (admin mode)
   /// </summary>
   /// <returns>Sensor select item collection</returns>
   public async Task<IFilterableList<SensorSelectItem>> GetForAllSensorSelectItemListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<SensorSelectItem>.ByDynamicFilterCacheKey, "all", filter);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<SensorSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetForAllSensorSelectListAsync(filterProto);
         var sensors = Auto.Mapper.Map<FilterableList<SensorSelectItem>>(query.Sensors);
         sensors.TotalCount = query.TotalCount ?? 0;

         return sensors;
      }
   }

   /// <summary>
   /// Get sensor select item list
   /// </summary>
   /// <returns>Sensor select item collection</returns>
   public async Task<IFilterableList<SensorSelectItem>> GetSensorSelectItemListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<SensorSelectItem>.ByDynamicFilterCacheKey, "all", filter);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<SensorSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetSensorSelectListAsync(filterProto);
         var sensors = Auto.Mapper.Map<FilterableList<SensorSelectItem>>(query.Sensors);
         sensors.TotalCount = query.TotalCount ?? 0;

         return sensors;
      }
   }

   #endregion

   #region Update

   /// <summary>
   /// Updates a sensor entity from the sensor model
   /// </summary>
   /// <param name="model">Sensor model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task UpdateAsync(SensorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var proto = Auto.Mapper.Map<SensorProto>(model);
      var reply = await _grpcClient.UpdateAsync(proto);

      // update models
      var sensor = Auto.Mapper.Map<Sensor>(reply);
      Auto.Mapper.Map(sensor, model);

    
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Sensor>.ByIdCacheKey, model.Id);
      if (_memoryCache.TryGetValue(cacheKey.Key, out object value) && value is not null && value is Sensor savedSensor)
         Auto.Mapper.Map(model, savedSensor);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);
   }

   #endregion

   #region Insert

   /// <summary>
   /// Adds a sensor entity by the sensor model
   /// </summary>
   /// <param name="model">Sensor model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task InsertAsync(SensorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.DeviceId, 1);

      // insert in data base
      var proto = Auto.Mapper.Map<SensorProto>(model);
      var reply = await _grpcClient.InsertAsync(proto);

      // update models
      var sensor = Auto.Mapper.Map<Sensor>(reply);
      Auto.Mapper.Map(sensor, model);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Sensor>.ByDynamicFilterPrefix, "all");
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Sensor>.ByIdCacheKey, sensor.Id);
      await _staticCacheManager.SetAsync(cacheKey, sensor);
   }

   #endregion

   #region Delete

   /// <summary>
   /// Deletes a sensor entity by the sensor model
   /// </summary>
   /// <param name="model">Sensor model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeleteAsync(SensorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      _ = await _grpcClient.DeleteAsync(new() { Id = model.Id });

      await _clearCacheService.SensorClearCache();
   }

   #endregion

   #region Common

   /// <summary>
   /// Checks sensor sytemname availability
   /// </summary>
   /// <param name="systemName">Checking systemname</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Response with the result: success or error coolecyion</returns>
   public async Task<CommonResponse> CheckSystemNameAvailabilityAsync(string systemName, long deviceId)
   {
      var result = await _grpcClient.CheckSystemNameAvailabilityAsync(new() { SystemName = systemName, Id = deviceId });
      return result;
   }

   #endregion

   #endregion
}
