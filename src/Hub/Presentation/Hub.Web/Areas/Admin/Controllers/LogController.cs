using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Logging;
using Hub.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class LogController : BaseAdminController
{
   #region Fields

   private readonly IUserActivityService _userActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly ILogger _logger;
   private readonly ILogModelFactory _logModelFactory;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;

   #endregion

   #region Ctor

   public LogController(IUserActivityService userActivityService,
       ILocalizationService localizationService,
       ILogger logger,
       ILogModelFactory logModelFactory,
       INotificationService notificationService,
       IPermissionService permissionService)
   {
      _userActivityService = userActivityService;
      _localizationService = localizationService;
      _logger = logger;
      _logModelFactory = logModelFactory;
      _notificationService = notificationService;
      _permissionService = permissionService;
   }

   #endregion

   #region Methods

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
         return AccessDeniedView();

      //prepare model
      var model = await _logModelFactory.PrepareLogSearchModelAsync(new LogSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> LogList(LogSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _logModelFactory.PrepareLogListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired("clearall")]
   public virtual async Task<IActionResult> ClearAll()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
         return AccessDeniedView();

      await _logger.ClearLogAsync();

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Log.Cleared"));

      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> View(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
         return AccessDeniedView();

      //try to get a log with the specified id
      var log = await _logger.GetLogByIdAsync(id);
      if (log == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _logModelFactory.PrepareLogModelAsync(null, log);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
         return AccessDeniedView();

      //try to get a log with the specified id
      var log = await _logger.GetLogByIdAsync(id);
      if (log == null)
         return RedirectToAction("List");

      await _logger.DeleteLogAsync(log);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"), log);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Log.Deleted"));

      return RedirectToAction("List");
   }

   [HttpPost]
   public virtual async Task<IActionResult> DeleteSelected(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      await _logger.DeleteLogsAsync((await _logger.GetLogByIdsAsync(selectedIds.ToArray())).ToList());

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

      return Json(new { Result = true });
   }

   #endregion
}