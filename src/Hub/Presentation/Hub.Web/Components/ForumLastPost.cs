using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Services.Forums;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Hub.Services.Security;

namespace Hub.Web.Components;

public class ForumLastPostViewComponent : AppViewComponent
{
   private readonly IForumModelFactory _forumModelFactory;
   private readonly IForumService _forumService;
   private readonly IPermissionService _permissionService;

   public ForumLastPostViewComponent(IForumModelFactory forumModelFactory, IForumService forumService, IPermissionService permissionService)
   {
      _forumModelFactory = forumModelFactory;
      _forumService = forumService;
      _permissionService = permissionService;
   }

   public async Task<IViewComponentResult> InvokeAsync(long forumPostId, bool showTopic)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Content("");

      var forumPost = await _forumService.GetPostByIdAsync(forumPostId);
      var model = await _forumModelFactory.PrepareLastPostModelAsync(forumPost, showTopic);

      return View(model);
   }
}
