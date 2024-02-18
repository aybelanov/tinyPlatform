using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Hub.Services.Security;

namespace Hub.Web.Components;

public class ForumBreadcrumbViewComponent : AppViewComponent
{
   private readonly IForumModelFactory _forumModelFactory;
   private readonly IPermissionService _permissionService;

   public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory, IPermissionService permissionService)
   {
      _forumModelFactory = forumModelFactory;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync(long? forumGroupId, long? forumId, long? forumTopicId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Content("");

      var model = await _forumModelFactory.PrepareForumBreadcrumbModelAsync(forumGroupId, forumId, forumTopicId);
      return View(model);
   }
}
