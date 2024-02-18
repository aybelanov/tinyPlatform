using Hub.Core.Domain.Clients;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Sensors;

/// <summary>
/// Represents a dashboard client sensor service interface 
/// </summary>
public interface ISensorService
{
    /// <summary>
    /// Deletes a sensor
    /// </summary>
    /// <param name="devices">Sensor entity</param>
    /// <returns></returns>
    Task DeleteAsync(Sensor devices);

    /// <summary>
    /// Gets sensor select items by the dynamic filter
    /// </summary>
    /// <param name="filter">Dynamic filter</param>
    /// <returns>Filterable sensor select item list connection</returns>
    Task<IFilterableList<SensorSelectItem>> GetSensorSelectItemListAsync(DynamicFilter filter);

    /// <summary>
    /// Gets sensors for common log
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Sensor identifier collection</returns>
    Task<IList<long>> GetCommonLogSensorIdsAsync(long userId);

    /// <summary>
    /// Get sensor entites by its id for specified user
    /// </summary>
    /// <param name="ids">Collection of sensor ids id</param>
    /// <returns>The collection of sensor entites (async operation)</returns>
    Task<IList<Sensor>> GetSensorsByIdsAsync(IList<long> ids);

    /// <summary>
    /// Gets sensors by the dynamic filter
    /// </summary>
    /// <param name="filter">Dynamic filter</param>
    /// <returns>Filterable collection</returns>
    Task<IFilterableList<Sensor>> GetSensorsAsync(DynamicFilter filter);

    /// <summary>
    /// Update a sensor entity
    /// </summary>
    /// <param name="sensor">Sensor entity</param>
    /// <returns></returns>
    Task UpdateAsync(Sensor sensor);

    /// <summary>
    /// Inserts sensor to database
    /// </summary>
    /// <param name="sensor">Adding sensor entity</param>
    /// <returns>Device identifier</returns>
    Task<long> InsertAsync(Sensor sensor);

    /// <summary>
    /// Is a sensor in a user scope
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="sensorId">Sensor identifier</param>
    /// <returns>Result: true - in scope; false - not in scope.</returns>
    Task<bool> IsInUserScopeAsync(long userId, long sensorId);

   /// <summary>
   /// User scope for sensors
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>User scope query</returns>
   IQueryable<Sensor> UserScope(long? userId);
}
