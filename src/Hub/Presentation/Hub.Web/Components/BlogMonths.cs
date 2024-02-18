using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Core.Domain.Blogs;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Hub.Services.Security;

namespace Hub.Web.Components;

public class BlogMonthsViewComponent : AppViewComponent
{
   private readonly BlogSettings _blogSettings;
   private readonly IBlogModelFactory _blogModelFactory;
   private readonly IPermissionService _permissionService;

   public BlogMonthsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory, IPermissionService permissionService)
   {
      _blogSettings = blogSettings;
      _blogModelFactory = blogModelFactory;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Content("");

      if (!_blogSettings.Enabled)
         return Content("");

      var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
      return View(model);
   }
}
