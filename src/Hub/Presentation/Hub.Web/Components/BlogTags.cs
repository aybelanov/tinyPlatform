using Hub.Core.Domain.Blogs;
using Hub.Services.Security;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class BlogTagsViewComponent : AppViewComponent
{
   private readonly BlogSettings _blogSettings;
   private readonly IBlogModelFactory _blogModelFactory;
   private readonly IPermissionService _permissionService;

   public BlogTagsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory, IPermissionService permissionService)
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

      var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
      return View(model);
   }
}
