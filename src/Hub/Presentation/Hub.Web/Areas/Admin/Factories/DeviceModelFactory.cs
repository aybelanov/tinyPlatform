using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Directory;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Models.Extensions;
using Hub.Web.Framework.TagHelpers.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents a device model factory interface implementation
/// </summary>
public class DeviceModelFactory : IDeviceModelFactory
{
   #region fields

   private readonly UserSettings _userSettings;
   private readonly DeviceSettings _deviceSettings;
   private readonly DateTimeSettings _dateTimeSettings;
   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IHubDeviceService _deviceService;
   private readonly IUserService _userService;
   private readonly IHubSensorRecordService _sensorDataService;
   private readonly IHubSensorService _sensorService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IGeoLookupService _geoLookupService;
   private readonly ILocalizationService _localizationService;
   private readonly ILogger _logger;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;
   private readonly ICommunicator _communicator;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DeviceModelFactory(UserSettings userSettings,
      DeviceSettings deviceSettings,
      DateTimeSettings dateTimeSettings,
      IBaseAdminModelFactory baseAdminModelFactory,
      IDeviceActivityService deviceActivityService,
      IHubDeviceService deviceService,
      IUserService userService,
      IHubSensorRecordService sensorDataService,
      IHubSensorService sensorService,
      IDateTimeHelper dateTimeHelper,
      IGeoLookupService geoLookupService,
      ILocalizationService localizationService,
      IWorkContext workContext,
      ICommunicator communicator,
      ILogger logger,
      IGenericAttributeService genericAttributeService)
   {
      _userSettings = userSettings;
      _deviceSettings = deviceSettings;
      _dateTimeSettings = dateTimeSettings;
      _baseAdminModelFactory = baseAdminModelFactory;
      _deviceActivityService = deviceActivityService;
      _deviceService = deviceService;
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _geoLookupService = geoLookupService;
      _localizationService = localizationService;
      _workContext = workContext;
      _communicator = communicator;
      _logger = logger;
      _genericAttributeService = genericAttributeService;
      _sensorDataService = sensorDataService;
      _sensorService = sensorService;
   }


   #endregion

   #region Utilities

   /// <summary>
   /// Prepare user of shared devices model
   /// </summary>
   /// <param name="searchModel">Device</param>
   /// <param name="user">Device</param>
   /// <returns>Dvice  search model</returns>
   protected DeviceUserSearchModel PrepareDeviceUserSearchModelAsync(DeviceUserSearchModel searchModel, Device device)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (device == null)
         throw new ArgumentNullException(nameof(device));

