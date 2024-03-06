using Hub.Services.Security;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class PollBlockViewComponent : AppViewComponent
{
   private readonly IPollModelFactory _pollModelFactory;
   private readonly IPermissionService _permissionService;

   public PollBlockViewComponent(IPollModelFactory pollModelFactory, IPermissionService permissionService)
   {
      _pollModelFactory = pollModelFactory;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync(string systemKeyword)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessPolls))
         return Content("");

      if (string.IsNullOrWhiteSpace(systemKeyword))
         return Content("");

      var model = await _pollModelFactory.PreparePollModelBySystemNameAsync(systemKeyword);
      if (model == null)
         return Content("");

      return View(model);
   }
}
