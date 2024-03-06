using Hub.Services.Security;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class ForumActiveDiscussionsSmallViewComponent : AppViewComponent
{
   private readonly IForumModelFactory _forumModelFactory;
   private readonly IPermissionService _permissionService;

   public ForumActiveDiscussionsSmallViewComponent(IForumModelFactory forumModelFactory, IPermissionService permissionService)
   {
      _permissionService = permissionService;
      _forumModelFactory = forumModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Content("");

      var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync();
      if (!model.ForumTopics.Any())
         return Content("");

      return View(model);
   }
}
