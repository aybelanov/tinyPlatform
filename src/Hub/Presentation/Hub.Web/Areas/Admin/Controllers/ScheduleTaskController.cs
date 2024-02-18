﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.ScheduleTasks;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Areas.Admin.Models.Tasks;
using Hub.Web.Areas.Admin.Factories;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class ScheduleTaskController : BaseAdminController
{
   #region Fields

   private readonly IUserActivityService _userActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IScheduleTaskModelFactory _scheduleTaskModelFactory;
   private readonly IScheduleTaskService _scheduleTaskService;
   private readonly IScheduleTaskRunner _taskRunner;

   #endregion

   #region Ctor

   public ScheduleTaskController(IUserActivityService userActivityService,
       ILocalizationService localizationService,
       INotificationService notificationService,
       IPermissionService permissionService,
       IScheduleTaskModelFactory scheduleTaskModelFactory,
       IScheduleTaskService scheduleTaskService,
       IScheduleTaskRunner taskRunner)
   {
      _userActivityService = userActivityService;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _scheduleTaskModelFactory = scheduleTaskModelFactory;
      _scheduleTaskService = scheduleTaskService;
      _taskRunner = taskRunner;
   }

   #endregion

   #region Methods

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
         return AccessDeniedView();

      //prepare model
      var model = await _scheduleTaskModelFactory.PrepareScheduleTaskSearchModelAsync(new ScheduleTaskSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> List(ScheduleTaskSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _scheduleTaskModelFactory.PrepareScheduleTaskListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> TaskUpdate(ScheduleTaskModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
         return AccessDeniedView();

      //try to get a schedule task with the specified id
      var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.Id)
                         ?? throw new ArgumentException("Schedule task cannot be loaded");

      //To prevent inject the XSS payload in Schedule tasks ('Name' field), we must disable editing this field, 
      //but since it is required, we need to get its value before updating the entity.
      if (!string.IsNullOrEmpty(scheduleTask.Name))
      {
         model.Name = scheduleTask.Name;
         ModelState.Remove(nameof(model.Name));
      }

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      if (!scheduleTask.Enabled && model.Enabled)
         scheduleTask.LastEnabledUtc = DateTime.UtcNow;

      scheduleTask = model.ToEntity(scheduleTask);

      await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

      //activity log
      await _userActivityService.InsertActivityAsync("EditTask",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTask"), scheduleTask.Id), scheduleTask);

      return new NullJsonResult();
   }

   public virtual async Task<IActionResult> RunNow(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
         return AccessDeniedView();

      try
      {
         //try to get a schedule task with the specified id
         var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id)
                            ?? throw new ArgumentException("Schedule task cannot be loaded", nameof(id));

         await _taskRunner.ExecuteAsync(scheduleTask, true, true, false);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.ScheduleTasks.RunNow.Done"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("List");
   }

   #endregion
}