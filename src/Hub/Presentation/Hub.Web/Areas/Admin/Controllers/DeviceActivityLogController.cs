using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Clients;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Logging;
using Hub.Web.Framework.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class DeviceActivityLogController : BaseAdminController
{
   #region Fields

   private readonly IActivityLogModelFactory _activityLogModelFactory;
   private readonly IUserActivityService _userActivityService;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly IPermissionService _permissionService;
   private readonly INotificationService _notificationService;

   #endregion

   #region Ctor

   public DeviceActivityLogController(IActivityLogModelFactory activityLogModelFactory,
       IUserActivityService userActivityService, 
       IDeviceActivityService deviceActivityService,
       ILocalizationService localizationService,
       INotificationService notificationService,
       IPermissionService permissionService)
   {
      _activityLogModelFactory = activityLogModelFactory;
      _userActivityService = userActivityService;
      _deviceActivityService = deviceActivityService;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> ActivityTypes()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
         return AccessDeniedView();

      //prepare model
      var model = await _activityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel() { Subject = typeof(Device).Name });

      return View(model);
   }

   [HttpPost, ActionName("SaveTypes")]
   public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
         return AccessDeniedView();

      //activity log
      await _userActivityService.InsertActivityAsync("EditActivityLogTypes", "Device activity log types have been edited.");

      //get identifiers of selected activity types
      var selectedActivityTypesIds = form["checkbox_activity_types"]
          .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
          .Select(idString => long.TryParse(idString, out var id) ? id : 0)
          .Distinct().ToList();

      //update activity types
      var activityTypes = await _deviceActivityService.GetAllActivityTypesAsync();
      foreach (var activityType in activityTypes)
      {
         activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
         await _deviceActivityService.UpdateActivityTypeAsync(activityType);
      }

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Devices.ActivityLogType.Updated"));

      return RedirectToAction("ActivityTypes");
   }


   public virtual async Task<IActionResult> ActivityLogs()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
         return AccessDeniedView();

      //prepare model
      var model = await _activityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel()
      {
         SubjectType = typeof(Device).Name
      });

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
         return await AccessDeniedDataTablesJson();

      searchModel.SubjectType = typeof(Device).Name;

      //prepare model
      var model = await _activityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ActivityLogDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
         return AccessDeniedView();

      //try to get a log item with the specified id
      var logItem = await _deviceActivityService.GetActivityByIdAsync(id)
          ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

      await _deviceActivityService.DeleteActivityAsync(logItem);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteActivityLog", $"Activity log record for {logItem.SubjectName} {logItem.SubjectId} has been deleted.");

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> ClearAll()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
         return AccessDeniedView();

      await _deviceActivityService.ClearAllActivitiesAsync();

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteActivityLog", "Device activity log has been cleared.");

      return RedirectToAction("ActivityLogs");
   }

   #endregion
}