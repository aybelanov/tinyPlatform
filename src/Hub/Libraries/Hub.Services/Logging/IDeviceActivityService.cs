﻿using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Logging;
using Shared.Clients;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Logging;

/// <summary>
/// Represents a device activity service interface
/// </summary>
public interface IDeviceActivityService
{
   /// <summary>
   /// Updates an activity log type item
   /// </summary>
   /// <param name="activityLogType">Activity log type item</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateActivityTypeAsync(ActivityLogType activityLogType);

   /// <summary>
   /// Gets all activity log type items
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log type items
   /// </returns>
   Task<IList<ActivityLogType>> GetAllActivityTypesAsync();

   /// <summary>
   /// Gets an activity log type item
   /// </summary>
   /// <param name="activityLogTypeId">Activity log type identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log type item
   /// </returns>
   Task<ActivityLogType> GetActivityTypeByIdAsync(long activityLogTypeId);

   /// <summary>
   /// Inserts an activity log item
   /// </summary>
   /// <param name="device">Device</param>
   /// <param name="systemKeyword">System keyword</param>
   /// <param name="comment">Comment</param>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log item
   /// </returns>
   Task<ActivityLog> InsertActivityAsync(Device device, string systemKeyword, string comment, BaseEntity entity = null);

   /// <summary>
   /// Deletes an activity log item
   /// </summary>
   /// <param name="activityLog">Activity log</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteActivityAsync(ActivityLog activityLog);

   /// <summary>
   /// Gets all activity log items
   /// </summary>
   /// <param name="createdOnFrom">Log item creation from; pass null to load all records</param>
   /// <param name="createdOnTo">Log item creation to; pass null to load all records</param>
   /// <param name="deviceId">Device identifier; pass null to load all records</param>
   /// <param name="activityLogTypeId">Activity log type identifier; pass null to load all records</param>
   /// <param name="ipAddress">IP address; pass null or empty to load all records</param>
   /// <param name="entityName">Entity name; pass null to load all records</param>
   /// <param name="entityId">Entity identifier; pass null to load all records</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log items
   /// </returns>
   Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
       long? deviceId = null, long? activityLogTypeId = null, string ipAddress = null, string entityName = null, long? entityId = null,
       int pageIndex = 0, int pageSize = int.MaxValue);

   /// <summary>
   /// Get last activity record for device
   /// </summary>
   /// <param name="device">Device entity</param>
   /// <param name="systemKeyword">Activity log type system keword</param>
   /// <returns>Activity log record</returns>
   Task<ActivityLog> GetLastActivityRecordAsync(Device device, string systemKeyword = null);

   /// <summary>
   /// Gets an activity log item
   /// </summary>
   /// <param name="activityLogId">Activity log identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log item
   /// </returns>
   Task<ActivityLog> GetActivityByIdAsync(long activityLogId);

   /// <summary>
   /// Clears activity log
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task ClearAllActivitiesAsync();

   /// <summary>
   /// Gets device activities by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>FIlterable collection of the activity records</returns>
   Task<IFilterableList<ActivityLog>> GetActivitiesByDynamicFilterAsync(DynamicFilter filter);
}

