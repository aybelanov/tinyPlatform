using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Devices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class OnlineDeviceController : BaseAdminController
{
   #region Fields

   private readonly IDeviceModelFactory _deviceModelFactory;
   private readonly IPermissionService _permissionService;

   #endregion

   #region Ctor

   public OnlineDeviceController(IDeviceModelFactory deviceModelFactory,
       IPermissionService permissionService)
   {
      _deviceModelFactory = deviceModelFactory;
      _permissionService = permissionService;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //prepare model
      var model = await _deviceModelFactory.PrepareOnlineDeviceSearchModelAsync(new OnlineDeviceSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> List(OnlineDeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _deviceModelFactory.PrepareOnlineDeviceListModelAsync(searchModel);

      return Json(model);
   }

   #endregion
}