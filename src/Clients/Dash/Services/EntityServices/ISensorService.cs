using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Sensors;
using Shared.Clients;
using Shared.Clients.Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Sensor service interface
/// </summary>
public interface ISensorService
{
   /// <summary>
   /// Delete a sensor by the model
   /// </summary>
   /// <param name="model">Sensor model</param>
   /// <returns></returns>
   Task DeleteAsync(SensorModel model);

   /// <summary>
   /// Get all sensors
   /// </summary>
   /// <returns>Sensor entity collection</returns>
   Task<IFilterableList<Sensor>> GetSensorsAsync(DynamicFilter filter);

   /// <summary>
   /// Get sensor select item list
   /// </summary>
   /// <returns>Sensor select item collection</returns>
   Task<IFilterableList<SensorSelectItem>> GetSensorSelectItemListAsync(DynamicFilter filter);

   /// <summary>
   /// Get sensor select item list for all devices (admin mode)
   /// </summary>
   /// <returns>Sensor select item collection</returns>
   Task<IFilterableList<SensorSelectItem>> GetForAllSensorSelectItemListAsync(DynamicFilter filter);

   /// <summary>
   /// Get a sensor by identifier
   /// </summary>
   /// <param name="id">Entity inedtifier</param>
   /// <returns>Sensor entity</returns>
   Task<Sensor> GetSensorByIdAsync(long id);

   /// <summary>
   /// Get sensors by identifiers
   /// </summary>
   /// <param name="ids">Entity inedtifiers</param>
   /// <returns>Sensor entity collection</returns>
   Task<IFilterableList<Sensor>> GetSensorByIdsAsync(IEnumerable<long> ids);

   /// <summary>
   /// Insert a sensor from the model
   /// </summary>
   /// <param name="model">Moitor model</param>
   /// <returns></returns>
   Task InsertAsync(SensorModel model);

   /// <summary>
   /// Update a sensor from the model
   /// </summary>
   /// <param name="model">Sensor model</param>
   /// <returns></returns>
   Task UpdateAsync(SensorModel model);

   /// <summary>
   /// Checks sensor sytemname availability
   /// </summary>
   /// <param name="systemName">Checking systemname</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Response with the result: success or error coolecyion</returns>
   Task<CommonResponse> CheckSystemNameAvailabilityAsync(string systemName, long deviceId);
}