using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Represent a hub sensor record implementation
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class HubSensorRecordService(IRepository<Device> deviceRepository,
   IRepository<Sensor> sensorRepository,
   IRepository<User> userRepository,
   IRepository<SensorRecord> sensorRecordRepository) : IHubSensorRecordService
{

   #region Ctor

   #endregion

   #region Methods

   #region Delete

   /// <summary>
   /// Delete a sensor record entities
   /// </summary>
   /// <param name="ids">Collection of the entity id</param>
   /// <returns>Async operation</returns>
   public virtual Task DeleteRecordByIdAsync(IEnumerable<long> ids)
   {
      throw new NotImplementedException();
   }

   /// <summary>
   /// Delete a sensor record entity
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>Async operation</returns>
   public virtual Task DeleteRecordsByIdAsync(long id)
   {
      throw new NotImplementedException();
   }

   #endregion

   #region Get

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
   public virtual async Task<IPagedList<SensorRecord>> GetAllPagedSensorDataAsync(long[] sensorIds = null, long[] deviceIds = null, long[] userIds = null,
      DateTime? eventFrom = null, DateTime? eventTo = null, DateTime? createdFrom = null, DateTime? createdTo = null, int pageIndex = 0, int pageSize = int.MaxValue,
      bool getOnlyTotalCount = false)
   {
      var records = await sensorRecordRepository.GetAllPagedAsync(_ =>
      {
         var query = sensorRecordRepository.Table.AsNoTracking();

         if (sensorIds != null && sensorIds.Any())
            query = from r in query
                    where sensorIds.Contains(r.SensorId)
                    select r;

         if (deviceIds != null && deviceIds.Any())
            query = from r in query
                    join s in sensorRepository.Table.AsNoTracking() on r.SensorId equals s.Id
                    where deviceIds.Contains(s.DeviceId)
                    select r;

         if (userIds != null && userIds.Any())
            query = from r in query
                    join s in sensorRepository.Table.AsNoTracking() on r.SensorId equals s.Id
                    join d in deviceRepository.Table.AsNoTracking() on s.DeviceId equals d.Id
                    join u in userRepository.Table.AsNoTracking() on d.OwnerId equals u.Id
                    where userIds.Contains(u.Id)
                    select r;

         if (eventFrom.HasValue)
         {
            var from = eventFrom.Value.Ticks;
            query = from r in query
                    where r.EventTimestamp >= @from
                    select r;
         }

         if (eventTo.HasValue)
         {
            var to = eventTo.Value.Ticks;
            query = from r in query
                    where r.EventTimestamp <= to
                    select r;
         }

         if (createdFrom.HasValue)
         {
            query = from r in query
                    where r.CreatedTimeOnUtc >= createdFrom
                    select r;
         }

         if (createdTo.HasValue)
         {
            query = from r in query
                    where r.CreatedTimeOnUtc <= createdTo
                    select r;
         }

         query = query.OrderBy(x => x.Timestamp);

         return query;

      }, pageIndex, pageSize, getOnlyTotalCount);

      return records;
   }

   #endregion

   #endregion
}
