using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class UserRoleController : BaseAdminController
{
   #region Fields

   private readonly IUserActivityService _userActivityService;
   private readonly IUserRoleModelFactory _userRoleModelFactory;
   private readonly IUserService _userService;
   private readonly ILocalizationService _localizationService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public UserRoleController(IUserActivityService userActivityService,
       IUserRoleModelFactory userRoleModelFactory,
       IUserService userService,
       ILocalizationService localizationService,
       INotificationService notificationService,
       IPermissionService permissionService,
       IWorkContext workContext)
   {
      _userActivityService = userActivityService;
      _userRoleModelFactory = userRoleModelFactory;
      _userService = userService;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _userRoleModelFactory.PrepareUserRoleSearchModelAsync(new UserRoleSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> List(UserRoleSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _userRoleModelFactory.PrepareUserRoleListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> Create()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      //prepare model
      var model = await _userRoleModelFactory.PrepareUserRoleModelAsync(new UserRoleModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> Create(UserRoleModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var userRole = model.ToEntity<UserRole>();
         await _userService.InsertUserRoleAsync(userRole);

         //activity log
         await _userActivityService.InsertActivityAsync("AddNewUserRole",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewUserRole"), userRole.Name), userRole);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Added"));

         return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
      }

      //prepare model
      model = await _userRoleModelFactory.PrepareUserRoleModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> Edit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      //try to get a user role with the specified id
      var userRole = await _userService.GetUserRoleByIdAsync(id);
      if (userRole == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _userRoleModelFactory.PrepareUserRoleModelAsync(null, userRole);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> Edit(UserRoleModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      //try to get a user role with the specified id
      var userRole = await _userService.GetUserRoleByIdAsync(model.Id);
      if (userRole == null)
         return RedirectToAction("List");

      try
      {
         if (ModelState.IsValid)
         {
            if (userRole.IsSystemRole && !model.Active)
               throw new AppException(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));

            if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
               throw new AppException(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));

            userRole = model.ToEntity(userRole);
            await _userService.UpdateUserRoleAsync(userRole);

            //activity log
            await _userActivityService.InsertActivityAsync("EditUserRole",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditUserRole"), userRole.Name), userRole);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
         }

         //prepare model
         model = await _userRoleModelFactory.PrepareUserRoleModelAsync(model, userRole, true);

         //if we got this far, something failed, redisplay form
         return View(model);
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("Edit", new { id = userRole.Id });
      }
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      //try to get a user role with the specified id
      var userRole = await _userService.GetUserRoleByIdAsync(id);
      if (userRole == null)
         return RedirectToAction("List");

      try
      {
         await _userService.DeleteUserRoleAsync(userRole);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteUserRole",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteUserRole"), userRole.Name), userRole);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Deleted"));

         return RedirectToAction("List");
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
         return RedirectToAction("Edit", new { id = userRole.Id });
      }
   }

   #endregion
}