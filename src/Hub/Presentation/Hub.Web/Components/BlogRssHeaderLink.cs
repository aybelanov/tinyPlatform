using Hub.Core.Domain.Blogs;
using Hub.Services.Security;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class BlogRssHeaderLinkViewComponent : AppViewComponent
{
   private readonly BlogSettings _blogSettings;
   private readonly IPermissionService _permissionService;

   public BlogRssHeaderLinkViewComponent(BlogSettings blogSettings, IPermissionService permissionService)
   {
      _blogSettings = blogSettings;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Content("");

      if (!_blogSettings.Enabled || !_blogSettings.ShowHeaderRssUrl)
         return Content("");

      return View();
   }
}
