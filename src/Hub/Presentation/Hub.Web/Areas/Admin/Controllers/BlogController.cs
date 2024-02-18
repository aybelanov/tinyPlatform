using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Services.Blogs;
using Hub.Services.Common;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Blogs;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class BlogController : BaseAdminController
{
   #region Fields

   private readonly IBlogModelFactory _blogModelFactory;
   private readonly IBlogService _blogService;
   private readonly IUserActivityService _userActivityService;
   private readonly IEventPublisher _eventPublisher;
   private readonly ILocalizationService _localizationService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;


   #endregion

   #region Ctor

   public BlogController(IBlogModelFactory blogModelFactory,
       IBlogService blogService,
       IUserActivityService userActivityService,
       IEventPublisher eventPublisher,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       IWorkContext workContext,
       INotificationService notificationService,
       IPermissionService permissionService,
       IUrlRecordService urlRecordService)
   {
      _blogModelFactory = blogModelFactory;
      _blogService = blogService;
      _userActivityService = userActivityService;
      _eventPublisher = eventPublisher;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _urlRecordService = urlRecordService;
      _workContext = workContext;
      _genericAttributeService = genericAttributeService;
   }

   #endregion

   #region Methods        

   #region Blog posts

   public virtual IActionResult Index()
   {
      return RedirectToAction("BlogPosts");
   }

   public virtual async Task<IActionResult> BlogPosts(long? filterByBlogPostId, [FromQuery] bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //prepare model
      var model = await _blogModelFactory.PrepareBlogContentModelAsync(new BlogContentModel(), filterByBlogPostId);

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
   public virtual async Task<IActionResult> List(BlogPostSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _blogModelFactory.PrepareBlogPostListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> BlogPostCreate()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //prepare model
      var model = await _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> BlogPostCreate(BlogPostModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var blogPost = model.ToEntity<BlogPost>();
         blogPost.CreatedOnUtc = DateTime.UtcNow;
         await _blogService.InsertBlogPostAsync(blogPost);

         //activity log
         await _userActivityService.InsertActivityAsync("AddNewBlogPost",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewBlogPost"), blogPost.Id), blogPost);

         //search engine name
         var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
         await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Added"));

         if (!continueEditing)
            return RedirectToAction("BlogPosts");

         return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
      }

      //prepare model
      model = await _blogModelFactory.PrepareBlogPostModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> BlogPostEdit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //try to get a blog post with the specified id
      var blogPost = await _blogService.GetBlogPostByIdAsync(id);
      if (blogPost == null)
         return RedirectToAction("BlogPosts");

      //prepare model
      var model = await _blogModelFactory.PrepareBlogPostModelAsync(null, blogPost);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> BlogPostEdit(BlogPostModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //try to get a blog post with the specified id
      var blogPost = await _blogService.GetBlogPostByIdAsync(model.Id);
      if (blogPost == null)
         return RedirectToAction("BlogPosts");

      if (ModelState.IsValid)
      {
         blogPost = model.ToEntity(blogPost);
         await _blogService.UpdateBlogPostAsync(blogPost);

         //activity log
         await _userActivityService.InsertActivityAsync("EditBlogPost",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogPost"), blogPost.Id), blogPost);

         //search engine name
         var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
         await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Updated"));

         if (!continueEditing)
            return RedirectToAction("BlogPosts");

         return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
      }

      //prepare model
      model = await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //try to get a blog post with the specified id
      var blogPost = await _blogService.GetBlogPostByIdAsync(id);
      if (blogPost == null)
         return RedirectToAction("BlogPosts");

      await _blogService.DeleteBlogPostAsync(blogPost);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteBlogPost",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPost"), blogPost.Id), blogPost);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Deleted"));

      return RedirectToAction("BlogPosts");
   }

   #endregion

   #region Comments

   public virtual async Task<IActionResult> BlogComments(long? filterByBlogPostId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //try to get a blog post with the specified id
      var blogPost = await _blogService.GetBlogPostByIdAsync(filterByBlogPostId ?? 0);
      if (blogPost == null && filterByBlogPostId.HasValue)
         return RedirectToAction("BlogComments");

      //prepare model
      var model = await _blogModelFactory.PrepareBlogCommentSearchModelAsync(new BlogCommentSearchModel(), blogPost);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Comments(BlogCommentSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _blogModelFactory.PrepareBlogCommentListModelAsync(searchModel, searchModel.BlogPostId);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> CommentUpdate(BlogCommentModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //try to get a blog comment with the specified id
      var comment = await _blogService.GetBlogCommentByIdAsync(model.Id)
          ?? throw new ArgumentException("No comment found with the specified id");

      var previousIsApproved = comment.IsApproved;

      //fill entity from model
      comment = model.ToEntity(comment);

      await _blogService.UpdateBlogCommentAsync(comment);

      //raise event (only if it wasn't approved before and is approved now)
      if (!previousIsApproved && comment.IsApproved)
         await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

      //activity log
      await _userActivityService.InsertActivityAsync("EditBlogComment",
         string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), comment.Id), comment);

      return new NullJsonResult();
   }

   public virtual async Task<IActionResult> CommentDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      //try to get a blog comment with the specified id
      var comment = await _blogService.GetBlogCommentByIdAsync(id)
          ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

      await _blogService.DeleteBlogCommentAsync(comment);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteBlogPostComment",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), comment.Id), comment);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      var comments = await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray());

      await _blogService.DeleteBlogCommentsAsync(comments);
      //activity log
      foreach (var blogComment in comments)
         await _userActivityService.InsertActivityAsync("DeleteBlogPostComment",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), blogComment.Id), blogComment);

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> ApproveSelected(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      //filter not approved comments
      var blogComments = (await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

      foreach (var blogComment in blogComments)
      {
         blogComment.IsApproved = true;

         await _blogService.UpdateBlogCommentAsync(blogComment);

         //raise event 
         await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(blogComment));

         //activity log
         await _userActivityService.InsertActivityAsync("EditBlogComment",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
      }

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> DisapproveSelected(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      //filter approved comments
      var blogComments = (await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved);

      foreach (var blogComment in blogComments)
      {
         blogComment.IsApproved = false;

         await _blogService.UpdateBlogCommentAsync(blogComment);

         //activity log
         await _userActivityService.InsertActivityAsync("EditBlogComment",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
      }

      return Json(new { Result = true });
   }

   #endregion

   #endregion
}