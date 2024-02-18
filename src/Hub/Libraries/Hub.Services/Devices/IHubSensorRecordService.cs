using DocumentFormat.OpenXml.Spreadsheet;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Represents a sensor data service interface
/// </summary>
public interface IHubSensorRecordService
{
   /// <summary>
   /// Delete a sensor record entities
   /// </summary>
   /// <param name="ids">Collection of the entity id</param>
   /// <returns>Async operation</returns>
   Task DeleteRecordByIdAsync(IEnumerable<long> ids);

   /// <summary>
   /// Delete a sensor record entity
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>Async operation</returns>
   Task DeleteRecordsByIdAsync(long id);


   /// <summary>
   /// Get all sensor data records
   /// </summary>
   /// <param name="sensorIds">Sensor identifier array</param>
   /// <param name="deviceIds">Device identifier array</param>
   /// <param name="userIds">User (only owner) identifier</param>
   /// <param name="eventFrom">Date "from"</param>
   /// <param name="eventTo">Date "to"</param>
   /// <param name="createdFrom">Event date "from"</param>
   /// <param name="createdTo">Created date "to"</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records.
   /// GetTable to "true" if you don't want to load data from database</param>
   /// <returns>Paged sensor data record collection</returns>
   Task<IPagedList<SensorRecord>> GetAllPagedSensorDataAsync(long[] sensorIds = null, long[] deviceIds = null, long[] userIds = null,
      DateTime? eventFrom = null, DateTime? eventTo = null, DateTime? createdFrom = null, DateTime? createdTo = null,
      int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
}
