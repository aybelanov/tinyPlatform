using Hub.Core.Domain.News;
using Hub.Services.Security;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class NewsRssHeaderLinkViewComponent : AppViewComponent
{
   private readonly NewsSettings _newsSettings;
   private readonly IPermissionService _permissionService;

   public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings, IPermissionService permissionService)
   {
      _newsSettings = newsSettings;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
         return Content("");

      if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
         return Content("");

      return View();
   }
}
