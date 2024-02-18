using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Data.Extensions;
using Hub.Services.Clients;
using Hub.Services.Clients.Records;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.ExportImport;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Hub.Web.Framework.TagHelpers.Shared;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Clients;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class DeviceController : BaseAdminController
{
   #region fields

   private readonly DeviceSettings _deviceSettings;
   private readonly UserSettings _userSettings;
   private readonly DateTimeSettings _dateTimeSettings;
   private readonly EmailAccountSettings _emailAccountSettings;
   private readonly IUserActivityService _userActivityService;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IDeviceModelFactory _deviceModelFactory;
   private readonly IUserModelFactory _userModelFactory;
   private readonly IDeviceRegistrationService _deviceRegistrationService;
   private readonly IHubDeviceService _deviceService;
   private readonly ISensorRecordService _sensorDataService;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IEmailAccountService _emailAccountService;
   private readonly IEventPublisher _eventPublisher;
   private readonly IExportManager _exportManager;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly ILocalizationService _localizationService;
   private readonly ILocalizedEntityService _localizedEntityService;
   private readonly ILanguageService _languageService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IQueuedEmailService _queuedEmailService;
   private readonly IWorkContext _workContext;
   private readonly IWorkflowMessageService _workflowMessageService;

   #endregion

   #region Ctor
   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DeviceController(DeviceSettings deviceSettings,
      UserSettings userSettings,
      DateTimeSettings dateTimeSettings,
      EmailAccountSettings emailAccountSettings,
      IUserActivityService userActivityService,
      IDeviceActivityService deviceActivityService,
      IDeviceModelFactory deviceModelFactory,
      IUserModelFactory userModelFactory,
      IDeviceRegistrationService deviceRegistrationService,
      IHubDeviceService deviceService,
      ISensorRecordService sensorDataService,
      IUserService userService,
      IDateTimeHelper dateTimeHelper,
      IEmailAccountService emailAccountService,
      IEventPublisher eventPublisher,
      IExportManager exportManager,
      IGenericAttributeService genericAttributeService,
      ILanguageService languageService,
      ILocalizationService localizationService,
      ILocalizedEntityService localizedEntityService,
      INotificationService notificationService,
      IPermissionService permissionService,
      IQueuedEmailService queuedEmailService,
      IWorkContext workContext,
      IWorkflowMessageService workflowMessageService)
   {
      _deviceSettings = deviceSettings;
      _userSettings = userSettings;
      _dateTimeSettings = dateTimeSettings;
      _emailAccountSettings = emailAccountSettings;
      _userActivityService = userActivityService;
      _deviceActivityService = deviceActivityService;
      _deviceModelFactory = deviceModelFactory;
      _userModelFactory = userModelFactory;
      _deviceRegistrationService = deviceRegistrationService;
      _deviceService = deviceService;
      _sensorDataService = sensorDataService;   
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _emailAccountService = emailAccountService;
      _eventPublisher = eventPublisher;
      _exportManager = exportManager;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _localizedEntityService = localizedEntityService;
      _languageService = languageService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _queuedEmailService = queuedEmailService;
      _workContext = workContext;
      _workflowMessageService = workflowMessageService;
   }
   #endregion

   #region Methods

   #region Set device owner


   [HttpPost]
   public virtual async Task<IActionResult> LoadOwnerName(long ownerId)
   {
      string result = string.Empty;
      if (ownerId > 0)
      {
         var user = await _userService.GetUserByIdAsync(ownerId);
         result = _userSettings.UsernamesEnabled ? user.Username : user.Email;
      }

      return Json(new { Text = result });
   }


   public virtual async Task<IActionResult> OwnerAddPopup()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //prepare model
      var model = await _userModelFactory.PrepareUserSearchModelAsync(new UserSearchModel(), true);

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> OwnerAddPopupList(UserSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _userModelFactory.PrepareUserListModelAsync(searchModel);

      return Json(model);
   }

   #endregion

   #region Set shared users

   public virtual async Task<IActionResult> UsersAddPopup(long deviceId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //prepare model
      var model = await _userModelFactory.PrepareUserSearchModelAsync(new UserSearchModel(), true);

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> UsersAddPopupList(UserSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _userModelFactory.PrepareUserListModelAsync(searchModel);

      return Json(model);
   }


   [HttpPost]
   [FormValueRequired("save")]
   public virtual async Task<IActionResult> UsersAddPopup(DeviceUsersModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      var device = await _deviceService.GetDeviceByIdAsync(model.DeviceId);
      var owner = await _userService.GetUserByIdAsync(device.OwnerId);

      var existingSharedUserIds = (await _userService.GetUsersByDeviceIdAsync(model.DeviceId)).Select(x => x.Id);

      var newSharedUserIds = model.SelectedUserIds.Where(x => !existingSharedUserIds.Contains(x));

      // exclude the owner
      newSharedUserIds = newSharedUserIds.Where(x => x != owner.Id);

      // exclude guests
      newSharedUserIds = await _userService.GetOnlyRegisteredUserIdsAsync(newSharedUserIds);

      if (newSharedUserIds.Any())
      {
         await _deviceService.MapDeviceToUserAsync(model.DeviceId, newSharedUserIds);

         var newSharedUsers = await _userService.GetUsersByIdsAsync(newSharedUserIds.ToArray());

         var user = await _workContext.GetCurrentUserAsync();
         await _deviceActivityService.InsertActivityAsync(device, "Device.AddMap", $"Device {device.SystemName} has been mapped to users: {string.Join(", ", newSharedUsers.Select(x => x.Email).ToArray())} - by {user.Email}");
      }

      ViewBag.RefreshPage = true;

      return View(new UserSearchModel());
   }

   #endregion

   #region Device

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }


   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceSearchModelAsync(new DeviceSearchModel());

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> DeviceList(DeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceListModelAsync(searchModel);

      return Json(model);
   }


   [HttpPost, ActionName("Edit")]
   [FormValueRequired("changepassword")]
   public virtual async Task<IActionResult> ChangePassword(DeviceModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //try to get a device with the specified id
      var device = await _deviceService.GetDeviceByIdAsync(model.Id);
      if (device == null)
         return RedirectToAction("List");

      //ensure that the current user can change passwords
      if (!await _userService.IsAdminAsync(await _workContext.GetCurrentUserAsync()))
      {
         _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Devices.Devices.OnlyAdminCanChangePassword"));
         return RedirectToAction("Edit", new { id = device.Id });
      }

      if (!ModelState.IsValid)
         return RedirectToAction("Edit", new { id = device.Id });

      var changePassRequest = new ChangePasswordRequest(model.SystemName, false, _deviceSettings.DefaultPasswordFormat, model.Password);
      var changePassResult = await _deviceRegistrationService.ChangePasswordAsync(changePassRequest);

      if (changePassResult.Success)
      {
         var user = await _workContext.GetCurrentUserAsync();
         await _deviceActivityService.InsertActivityAsync(device, "Device.PasswordChanged", $"Password of {device.SystemName} has been changed by {user.Email}.", user);
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Devices.Devices.PasswordChanged"));
      }
      else
         foreach (var error in changePassResult.Errors)
            _notificationService.ErrorNotification(error);

      return RedirectToAction("Edit", new { id = device.Id });
   }


   public virtual async Task<IActionResult> Create()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceModelAsync(new DeviceModel(), null);

      return View(model);
   }


   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   [FormValueRequired("save", "save-continue")]
   public virtual async Task<IActionResult> Create(DeviceModel model, bool continueEditing, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      // check owner 
      var owner = await _userService.GetUserByIdAsync(model.OwnerId);
      if (owner == null || !await _userService.IsRegisteredAsync(owner))
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Register.Errors.OwnerIsNotRegistered"));

      if (ModelState.IsValid)
      {
         try
         {
            //fill entity from model
            var device = model.ToEntity<Device>();

            var registrationRequest = new DeviceRegistrationRequest(device, model.SystemName, model.Password, _deviceSettings.DefaultPasswordFormat, true);
            var registerResult = await _deviceRegistrationService.RegisterDeviceAsync(registrationRequest);

            if (registerResult.Success)
            {
               device.CannotLoginUntilDateUtc = model.CannotLoginUntilDateUtc.HasValue
               ? _dateTimeHelper.ConvertToUtcTime(model.CannotLoginUntilDateUtc.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
               : null;

               await _deviceService.UpdateDeviceAsync(device);

               // save locale (device entity is localizable entity) for the current language
               var lang = await _workContext.GetWorkingLanguageAsync();
               await _localizedEntityService.SaveLocalizedValueAsync(device, x => x.Name, model.Name, lang.Id);
               await _localizedEntityService.SaveLocalizedValueAsync(device, x => x.Description, model.Description, lang.Id);

               // save device configuration for further syncronization with the dispatcher
               await _genericAttributeService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);

               // activity log
               var user = await _workContext.GetCurrentUserAsync();
               await _deviceActivityService.InsertActivityAsync(device, "Device.Register", $"Device {device.SystemName} has been registered by {user.Email}.", user);

               // notification
               _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Devices.Devices.Added"));

               if (!continueEditing)
                  return RedirectToAction("List");

               return RedirectToAction("Edit", new { id = device.Id });
            }

            //errors
            foreach (var error in registerResult.Errors)
               ModelState.AddModelError("", error);
         }
         catch (Exception ex)
         {
            //ModelState.AddModelError("", ex.Message);
            _notificationService.ErrorNotification(ex.Message);
         }
      }

      //prepare model
      model = await _deviceModelFactory.PrepareDeviceModelAsync(model, null);

      //if we got this far, something failed, redisplay form
      return View(model);
   }


   public virtual async Task<IActionResult> Edit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //try to get a device with the specified id
      var device = await _deviceService.GetDeviceByIdAsync(id);
      if (device == null || device.IsDeleted)
         return RedirectToAction("List");

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceModelAsync(null, device);

      return View(model);
   }


   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   [FormValueRequired("save", "save-continue")]
   public virtual async Task<IActionResult> Edit(DeviceModel model, bool continueEditing, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //try to get a device with the specified id
      var device = await _deviceService.GetDeviceByIdAsync(model.Id);
      if (device == null || device.IsDeleted)
         return RedirectToAction("List");

      // check owner 
      var owner = await _userService.GetUserByIdAsync(model.OwnerId);
      if (owner == null || !await _userService.IsRegisteredAsync(owner))
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Register.Errors.OwnerIsNotRegistered"));

      // check new system name (if it is changed)
      if (model.SystemName != device.SystemName)
      {
         if (_userSettings.UsernameMinLenght > 0 && model.SystemName.Length < _userSettings.UsernameMinLenght)
            ModelState.AddModelError("", string.Format(await _localizationService.GetResourceAsync("Account.Register.Errors.SystemNameIsShort"), _userSettings.UsernameMinLenght));

         if ((await _deviceService.GetDeviceBySystemNameAsync(model.SystemName)) is not null)
            ModelState.AddModelError("", string.Format(await _localizationService.GetResourceAsync("Account.Register.Errors.SystemNameAlreadyExists"), model.SystemName));
      }

      if (ModelState.IsValid)
      {
         try
         {
            var isOwnerChanged = model.OwnerId != device.OwnerId;
            var isSystemNameChanged = model.SystemName != device.SystemName;
            var prevSystemName = device.SystemName;

            // save device configuration for further syncronization with the dispatcher
            if (isSystemNameChanged)
               await _genericAttributeService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);

            device = model.ToEntity(device);

            device.CannotLoginUntilDateUtc = model.CannotLoginUntilDateUtc.HasValue
               ? _dateTimeHelper.ConvertToUtcTime(model.CannotLoginUntilDateUtc.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
               : null;

            //unmap new owner from shared devices (if map exists)
            if (isOwnerChanged)
               await _deviceService.UnmapDeviceFromUserAsync(device.Id, model.OwnerId);

            // update device
            await _deviceService.UpdateDeviceAsync(device);

            // save locale (device entity is localizable entity) for the current language
            var lang = await _workContext.GetWorkingLanguageAsync();
            await _localizedEntityService.SaveLocalizedValueAsync(device, x => x.Name, model.Name, lang.Id);
            await _localizedEntityService.SaveLocalizedValueAsync(device, x => x.Description, model.Description, lang.Id);

            var user = await _workContext.GetCurrentUserAsync();

            if (isSystemNameChanged)
               await _deviceActivityService.InsertActivityAsync(device, "Device.SystemNameChanged", $"Device system name {prevSystemName} has been changed to {device.SystemName} by {user.Email}", user);

            if (isOwnerChanged)
               await _deviceActivityService.InsertActivityAsync(device, "Device.OwnerChanged", $"Device owner of {device.SystemName} has been changed by {user.Email}", device);

            //activity log
            await _deviceActivityService.InsertActivityAsync(device, "Device.Updated", $"Device {device.SystemName} has been updated by {user.Email}", user);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Devices.Devices.Updated"));

            if (!continueEditing)
               return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = device.Id });
         }
         catch (Exception exc)
         {
            _notificationService.ErrorNotification(exc.Message);
         }
      }

      //prepare model
      model = await _deviceModelFactory.PrepareDeviceModelAsync(model, device);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //try to get a user with the specified id
      var device = await _deviceService.GetDeviceByIdAsync(id);
      if (device == null)
         return RedirectToAction("List");

      try
      {
         //soft deleting (device is ISoftDelete entity). So we don't delete locale properties
         await _deviceService.DeleteAsync(device);

         //activity log
         var user = await _workContext.GetCurrentUserAsync();
         await _deviceActivityService.InsertActivityAsync(device, "Device.Deleted", $"Device {device.SystemName} has been deleted by {user.Username}", user);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Devices.Devices.Deleted"));

         return RedirectToAction("List");
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
         return RedirectToAction("Edit", new { id = device.Id });
      }
   }


   [HttpPost]
   public virtual async Task<IActionResult> DeleteMap(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      long userId = 0, deviceId = 0;
      if (Request.Query.TryGetValue("DeviceId", out var deviceIdValue) && deviceIdValue.Count == 1)
      {
         userId = id;
         deviceId = Convert.ToInt64(deviceIdValue.ToString());
      }
      else if (Request.Query.TryGetValue("UserId", out var userIdValue) && userIdValue.Count == 1)
      {
         deviceId = id;
         userId = Convert.ToInt64(userIdValue.ToString());
      }

      try
      {
         //try to get a user with the specified id
         await _deviceService.UnmapDeviceFromUserAsync(deviceId, userId);

         //activity log
         var user = await _workContext.GetCurrentUserAsync();
         var mapDevice = await _deviceService.GetDeviceByIdAsync(deviceId);
         var mapUser = await _userService.GetUserByIdAsync(userId);
         await _deviceActivityService.InsertActivityAsync(mapDevice, "Device.DeleteMap", $"Map device {mapDevice.SystemName} to user {mapUser.Email} has been deleted by {user.Email}", user);

         return new NullJsonResult();
      }
      catch (Exception exc)
      {
         return Content(exc.ToString());
      }
   }


   public virtual async Task<IActionResult> DeviceUsersSelect(DeviceUserSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return await AccessDeniedDataTablesJson();

      //try to get a device with the specified id
      var device = await _deviceService.GetDeviceByIdAsync(searchModel.DeviceId)
          ?? throw new ArgumentException("No device found with the specified id");

      var model = await _deviceModelFactory.PrepareDeviceUserListModelAsync(searchModel, device);

      return Json(model);
   }


   #endregion

   #region Device statistics

   [SaveLastVisitedPage(true)]
   public virtual async Task<IActionResult> LoadDeviceStatistics(string period)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return Content(string.Empty);

      var result = new List<object>();

      var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
      var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();

      var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);

      switch (period)
      {
         case "year":
            //year statistics
            var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
            var searchYearDateDevice = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
            for (var i = 0; i <= 12; i++)
            {
               result.Add(new
               {
                  date = searchYearDateDevice.Date.ToString("Y", culture),
                  value = (await _deviceService.GetAllPagedDevicesAsync(
                       createdOnTo: _dateTimeHelper.ConvertToUtcTime(searchYearDateDevice.AddMonths(1), timeZone),
                       pageIndex: 0,
                       pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
               });

               searchYearDateDevice = searchYearDateDevice.AddMonths(1);
            }

            break;
         case "month":
            //month statistics
            var monthAgoDt = nowDt.AddDays(-30);
            var searchMonthDateDevice = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
            for (var i = 0; i <= 30; i++)
            {
               result.Add(new
               {
                  date = searchMonthDateDevice.Date.ToString("M", culture),
                  value = (await _deviceService.GetAllPagedDevicesAsync(
                       createdOnTo: _dateTimeHelper.ConvertToUtcTime(searchMonthDateDevice.AddDays(1), timeZone),
                       pageIndex: 0,
                       pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
               });

               searchMonthDateDevice = searchMonthDateDevice.AddDays(1);
            }

            break;
         case "week":
         default:
            //week statistics
            var weekAgoDt = nowDt.AddDays(-7);
            var searchWeekDateDevice = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
            for (var i = 0; i <= 7; i++)
            {
               result.Add(new
               {
                  date = searchWeekDateDevice.Date.ToString("d dddd", culture),
                  value = (await _deviceService.GetAllPagedDevicesAsync(
                       createdOnTo: _dateTimeHelper.ConvertToUtcTime(searchWeekDateDevice.AddDays(1), timeZone),
                       pageIndex: 0,
                       pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
               });

               searchWeekDateDevice = searchWeekDateDevice.AddDays(1);
            }

            break;
      }

      return Json(result);
   }

   [SaveLastVisitedPage(true)]
   public virtual async Task<IActionResult> LoadDataRecordsStatistics(string period, long deviceId, long userId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return Content(string.Empty);

      var timeInterval = period switch
      {
         "year" => (TimeIntervalType.Year, "Y"),
         "quarter" => (TimeIntervalType.Quarter, "Y"),
         "month" => (TimeIntervalType.Month, "M"),
         "decade" => (TimeIntervalType.Decade, "d"),
         "week" => (TimeIntervalType.Week, "d"),
         "day" => (TimeIntervalType.Day, "t"),
         "hour" => (TimeIntervalType.Hour, "t"),
         "minute" => (TimeIntervalType.Minute, "ss"),
         _ => (TimeIntervalType.Day, "t")
      };

      var filter = new DynamicFilter()
      {
         UserId = userId > 0 ? userId : null,
         DeviceId = deviceId > 0 ? deviceId : null,
         TimeInterval = timeInterval.Item1
      };

      var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);
     
      var result = (await _sensorDataService.GetDataStatisticsAsync(filter))
      .SelectAwait(async x => new
      {
         date = (await _dateTimeHelper.ConvertToUserTimeAsync(new DateTime(x.Moment, DateTimeKind.Utc))).ToString(timeInterval.Item2, culture),
         value = x.RecordCount

      }).ToEnumerable();

      return Json(result);
   }

   #endregion

   #region Activity log


   [HttpPost]
   public virtual async Task<IActionResult> ListActivityLog(DeviceActivityLogSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return await AccessDeniedDataTablesJson();

      //try to get a device with the specified id
      var device = await _deviceService.GetDeviceByIdAsync(searchModel.DeviceId)
          ?? throw new ArgumentException("No device found with the specified id");

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceActivityLogListModelAsync(searchModel, device);

      return Json(model);
   }


   #endregion

   #region Export / Import

   [HttpPost, ActionName("ExportExcel")]
   [FormValueRequired("exportexcel-all")]
   public virtual async Task<IActionResult> ExportExcelAll(DeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return AccessDeniedView();

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

      try
      {
         var bytes = await _exportManager.ExportDevicesToXlsxAsync(devices);
         return File(bytes, MimeTypes.TextXlsx, "devices.xlsx");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   [HttpPost]
   public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      var devices = new List<Device>();
      if (selectedIds != null)
      {
         var ids = selectedIds
             .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(x => Convert.ToInt64(x))
             .ToArray();
         devices.AddRange(await _deviceService.GetDevicesByIdsAsync(ids));
      }

      try
      {
         var bytes = await _exportManager.ExportDevicesToXlsxAsync(devices);
         return File(bytes, MimeTypes.TextXlsx, "devices.xlsx");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   [HttpPost, ActionName("ExportXML")]
   [FormValueRequired("exportxml-all")]
   public virtual async Task<IActionResult> ExportXmlAll(DeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return AccessDeniedView();

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

      try
      {
         var xml = await _exportManager.ExportDevicesToXmlAsync(devices);
         return File(Encoding.UTF8.GetBytes(xml), "application/xml", "devices.xml");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   [HttpPost]
   public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      var devices = new List<Device>();
      if (selectedIds != null)
      {
         var ids = selectedIds
             .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(x => Convert.ToInt64(x))
             .ToArray();
         devices.AddRange(await _deviceService.GetDevicesByIdsAsync(ids));
      }

      try
      {
         var xml = await _exportManager.ExportDevicesToXmlAsync(devices);
         return File(Encoding.UTF8.GetBytes(xml), "application/xml", "devices.xml");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   #endregion

   #endregion
}
