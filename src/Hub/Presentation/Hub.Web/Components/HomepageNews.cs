using Hub.Core.Domain.News;
using Hub.Services.Security;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class HomepageNewsViewComponent : AppViewComponent
{
   private readonly INewsModelFactory _newsModelFactory;
   private readonly NewsSettings _newsSettings;
   private readonly IPermissionService _permissionService;

   public HomepageNewsViewComponent(INewsModelFactory newsModelFactory, NewsSettings newsSettings, IPermissionService permissionService)
   {
      _newsModelFactory = newsModelFactory;
      _newsSettings = newsSettings;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
         return Content("");

      if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
         return Content("");

      var model = await _newsModelFactory.PrepareHomepageNewsItemsModelAsync();
      return View(model);
   }
}
