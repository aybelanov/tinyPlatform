using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Logging;
using Hub.Data;
using Hub.Services.Clients;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using Shared.Common;

namespace Hub.Services.Logging;

/// <summary>
/// Represents a device activity service interface implementation
/// </summary>
public class DeviceActivityService : BaseActivityService, IDeviceActivityService
{
   #region Fields

   private readonly IRepository<ActivityLog> _activityLogRepository;
   private readonly IWebHelper _webHelper;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DeviceActivityService(IRepository<ActivityLog> activityLogRepository,
       IRepository<ActivityLogType> activityLogTypeRepository,
       IWebHelper webHelper,
       IWorkContext workContext)
      : base(activityLogRepository, activityLogTypeRepository, webHelper)
   {
      _activityLogRepository = activityLogRepository;
      _webHelper = webHelper;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets all activity log type items
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log type items
   /// </returns>
   public override async Task<IList<ActivityLogType>> GetAllActivityTypesAsync()
   {
      var activityLogTypes = (await base.GetAllActivityTypesAsync()).Where(x => x.SystemKeyword.StartsWith("Device."));

      return activityLogTypes.ToList();
   }

   /// <summary>
   /// Get last activity record for device
   /// </summary>
   /// <param name="device">Device entity</param>
   /// <param name="systemKeyword">Activity log type system keword</param>
   /// <returns>Activity log record</returns>
   public Task<ActivityLog> GetLastActivityRecordAsync(Device device, string systemKeyword = null)
   {
      return base.GetLastActivityRecordAsync(device, systemKeyword);
   }


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
   public virtual async Task<ActivityLog> InsertActivityAsync(Device device, string systemKeyword, string comment, BaseEntity entity = null)
   {
      if (device == null)
         return null;

      //try to get activity log type by passed system keyword
      var activityLogType = (await GetAllActivityTypesAsync()).FirstOrDefault(type => type.SystemKeyword.Equals(systemKeyword));
      if (!activityLogType?.Enabled ?? true)
         return null;

      //insert log item
      var logItem = new ActivityLog
      {
         ActivityLogTypeId = activityLogType.Id,
         EntityId = entity?.Id,
         EntityName = entity?.GetType().Name,
         SubjectId = device.Id,
         SubjectName = typeof(Device).Name,
         Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
         CreatedOnUtc = DateTime.UtcNow,
         IpAddress = _webHelper.GetCurrentIpAddress()
      };
      await _activityLogRepository.InsertAsync(logItem);

      return logItem;
   }


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
   public virtual async Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(
       DateTime? createdOnFrom = null,
       DateTime? createdOnTo = null,
       long? deviceId = null,
       long? activityLogTypeId = null,
       string ipAddress = null,
       string entityName = null,
       long? entityId = null,
       int pageIndex = 0,
       int pageSize = int.MaxValue)
   {
      return await base.GetAllActivitiesAsync(
         createdOnFrom,
         createdOnTo,
         deviceId,
         typeof(Device).Name,
         activityLogTypeId,
         ipAddress,
         entityName,
         entityId,
         pageIndex,
         pageSize);
   }


   /// <summary>
   /// Clears device activity log
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public override async Task ClearAllActivitiesAsync()
   {
      await base.ClearAllActivitiesAsync<Device>();
   }

   /// <summary>
   /// Gets device activities by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>FIlterable collection of the activity records</returns>
   public async Task<IFilterableList<ActivityLog>> GetActivitiesByDynamicFilterAsync(DynamicFilter filter)
   {
      var result = await GetActivitiesByDynamicFilterAsync(nameof(Device), filter);
      return result;
   }
   #endregion
}
