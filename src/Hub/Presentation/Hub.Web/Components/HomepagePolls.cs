using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Hub.Services.Security;

namespace Hub.Web.Components;

public class HomepagePollsViewComponent : AppViewComponent
{
   private readonly IPollModelFactory _pollModelFactory;
   private readonly IPermissionService _permissionService;

   public HomepagePollsViewComponent(IPollModelFactory pollModelFactory, IPermissionService permissionService)
   {
      _pollModelFactory = pollModelFactory;
      _permissionService = permissionService;   
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessPolls))
         return Content("");

      var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
      if (!model.Any())
         return Content("");

      return View(model);
   }
}
