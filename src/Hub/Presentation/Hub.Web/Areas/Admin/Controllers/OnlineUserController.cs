using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Hub.Services.Security;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class OnlineUserController : BaseAdminController
{
   #region Fields

   private readonly IUserModelFactory _userModelFactory;
   private readonly IPermissionService _permissionService;

   #endregion

   #region Ctor

   public OnlineUserController(IUserModelFactory userModelFactory,
       IPermissionService permissionService)
   {
      _userModelFactory = userModelFactory;
      _permissionService = permissionService;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _userModelFactory.PrepareOnlineUserSearchModelAsync(new OnlineUserSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> List(OnlineUserSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _userModelFactory.PrepareOnlineUserListModelAsync(searchModel);

      return Json(model);
   }

   #endregion
}