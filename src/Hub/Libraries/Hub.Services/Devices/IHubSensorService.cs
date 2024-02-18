using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Represents a sensor service interface
/// </summary>
public interface IHubSensorService
{
   /// <summary>
   /// Delete entity 
   /// </summary>
   /// <param name="sensor">Entity</param>
   /// <returns>Async operation</returns>
   Task DeleteAsync(Sensor sensor);

   /// <summary>
   /// Delete entities
   /// </summary>
   /// <param name="sensors">Entities</param>
   /// <returns>Async operation</returns>
   Task DeleteAsync(IList<Sensor> sensors);

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
   Task<IPagedList<Sensor>> GetAllPagedSensorsAsync(long[] ids = null, long[] ownerIds = null, long[] deviceIds = null, string systemName = null,
      string name = null, bool? isEnabled = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false,
      bool includeDeleted = false);

   /// <summary>
   /// Get sensor entity by its identifier
   /// </summary>
   /// <param name="sensorId">Sensor entity identifier</param>
   /// <returns>Sensor entity  (async operation)</returns>
   Task<Sensor> GetSensorByIdAsync(long sensorId);

   /// <summary>
   /// Get sensor entity by its system name
   /// </summary>
   /// <param name="systemName">Sensor entity identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Sensor entity  (async operation)</returns>
   Task<Sensor> GetSensorBySystemNameAsync(string systemName, long deviceId);

   /// <summary>
   /// Get sensor entites by its id for specified user
   /// </summary>
   /// <param name="ids">Collection of sensor ids id</param>
   /// <returns>The collection of sensor entites (async operation)</returns>
   Task<IList<Sensor>> GetSensorsByIdsAsync(IList<long> ids);

   /// <summary>
   /// Gets device sensors by the device identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Sensor collection</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   Task<IList<Sensor>> GetSensorsByDeviceIdAsync(long deviceId);

   /// <summary>
   /// Insert collection of the sensor entities to DB
   /// </summary>
   /// <param name="entities"></param>
   /// <returns>Async operation</returns>
   Task InsertAsync(IEnumerable<Sensor> entities);

   /// <summary>
   /// Insert a sensor entity to DB
   /// </summary>
   /// <param name="entity">A sensor entity</param>
   /// <returns>Async operation</returns>
   Task InsertAsync(Sensor entity);

   /// <summary>
   /// Update collection of the sensor entity
   /// </summary>
   /// <param name="entities">A sensor entity collection</param>
   /// <returns>Async operation</returns>
   Task UpdateAsync(IEnumerable<Sensor> entities);

   /// <summary>
   /// Update the sensor entity
   /// </summary>
   /// <param name="entity">A sensor entity</param>
   /// <returns>Async operation</returns>
   Task UpdateAsync(Sensor entity);
}
