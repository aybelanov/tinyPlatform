using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Clients;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Devices;
#pragma warning disable CS1591

public class HubSensorService : IHubSensorService
{
   #region Fields

   private readonly IRepository<Device> _deviceRepository;
   private readonly IRepository<Sensor> _sensorRepository;
   private readonly IRepository<SensorWidget> _sensorWidgetRepository;
   private readonly IRepository<Presentation> _presentationRepository;
   private readonly IRepository<User> _userRepository;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctor

   public HubSensorService(IRepository<Device> deviceRepository,
       IRepository<Sensor> sensorRepository,
       IRepository<SensorWidget> sensorWidgetRepository,
       IRepository<Presentation> monitorSensorWidgetRepository,
       IStaticCacheManager staticCacheManager,
       IRepository<User> userRepository)
   {
      _deviceRepository = deviceRepository;
      _sensorRepository = sensorRepository;
      _sensorWidgetRepository = sensorWidgetRepository;
      _presentationRepository = monitorSensorWidgetRepository;
      _staticCacheManager = staticCacheManager;
      _userRepository = userRepository;
   }

   #endregion

   #region Methods

   #region Delete

   /// <summary>
   /// Delete entity 
   /// </summary>
   /// <param name="sensor">Entity</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteAsync(Sensor sensor)
   {
      ArgumentNullException.ThrowIfNull(sensor);

      var sensorWidgets = _sensorWidgetRepository.Table.AsNoTracking().Where(x=>x.SensorId == sensor.Id);   

      var presentations =
         from p in _presentationRepository.Table.AsNoTracking()
         join sw in sensorWidgets on p.SensorWidgetId equals sw.Id
         select p;

      await presentations.ExecuteDeleteAsync();
      await sensorWidgets.ExecuteDeleteAsync();

      // soft delete
      await _sensorRepository.DeleteAsync(sensor);
   }

   /// <summary>
   /// Delete entities
   /// </summary>
   /// <param name="sensors">Entities</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteAsync(IList<Sensor> sensors)
   {
      ArgumentNullException.ThrowIfNull(sensors);

      await _sensorRepository.DeleteAsync(sensors);
   }

   #endregion

   #region Get


   /// <summary>
   /// Get all sensors
   /// </summary>
   /// <param name="ids">Sensor identifier array</param>
   /// <param name="ownerIds">User identifier</param>
   /// <param name="deviceIds">Device identifier array</param>
   /// <param name="systemName">Sensor system name</param>
   /// <param name="name">sensor name</param>
   /// <param name="isEnabled">Is sensor enabled</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records.</param>
   /// <param name="includeDeleted">Include deleted item</param>
   /// <returns>Paged list of the sensors</returns>
   public virtual async Task<IPagedList<Sensor>> GetAllPagedSensorsAsync(long[] ids = null, long[] ownerIds = null, long[] deviceIds = null, string systemName = null,
      string name = null, bool? isEnabled = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false,
      bool includeDeleted = false)
   {
      var sensors = await _sensorRepository.GetAllPagedAsync(query =>
      {
         if (ids != null && ids.Any())
            query = query.Where(x => ids.Contains(x.Id));

         if (ownerIds != null && ownerIds.Any())
            query = from s in query
                    join d in _deviceRepository.Table on s.DeviceId equals d.Id
                    join u in _userRepository.Table on d.OwnerId equals u.Id
                    where ownerIds.Contains(u.Id)
                    select s;

         if (deviceIds != null && deviceIds.Any())
            query = from s in query
                    join d in _deviceRepository.Table on s.DeviceId equals d.Id
                    where deviceIds.Contains(d.Id)
                    select s;


         if (!string.IsNullOrWhiteSpace(systemName))
            query = query.Where(x => x.SystemName == systemName);

         if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name == name);

         if (isEnabled.HasValue)
            query = query.Where(x => x.Enabled == isEnabled.Value);

         query = query.OrderBy(x => x.Id);

         return query;

      }, pageIndex, pageSize, getOnlyTotalCount);