      searchModel.DeviceId = device.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }


   /// <summary>
   /// Prepare device activity log search model
   /// </summary>
   /// <param name="searchModel">Device activity log search model</param>
   /// <param name="user">Device</param>
   /// <returns>Dvice activity log search model</returns>
   protected DeviceActivityLogSearchModel PrepareActivityLogSearchModelAsync(DeviceActivityLogSearchModel searchModel, Device device)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (device == null)
         throw new ArgumentNullException(nameof(device));

      searchModel.DeviceId = device.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }


   /// <summary>
   /// Prepares the deice statistic model
   /// </summary>
   /// <param name="device">Device</param>
   /// <returns>Device statistic model</returns>
   protected async Task<DeviceStatisticsModel> PrepareDeviceStatisticsModelAsync(Device device)
   {
      var model = new DeviceStatisticsModel();
      var beenRecentlyLimit = DateTime.UtcNow.AddMinutes(-_deviceSettings.BeenRecentlyMinutes);
      var onlineDeviceIds = await _communicator.GetOnlineDeviceIdsAsync();

      // online status
      if (onlineDeviceIds.Contains(device.Id))
      {
         model.Status = "online";
         model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Online");
      }
      else
      {
         var lastActivityDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device))?.CreatedOnUtc;

         if (lastActivityDateUtc.HasValue && lastActivityDateUtc.Value >= beenRecentlyLimit)
         {
            model.Status = "beenrecently";
            model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.BeenRecently");
         }
         else if (lastActivityDateUtc.HasValue && lastActivityDateUtc.Value < beenRecentlyLimit)
         {
            model.Status = "offline";
            model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Offline");
         }
         else if (!lastActivityDateUtc.HasValue)
         {
            model.Status = "noactivities";
            model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.NoActivities");
         }
      }

      model.NumberOfSensors = (await _sensorService.GetAllPagedSensorsAsync(
         deviceIds: new[] { device.Id }, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

      model.NumberOfDataRecords = (await _sensorDataService.GetAllPagedSensorDataAsync(
         deviceIds: new[] { device.Id }, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

      var deviceUsers = await _userService.GetAllUsersAsync(deviceIds: new[] { device.Id }, pageIndex: 0, pageSize: int.MaxValue);

      model.NumberOfTotalDeviceUsers = deviceUsers.TotalCount + 1; // added the owner

      var onlineUserIds = await _communicator.GetOnlineUserIdsAsync();

      model.NumberOfOnlineDeviceUsers = onlineUserIds.Intersect(deviceUsers.Select(x => x.Id).Union(new[] { device.OwnerId })).Count();

      return model;
   }

   #endregion

   #region Method

   /// <summary>
   /// Prepare paged device activity log list model
   /// </summary>
   /// <param name="searchModel">Device activity log search model</param>
   /// <param name="device">Device</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device activity log list model
   /// </returns>
   public async Task<DeviceActivityLogListModel> PrepareDeviceActivityLogListModelAsync(DeviceActivityLogSearchModel searchModel, Device device)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (device == null)
         throw new ArgumentNullException(nameof(device));

      //get device activity log
      var activityLog = await _deviceActivityService.GetAllActivitiesAsync(deviceId: device.Id,
          pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //prepare list model
      var model = await new DeviceActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
      {
         return activityLog.SelectAwait(async logItem =>
         {
            //fill in model values from the entity
            var deviceActivityLogModel = logItem.ToModel<DeviceActivityLogModel>();

            //fill in additional values (not existing in the entity)
            deviceActivityLogModel.ActivityLogTypeName = (await _deviceActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;
            deviceActivityLogModel.IpAddress = _deviceSettings.StoreIpAddresses ? logItem.IpAddress : HubCommonDefaults.SpoofedIp;
            //convert dates to the user time
            deviceActivityLogModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

            return deviceActivityLogModel;
         });
      });

      return model;
   }


   /// <summary>
   /// Prepares paged device list model
   /// </summary>
   /// <param name="searchModel">Device search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device list model
   /// </returns>
   public async Task<DeviceListModel> PrepareDeviceListModelAsync(DeviceSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      User user = null;
      if (_userSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(searchModel.SearchUsername))
         user = await _userService.GetUserByUsernameAsync(searchModel.SearchUsername);
      else if (!string.IsNullOrWhiteSpace(searchModel.SearchUserEmail))
         user = await _userService.GetUserByEmailAsync(searchModel.SearchUserEmail);

      //get parameters to filter log
      var createdStartDateValue = searchModel.CreatedFrom == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
      var createdEndDateValue = searchModel.CreatedTo == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

      var updatededStartDateValue = searchModel.CreatedFrom == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.UpdatedFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
      var updatedEndDateValue = searchModel.CreatedTo == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.UpdatedTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);


      //get devices
      var devices = await _deviceService.GetAllPagedDevicesAsync(
         userIds: user is null ? null : new[] { user.Id },
         systemName: searchModel.SearchDeviceSystemName?.Trim(),
         name: searchModel.SearchDeviceName?.Trim(),
         ipAddress: searchModel.SearchIpAddress?.Trim(),
         createdOnFrom: createdStartDateValue,
         createdOnTo: createdEndDateValue,
         updatedOnFrom: updatededStartDateValue,
         updatedOnTo: updatedEndDateValue,
         isActive: searchModel.SearchDeviceActive.ToBoolean(),
         isEnabled: searchModel.SearchDeviceEnabled.ToBoolean(),
         pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
         includeDeleted: false);

      //prepare list model
      var model = await new DeviceListModel().PrepareToGridAsync(searchModel, devices, () =>
      {
         return devices.SelectAwait(async device =>
         {
            //fill in model values from the entity
            var deviceModel = device.ToModel<DeviceModel>();
            deviceModel.LastLoginDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device, "Device.Login"))?.CreatedOnUtc;
            deviceModel.LastActivityDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device))?.CreatedOnUtc;
            deviceModel.IPAddress = _deviceSettings.StoreIpAddresses ? device.LastIpAddress : string.Empty;

            var owner = await _userService.GetUserByIdAsync(device.OwnerId);
            deviceModel.OwnerName = _userSettings.UsernamesEnabled ? owner.Username : owner.Email;

            return deviceModel;
         });
      });

      return model;
   }


   /// <summary>
   /// Prepares device model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <param name="device">Device</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device model
   /// </returns>
   public async Task<DeviceModel> PrepareDeviceModelAsync(DeviceModel model, Device device)
   {
      if (device is not null)
      {
         //fill in model values from the entity
         model = device.ToModel<DeviceModel>();

         var lastActivityDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device))?.CreatedOnUtc;
         if (lastActivityDateUtc.HasValue)
            model.LastActivityDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(lastActivityDateUtc.Value, DateTimeKind.Utc);

         var lastLoginDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device, "Device.Login"))?.CreatedOnUtc;
         if (lastLoginDateUtc.HasValue)
            model.LastLoginDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(lastLoginDateUtc.Value, DateTimeKind.Utc);

         model.CannotLoginUntilDateUtc = device.CannotLoginUntilDateUtc.HasValue
            ? (await _dateTimeHelper.ConvertToUserTimeAsync(device.CannotLoginUntilDateUtc.Value, DateTimeKind.Utc))
            : null;

         model.CreatedOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(device.CreatedOnUtc, DateTimeKind.Utc);

         if (device.UpdatedOnUtc.HasValue)
            model.UpdatedOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(device.UpdatedOnUtc.Value, DateTimeKind.Utc);

         model.Name = await _localizationService.GetLocalizedAsync(device, device => device.Name);
         model.Description = await _localizationService.GetLocalizedAsync(device, device => device.Description);
         model.DeviceStatisticsModel = await PrepareDeviceStatisticsModelAsync(device);
         PrepareDeviceUserSearchModelAsync(model.DeviceUserSearchModel, device);
         PrepareActivityLogSearchModelAsync(model.DeviceActivityLogSearchModel, device);
      }
      else
      {
         model = new DeviceModel();
         // default value
         model.Enabled = true;
         model.IsActive = true;
         model.IsDeleted = false;
         model.DataSendingDelay = 1000;
         model.DataPacketSize = 1024;
         model.ClearDataDelay = 3600;
         model.DataflowReconnectDelay = 10000;
         model.CountDataRows = 1_000_000;
         model.OwnerId = (await _workContext.GetCurrentUserAsync()).Id;
      }

      return model;
   }

   /// <summary>
   /// Prepare paged device users list model
   /// </summary>
   /// <param name="searchModel">Device user search model</param>
   /// <param name="device">Device</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user list model
   /// </returns>
   public async Task<DeviceUserListModel> PrepareDeviceUserListModelAsync(DeviceUserSearchModel searchModel, Device device)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (device == null)
         throw new ArgumentNullException(nameof(device));

      var users = (await _userService.GetUsersByDeviceIdAsync(device.Id))
         .OrderByDescending(x => x.CreatedOnUtc).ThenByDescending(x => x.Id)
         .ToList().ToPagedList(searchModel);

      var model = await new DeviceUserListModel().PrepareToGridAsync(searchModel, users, () =>
      {
         return users.SelectAwait(async user =>
         {
            //fill in model values from the entity        
            var userModel = new UserModel();
            userModel.Id = user.Id;
            userModel.IsActive = user.IsActive;
            userModel.Email = user.Email;
            userModel.Username = user.Username;

            if (_userSettings.CompanyEnabled)
               userModel.Company = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CompanyAttribute);

            return userModel;
         });
      });

      return model;
   }


   /// <summary>
   /// Prepares device search model
   /// </summary>
   /// <param name="searchModel">Device search model</param>
   /// <param name="popup">For popup tables</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device search model
   /// </returns>
   public Task<DeviceSearchModel> PrepareDeviceSearchModelAsync(DeviceSearchModel searchModel, bool popup = false)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      if (popup)
         searchModel.SetPopupGridPageSize();
      else
         searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }


   /// <summary>
   /// Prepare online user search model
   /// </summary>
   /// <param name="searchModel">Online user search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online user search model
   /// </returns>
   public virtual async Task<OnlineDeviceSearchModel> PrepareOnlineDeviceSearchModelAsync(OnlineDeviceSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      searchModel.SearchOnline = true;
      searchModel.SearchBeenRecently = true;
      searchModel.SearchOffline = false;
      searchModel.SearchNoActivities = false;

      searchModel.SetGridPageSize();

      return await Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged online user list model
   /// </summary>
   /// <param name="searchModel">Online user search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online user list model
   /// </returns>
   public virtual async Task<OnlineDeviceListModel> PrepareOnlineDeviceListModelAsync(OnlineDeviceSearchModel searchModel)
   {
      ArgumentNullException.ThrowIfNull(searchModel);

      var now = DateTime.UtcNow;

      var activityStart = searchModel.SearchLastActivityFrom.HasValue
         ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
         : null;

      var activityEnd = searchModel.SearchLastActivityTo.HasValue
         ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
         : null;

      var onlineDevicesIds = await _communicator.GetOnlineDeviceIdsAsync();

      var devices = await _deviceService.GetOnlineDevicesAsync(
         utcNow: now,
         systemName: searchModel.SearchSystemName,
         onlineIds: onlineDevicesIds.ToArray(),
         online: searchModel.SearchOnline,
         beenRecently: searchModel.SearchBeenRecently,
         offline: searchModel.SearchOffline,
         noActivities: searchModel.SearchNoActivities,
         email: searchModel.SearchUserEmail,
         ipAddress: searchModel.SearchIpAddress,
         company: searchModel.SearchUserCompany,
         lastActivityFromUtc: activityStart,
         lastActivityToUtc: activityEnd);

      var beenRecentlyLimit = now.AddMinutes(-_deviceSettings.BeenRecentlyMinutes);

      //prepare list model
      var model = await new OnlineDeviceListModel().PrepareToGridAsync(searchModel, devices, () =>
      {
         return devices.SelectAwait(async device =>
         {
            //fill in model values from the entity
            var deviceModel = new OnlineDeviceModel();

            deviceModel.Id = device.Id;

            //fill in additional values (not existing in the entity)
            deviceModel.DeviceInfo = device.SystemName;

            var lastActivity = await _deviceActivityService.GetLastActivityRecordAsync(device);

            deviceModel.LastIpAddress = _deviceSettings.StoreIpAddresses
                     ? lastActivity.IpAddress
                     : await _localizationService.GetResourceAsync("Admin.Devices.Online.Fields.IPAddress.Disabled");

            deviceModel.Location = _deviceSettings.StoreIpAddresses && string.IsNullOrEmpty(lastActivity.IpAddress)
                     ? _geoLookupService.LookupCountryName(deviceModel.LastIpAddress)
                     : string.Empty;

            if (onlineDevicesIds.Contains(deviceModel.Id))
            {
               deviceModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Online");
            }
            else
            {
               var lastActivityDateUtc = lastActivity?.CreatedOnUtc;
               if (lastActivityDateUtc.HasValue)
               {
                  deviceModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(lastActivityDateUtc.Value, DateTimeKind.Utc);

                  if (lastActivityDateUtc.HasValue && lastActivityDateUtc.Value >= beenRecentlyLimit)
                     deviceModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.BeenRecently");

                  else if (lastActivityDateUtc.HasValue && lastActivityDateUtc.Value < beenRecentlyLimit)
                     deviceModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Offline");
               }
               else
               {
                  deviceModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.NoActivities");
               }
            }

            return deviceModel;
         });
      });

      return model;
   }

   #endregion
}
