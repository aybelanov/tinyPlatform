using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Web.Factories;
using Hub.Web.Models.Blogs;
using Microsoft.AspNetCore.Mvc;
using Hub.Core;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Security;
using Hub.Core.Events;
using Hub.Core.Rss;
using Hub.Services.Blogs;
using Hub.Services.Users;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Web.Framework;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Hub.Core.Domain;

namespace Hub.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class BlogController : BasePublicController
{
   #region Fields

   private readonly BlogSettings _blogSettings;
   private readonly CaptchaSettings _captchaSettings;
   private readonly AppInfoSettings _appSettings;
   private readonly IBlogModelFactory _blogModelFactory;
   private readonly IBlogService _blogService;
   private readonly IUserActivityService _userActivityService;
   private readonly IUserService _userService;
   private readonly IEventPublisher _eventPublisher;
   private readonly ILocalizationService _localizationService;
   private readonly IPermissionService _permissionService;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWebHelper _webHelper;
   private readonly IWorkContext _workContext;
   private readonly IWorkflowMessageService _workflowMessageService;
   private readonly LocalizationSettings _localizationSettings;

   #endregion

   #region Ctor

   public BlogController(BlogSettings blogSettings,
       CaptchaSettings captchaSettings,
       AppInfoSettings appSettings,
       IBlogModelFactory blogModelFactory,
       IBlogService blogService,
       IUserActivityService userActivityService,
       IUserService userService,
       IEventPublisher eventPublisher,
       ILocalizationService localizationService,
       IPermissionService permissionService,
       IUrlRecordService urlRecordService,
       IWebHelper webHelper,
       IWorkContext workContext,
       IWorkflowMessageService workflowMessageService,
       LocalizationSettings localizationSettings)
   {
      _blogSettings = blogSettings;
      _captchaSettings = captchaSettings;
      _appSettings = appSettings;   
      _blogModelFactory = blogModelFactory;
      _blogService = blogService;
      _userActivityService = userActivityService;
      _userService = userService;
      _eventPublisher = eventPublisher;
      _localizationService = localizationService;
      _permissionService = permissionService;
      _urlRecordService = urlRecordService;
      _webHelper = webHelper;
      _workContext = workContext;
      _workflowMessageService = workflowMessageService;
      _localizationSettings = localizationSettings;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List(BlogPagingFilteringModel command)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Challenge();

      if (!_blogSettings.Enabled)
         return RedirectToRoute("Homepage");

      var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
      return View("List", model);
   }


   public virtual async Task<IActionResult> BlogByTag(BlogPagingFilteringModel command)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Challenge();

      if (!_blogSettings.Enabled)
         return RedirectToRoute("Homepage");

      var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
      return View("List", model);
   }


   public virtual async Task<IActionResult> BlogByMonth(BlogPagingFilteringModel command)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Challenge();

      if (!_blogSettings.Enabled)
         return RedirectToRoute("Homepage");

      var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
      return View("List", model);
   }


   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> ListRss(long languageId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Challenge();

      var feed = new RssFeed(
          $"tpiotp: Blog",
          "Blog",
          new Uri(_webHelper.GetAppLocation()),
          DateTime.UtcNow);

      if (!_blogSettings.Enabled)
         return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

      var items = new List<RssItem>();
      var blogPosts = await _blogService.GetAllBlogPostsAsync(languageId);
      foreach (var blogPost in blogPosts)
      {
         var blogPostUrl = Url.RouteUrl("BlogPost", new { SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) }, _webHelper.GetCurrentRequestProtocol());
         items.Add(new RssItem(blogPost.Title, blogPost.Body, new Uri(blogPostUrl),
             $"urn:service:{_appSettings.Name}:blog:post:{blogPost.Id}", blogPost.CreatedOnUtc));
      }
      feed.Items = items;
      return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
   }

   public virtual async Task<IActionResult> BlogPost(long blogPostId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Challenge();

      if (!_blogSettings.Enabled)
         return RedirectToRoute("Homepage");

      var blogPost = await _blogService.GetBlogPostByIdAsync(blogPostId);
      if (blogPost == null)
         return InvokeHttp404();

      var notAvailable =
          //availability dates
          !_blogService.BlogPostIsAvailable(blogPost);
      //Check whether the current user has a "Manage blog" permission (usually a platform owner)
      //We should allows him (her) to use "Preview" functionality
      var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog);
      if (notAvailable && !hasAdminAccess)
         return InvokeHttp404();

      //display "edit" (manage) link
      if (hasAdminAccess)
         DisplayEditLink(Url.Action("BlogPostEdit", "Blog", new { id = blogPost.Id, area = AreaNames.Admin }));

      var model = new BlogPostModel();
      await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

      return View(model);
   }

   [HttpPost, ActionName("BlogPost")]
   [FormValueRequired("add-comment")]
   [ValidateCaptcha]
   public virtual async Task<IActionResult> BlogCommentAdd(long blogPostId, BlogPostModel model, bool captchaValid)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessBlog))
         return Challenge();

      if (!_blogSettings.Enabled)
         return RedirectToRoute("Homepage");

      var blogPost = await _blogService.GetBlogPostByIdAsync(blogPostId);
      if (blogPost == null || !blogPost.AllowComments)
         return RedirectToRoute("Homepage");

      var user = await _workContext.GetCurrentUserAsync();
      if (await _userService.IsGuestAsync(user) && !_blogSettings.AllowNotRegisteredUsersToLeaveComments)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Blog.Comments.OnlyRegisteredUsersLeaveComments"));

      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      if (ModelState.IsValid)
      {
         var comment = new BlogComment
         {
            BlogPostId = blogPost.Id,
            UserId = user.Id,
            CommentText = model.AddNewComment.CommentText,
            IsApproved = !_blogSettings.BlogCommentsMustBeApproved,
            CreatedOnUtc = DateTime.UtcNow,
         };

         await _blogService.InsertBlogCommentAsync(comment);

         //notify a platform owner
         if (_blogSettings.NotifyAboutNewBlogComments)
            await _workflowMessageService.SendBlogCommentNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

         //activity log
         await _userActivityService.InsertActivityAsync("PublicPlatform.AddBlogComment",
             await _localizationService.GetResourceAsync("ActivityLog.PublicPlatform.AddBlogComment"), comment);

         //raise event
         if (comment.IsApproved)
            await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

         //The text boxes should be cleared after a comment has been posted
         //That' why we reload the page
         TempData["hub.blog.addcomment.result"] = comment.IsApproved
             ? await _localizationService.GetResourceAsync("Blog.Comments.SuccessfullyAdded")
             : await _localizationService.GetResourceAsync("Blog.Comments.SeeAfterApproving");
         return RedirectToRoute("BlogPost", new { SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
      }

      //If we got this far, something failed, redisplay form
      await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

      return View(model);
   }

   #endregion
}