      return sensors;
   }

   /// <summary>
   /// Get sensor entity by its identifier
   /// </summary>
   /// <param name="sensorId">Sensor entity identifier</param>
   /// <returns>Sensor entity  (async operation)</returns>
   public virtual async Task<Sensor> GetSensorByIdAsync(long sensorId)
   {
      if (sensorId < 1)
         throw new ArgumentOutOfRangeException(nameof(sensorId));

      return await _sensorRepository.GetByIdAsync(sensorId,
         cache => cache.PrepareKeyForDefaultCache(AppEntityCacheDefaults<Sensor>.ByIdCacheKey, sensorId));
   }

   /// <summary>
   /// Get sensor entity by its system name
   /// </summary>
   /// <param name="systemName">Sensor entity identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Sensor entity  (async operation)</returns>
   public async Task<Sensor> GetSensorBySystemNameAsync(string systemName, long deviceId)
   {
      if (string.IsNullOrEmpty(systemName))
         return null;

      var query = from s in _sensorRepository.Table
                  where s.SystemName == systemName && s.DeviceId == deviceId 
                  select s;

      // we don't use common GetAll cache becase this method uses by incomming device methods
      var key = _staticCacheManager.PrepareKeyForShortTermCache(ClientCacheDefaults<Sensor>.BySensorSystemNameCacheKey, systemName, deviceId);

      return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }


   /// <summary>
   /// Get sensor entites by its id for specified user
   /// </summary>
   /// <param name="ids">Collection of sensor ids id</param>
   /// <returns>The collection of sensor entites (async operation)</returns>
   public virtual async Task<IList<Sensor>> GetSensorsByIdsAsync(IList<long> ids)
   {
      ArgumentNullException.ThrowIfNull(ids);

      return await _sensorRepository.GetByIdsAsync(ids);
   }


   /// <summary>
   /// Gets device sensors by the device identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Sensor collection</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public virtual async Task<IList<Sensor>> GetSensorsByDeviceIdAsync(long deviceId)
   {
      if (deviceId < 1)
         throw new ArgumentOutOfRangeException(nameof(deviceId));

      var query = from s in _sensorRepository.Table
                  where s.DeviceId == deviceId
                  select s;

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ClientCacheDefaults<Sensor>.ByDeviceIdCacheKey, deviceId);

      return await _staticCacheManager.GetAsync(cacheKey, async () => await query.ToListAsync());
   }

   #endregion

   #region Insert

   /// <summary>
   /// Insert collection of the sensor entities to DB
   /// </summary>
   /// <param name="entities"></param>
   /// <returns>Async operation</returns>
   public virtual async Task InsertAsync(IEnumerable<Sensor> entities)
   {
      await _sensorRepository.InsertAsync(entities.ToList());
   }

   /// <summary>
   /// Insert a sensor entity to DB
   /// </summary>
   /// <param name="entity">A sensor entity</param>
   /// <returns>Async operation</returns>
   public virtual async Task InsertAsync(Sensor entity)
   {
      await _sensorRepository.InsertAsync(entity);
   }

   #endregion

   #region Update

   /// <summary>
   /// Update collection of the sensor entity
   /// </summary>
   /// <param name="entities">A sensor entity collection</param>
   /// <returns>Async operation</returns>
   public virtual async Task UpdateAsync(IEnumerable<Sensor> entities)
   {
      await _sensorRepository.UpdateAsync(entities.ToList());
   }

   /// <summary>
   /// Update the sensor entity
   /// </summary>
   /// <param name="entity">A sensor entity</param>
   /// <returns>Async operation</returns>
   public virtual async Task UpdateAsync(Sensor entity)
   {
      await _sensorRepository.UpdateAsync(entity);
   }

   #endregion

   #endregion
}
#pragma warning restore CS1591