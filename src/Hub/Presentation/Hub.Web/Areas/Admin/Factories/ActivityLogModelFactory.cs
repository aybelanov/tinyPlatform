using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Models.Logging;
using Hub.Services.Users;
using Hub.Services.Helpers;
using Hub.Services.Logging;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Framework.Models.Extensions;
using Hub.Data.Extensions;
using Hub.Services.Devices;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Services.Common;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the activity log model factory implementation
/// </summary>
public partial class ActivityLogModelFactory : IActivityLogModelFactory
{
   #region Fields

   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly IUserActivityService _userActivityService;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IUserService _userService;
   private readonly IHubDeviceService _deviceService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly UserSettings _userSettings;
   private readonly DeviceSettings _deviceSettings;   

   #endregion

   #region Ctor

   public ActivityLogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
       IUserActivityService userActivityService,
       IDeviceActivityService deviceActivityService,
       IUserService userService,
       IHubDeviceService deviceService,
       IDateTimeHelper dateTimeHelper,
       DeviceSettings deviceSettings,
       UserSettings userSettings)
   {
      _baseAdminModelFactory = baseAdminModelFactory;
      _userActivityService = userActivityService;
      _deviceActivityService = deviceActivityService;
      _userService = userService;
      _deviceService = deviceService;
      _dateTimeHelper = dateTimeHelper;
      _deviceSettings = deviceSettings;
      _userSettings = userSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare activity log type models
   /// </summary>
   /// <param name="subject">Client type (user or device)</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of activity log type models
   /// </returns>
   protected virtual async Task<IList<ActivityLogTypeModel>> PrepareActivityLogTypeModelsAsync(string subject)
   {
      //prepare available activity log types
      var availableActivityTypes = subject == typeof(Device).Name
         ? await _deviceActivityService.GetAllActivityTypesAsync()
         : await _userActivityService.GetAllActivityTypesAsync();

      var models = availableActivityTypes.Select(activityType => activityType.ToModel<ActivityLogTypeModel>()).ToList();

      return models;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare activity log types search model
   /// </summary>
   /// <param name="searchModel">Activity log types search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log types search model
   /// </returns>
   public virtual async Task<ActivityLogTypeSearchModel> PrepareActivityLogTypeSearchModelAsync(ActivityLogTypeSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      searchModel.ActivityLogTypeListModel = await PrepareActivityLogTypeModelsAsync(searchModel.Subject);

      //prepare grid
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare activity log search model
   /// </summary>
   /// <param name="searchModel">Activity log search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log search model
   /// </returns>
   public virtual async Task<ActivityLogSearchModel> PrepareActivityLogSearchModelAsync(ActivityLogSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare available activity log types
      await _baseAdminModelFactory.PrepareActivityLogTypesAsync(searchModel.ActivityLogType, toDevice: searchModel.SubjectType == typeof(Device).Name);

      //prepare grid
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged activity log list model
   /// </summary>
   /// <param name="searchModel">Activity log search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log list model
   /// </returns>
   public virtual async Task<ActivityLogListModel> PrepareActivityLogListModelAsync(ActivityLogSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get parameters to filter log
      var startDateValue = searchModel.CreatedOnFrom == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
      var endDateValue = searchModel.CreatedOnTo == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

      if (searchModel.SubjectType == typeof(Device).Name)
      {
         //get log for devices
         var activityLog = (await _deviceActivityService.GetAllActivitiesAsync(createdOnFrom: startDateValue,
             createdOnTo: endDateValue,
             activityLogTypeId: searchModel.ActivityLogTypeId,
             ipAddress: searchModel.IpAddress,
             pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize));

         if (activityLog is null)
            return new ActivityLogListModel();

         //prepare list model
         var deviceIds = activityLog.GroupBy(logItem => logItem.SubjectId).Select(logItem => logItem.Key);
         var activityLogDevices = await _deviceService.GetDevicesByIdsAsync(deviceIds.ToArray());

         var model = await new ActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
         {
            return activityLog.SelectAwait(async logItem =>
            {
               //fill in model values from the entity
               var logItemModel = logItem.ToModel<ActivityLogModel>();
               logItemModel.ActivityLogTypeName = (await _userActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;
               logItemModel.Subject = activityLogDevices?.FirstOrDefault(x => x.Id == logItem.SubjectId)?.SystemName;
               logItemModel.SubjectId = logItem.SubjectId;
               logItemModel.IpAddress = _deviceSettings.StoreIpAddresses ? logItem.IpAddress : HubCommonDefaults.SpoofedIp;

               //convert dates to the user time
               logItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

               return logItemModel;
            });
         });

         return model;
      }
      else
      {
         //get log for users and clients
         var activityLog = await _userActivityService.GetAllActivitiesAsync(createdOnFrom: startDateValue,
             createdOnTo: endDateValue,
             activityLogTypeId: searchModel.ActivityLogTypeId,
             ipAddress: searchModel.IpAddress,
             pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

         if (activityLog is null)
            return new ActivityLogListModel();

         //prepare list model
         var userIds = activityLog.GroupBy(logItem => logItem.SubjectId).Select(logItem => logItem.Key);
         var activityLogUsers = await _userService.GetUsersByIdsAsync(userIds.ToArray());

         var model = await new ActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
         {
            return activityLog.SelectAwait(async logItem =>
               {
                  //fill in model values from the entity
                  var logItemModel = logItem.ToModel<ActivityLogModel>();
                  logItemModel.ActivityLogTypeName = (await _userActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;
                  logItemModel.IpAddress = _userSettings.StoreIpAddresses ? logItem.IpAddress : HubCommonDefaults.SpoofedIp;

                  logItemModel.Subject = activityLogUsers?.FirstOrDefault(x => x.Id == logItem.SubjectId)?.Email;
                  logItemModel.SubjectId = logItem.SubjectId;

                  //convert dates to the user time
                  logItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                  return logItemModel;
               });
         });

         return model;
      }
   }

   #endregion
}