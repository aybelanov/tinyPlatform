using System;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Forums;
using Hub.Services.Localization;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Forums;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class ForumController : BaseAdminController
{
   #region Fields

   private readonly IForumModelFactory _forumModelFactory;
   private readonly IForumService _forumService;
   private readonly ILocalizationService _localizationService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public ForumController(IForumModelFactory forumModelFactory,
       IForumService forumService,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService, 
       IWorkContext workContext,
       INotificationService notificationService,
       IPermissionService permissionService)
   {
      _forumModelFactory = forumModelFactory;
      _forumService = forumService;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _workContext = workContext;   
   }

   #endregion

   #region Methods

   #region List

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //prepare model
      var model = await _forumModelFactory.PrepareForumGroupSearchModelAsync(new ForumGroupSearchModel());

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ForumGroupList(ForumGroupSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _forumModelFactory.PrepareForumGroupListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ForumList(ForumSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return await AccessDeniedDataTablesJson();

      //try to get a forum group with the specified id
      var forumGroup = await _forumService.GetForumGroupByIdAsync(searchModel.ForumGroupId)
          ?? throw new ArgumentException("No forum group found with the specified id");

      //prepare model
      var model = await _forumModelFactory.PrepareForumListModelAsync(searchModel, forumGroup);

      return Json(model);
   }

   #endregion

   #region Create

   public virtual async Task<IActionResult> CreateForumGroup()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //prepare model
      var model = await _forumModelFactory.PrepareForumGroupModelAsync(new ForumGroupModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> CreateForumGroup(ForumGroupModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var forumGroup = model.ToEntity<ForumGroup>();
         forumGroup.CreatedOnUtc = DateTime.UtcNow;
         forumGroup.UpdatedOnUtc = DateTime.UtcNow;
         await _forumService.InsertForumGroupAsync(forumGroup);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Added"));

         return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
      }

      //prepare model
      model = await _forumModelFactory.PrepareForumGroupModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> CreateForum()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //prepare model
      var model = await _forumModelFactory.PrepareForumModelAsync(new ForumModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> CreateForum(ForumModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var forum = model.ToEntity<Forum>();
         forum.CreatedOnUtc = DateTime.UtcNow;
         forum.UpdatedOnUtc = DateTime.UtcNow;
         await _forumService.InsertForumAsync(forum);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Added"));

         return continueEditing ? RedirectToAction("EditForum", new { forum.Id }) : RedirectToAction("List");
      }

      //prepare model
      model = await _forumModelFactory.PrepareForumModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Edit

   public virtual async Task<IActionResult> EditForumGroup(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //try to get a forum group with the specified id
      var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
      if (forumGroup == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _forumModelFactory.PrepareForumGroupModelAsync(null, forumGroup);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> EditForumGroup(ForumGroupModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //try to get a forum group with the specified id
      var forumGroup = await _forumService.GetForumGroupByIdAsync(model.Id);
      if (forumGroup == null)
         return RedirectToAction("List");

      if (ModelState.IsValid)
      {
         forumGroup = model.ToEntity(forumGroup);
         forumGroup.UpdatedOnUtc = DateTime.UtcNow;
         await _forumService.UpdateForumGroupAsync(forumGroup);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Updated"));

         return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
      }

      //prepare model
      model = await _forumModelFactory.PrepareForumGroupModelAsync(model, forumGroup, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> EditForum(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //try to get a forum with the specified id
      var forum = await _forumService.GetForumByIdAsync(id);
      if (forum == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _forumModelFactory.PrepareForumModelAsync(null, forum);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> EditForum(ForumModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //try to get a forum with the specified id
      var forum = await _forumService.GetForumByIdAsync(model.Id);
      if (forum == null)
         return RedirectToAction("List");

      if (ModelState.IsValid)
      {
         forum = model.ToEntity(forum);
         forum.UpdatedOnUtc = DateTime.UtcNow;
         await _forumService.UpdateForumAsync(forum);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Updated"));

         return continueEditing ? RedirectToAction("EditForum", forum.Id) : RedirectToAction("List");
      }

      //prepare model
      model = await _forumModelFactory.PrepareForumModelAsync(model, forum, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Delete

   [HttpPost]
   public virtual async Task<IActionResult> DeleteForumGroup(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //try to get a forum group with the specified id
      var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
      if (forumGroup == null)
         return RedirectToAction("List");

      await _forumService.DeleteForumGroupAsync(forumGroup);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Deleted"));

      return RedirectToAction("List");
   }

   [HttpPost]
   public virtual async Task<IActionResult> DeleteForum(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
         return AccessDeniedView();

      //try to get a forum with the specified id
      var forum = await _forumService.GetForumByIdAsync(id);
      if (forum == null)
         return RedirectToAction("List");

      await _forumService.DeleteForumAsync(forum);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Deleted"));

      return RedirectToAction("List");
   }

   #endregion

   #endregion
}