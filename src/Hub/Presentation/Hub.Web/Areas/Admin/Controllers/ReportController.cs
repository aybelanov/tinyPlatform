using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Reports;
using Microsoft.AspNetCore.Mvc;
using Hub.Services.Security;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class ReportController : BaseAdminController
{
   #region Fields

   private readonly IPermissionService _permissionService;
   private readonly IReportModelFactory _reportModelFactory;

   #endregion

   #region Ctor

   public ReportController(
       IPermissionService permissionService,
       IReportModelFactory reportModelFactory)
   {
      _permissionService = permissionService;
      _reportModelFactory = reportModelFactory;
   }

   #endregion

   #region Methods

   #region User reports

   public virtual async Task<IActionResult> RegisteredUsers()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _reportModelFactory.PrepareUserReportsSearchModelAsync(new UserReportsSearchModel());

      return View(model);
   }

   public virtual async Task<IActionResult> BestUsersByOrderTotal()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _reportModelFactory.PrepareUserReportsSearchModelAsync(new UserReportsSearchModel());

      return View(model);
   }

   public virtual async Task<IActionResult> BestUsersByNumberOfOrders()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _reportModelFactory.PrepareUserReportsSearchModelAsync(new UserReportsSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ReportRegisteredUsersList(RegisteredUsersReportSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _reportModelFactory.PrepareRegisteredUsersReportListModelAsync(searchModel);

      return Json(model);
   }

   #endregion

   #endregion
}
