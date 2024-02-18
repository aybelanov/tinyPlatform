using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Hub.Core;
using Hub.Core.Domain.Security;
using Hub.Services.Users;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class SecurityController : BaseAdminController
{
   #region Fields

   private readonly IUserService _userService;
   private readonly ILocalizationService _localizationService;
   private readonly ILogger _logger;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly ISecurityModelFactory _securityModelFactory;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public SecurityController(IUserService userService,
       ILocalizationService localizationService,
       ILogger logger,
       INotificationService notificationService,
       IPermissionService permissionService,
       ISecurityModelFactory securityModelFactory,
       IWorkContext workContext)
   {
      _userService = userService;
      _localizationService = localizationService;
      _logger = logger;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _securityModelFactory = securityModelFactory;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> AccessDenied(string pageUrl)
   {
      var currentUser = await _workContext.GetCurrentUserAsync();
      if (currentUser == null || await _userService.IsGuestAsync(currentUser))
      {
         await _logger.InformationAsync($"Access denied to anonymous request on {pageUrl}");
         return View();
      }

      await _logger.InformationAsync($"Access denied to user #{currentUser.Email} '{currentUser.Email}' on {pageUrl}");

      return View();
   }

   public virtual async Task<IActionResult> Permissions()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      //prepare model
      var model = await _securityModelFactory.PreparePermissionMappingModelAsync(new PermissionMappingModel());

      return View(model);
   }

   [HttpPost, ActionName("Permissions")]
   public virtual async Task<IActionResult> PermissionsSave(PermissionMappingModel model, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
         return AccessDeniedView();

      var permissionRecords = await _permissionService.GetAllPermissionRecordsAsync();
      var userRoles = await _userService.GetAllUserRolesAsync(true);

      foreach (var cr in userRoles)
      {
         var formKey = "allow_" + cr.Id;
         var permissionRecordSystemNamesToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
             ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
             : new List<string>();

         foreach (var pr in permissionRecords)
         {
            var allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);

            if (allow == await _permissionService.AuthorizeAsync(pr.SystemName, cr.Id))
               continue;

            if (allow)
               await _permissionService.InsertPermissionRecordUserRoleMappingAsync(new PermissionRecordUserRole { PermissionRecordId = pr.Id, UserRoleId = cr.Id });
            else
               await _permissionService.DeletePermissionRecordUserRoleMappingAsync(pr.Id, cr.Id);

            await _permissionService.UpdatePermissionRecordAsync(pr);
         }
      }

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.ACL.Updated"));

      return RedirectToAction("Permissions");
   }

   #endregion
}