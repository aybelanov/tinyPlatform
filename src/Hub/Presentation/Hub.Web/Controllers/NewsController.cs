using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Security;
using Hub.Core.Events;
using Hub.Core.Rss;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.News;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Web.Factories;
using Hub.Web.Framework;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Hub.Web.Models.News;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class NewsController : BasePublicController
{
   #region Fields

   private readonly CaptchaSettings _captchaSettings;
   private readonly AppInfoSettings _appSettings;
   private readonly IUserActivityService _userActivityService;
   private readonly IUserService _userService;
   private readonly IEventPublisher _eventPublisher;
   private readonly ILocalizationService _localizationService;
   private readonly INewsModelFactory _newsModelFactory;
   private readonly INewsService _newsService;
   private readonly IPermissionService _permissionService;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWebHelper _webHelper;
   private readonly IWorkContext _workContext;
   private readonly IWorkflowMessageService _workflowMessageService;
   private readonly LocalizationSettings _localizationSettings;
   private readonly NewsSettings _newsSettings;

   #endregion

   #region Ctor

   public NewsController(CaptchaSettings captchaSettings,
      AppInfoSettings appSettings,
       IUserActivityService userActivityService,
       IUserService userService,
       IEventPublisher eventPublisher,
       ILocalizationService localizationService,
       INewsModelFactory newsModelFactory,
       INewsService newsService,
       IPermissionService permissionService,


       IUrlRecordService urlRecordService,
       IWebHelper webHelper,
       IWorkContext workContext,
       IWorkflowMessageService workflowMessageService,
       LocalizationSettings localizationSettings,
       NewsSettings newsSettings)
   {
      _captchaSettings = captchaSettings;
      _appSettings = appSettings;
      _userActivityService = userActivityService;
      _userService = userService;
      _eventPublisher = eventPublisher;
      _localizationService = localizationService;
      _newsModelFactory = newsModelFactory;
      _newsService = newsService;
      _permissionService = permissionService;


      _urlRecordService = urlRecordService;
      _webHelper = webHelper;
      _workContext = workContext;
      _workflowMessageService = workflowMessageService;
      _localizationSettings = localizationSettings;
      _newsSettings = newsSettings;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List(NewsPagingFilteringModel command)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
         return Challenge();

      if (!_newsSettings.Enabled)
         return RedirectToRoute("Homepage");

      var model = await _newsModelFactory.PrepareNewsItemListModelAsync(command);
      return View(model);
   }

   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> ListRss(long languageId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
         return Challenge();

      var feed = new RssFeed(
          $"{_appSettings.Name}: News",
          "News",
          new Uri(_webHelper.GetAppLocation()),
          DateTime.UtcNow);

      if (!_newsSettings.Enabled)
         return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

      var items = new List<RssItem>();
      var newsItems = await _newsService.GetAllNewsAsync(languageId);
      foreach (var n in newsItems)
      {
         var newsUrl = Url.RouteUrl("NewsItem", new { SeName = await _urlRecordService.GetSeNameAsync(n, n.LanguageId, ensureTwoPublishedLanguages: false) }, _webHelper.GetCurrentRequestProtocol());
         items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:service:{_appSettings.Name}:news:blog:{n.Id}", n.CreatedOnUtc));
      }
      feed.Items = items;
      return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
   }

   public virtual async Task<IActionResult> NewsItem(long newsItemId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
         return Challenge();

      if (!_newsSettings.Enabled)
         return RedirectToRoute("Homepage");

      var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
      if (newsItem == null)
         return InvokeHttp404();

      var notAvailable =
          //published?
          !newsItem.Published ||
          //availability dates
          !_newsService.IsNewsAvailable(newsItem);
      //Check whether the current user has a "Manage news" permission (usually a platform owner)
      //We should allows him (her) to use "Preview" functionality
      var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews);
      if (notAvailable && !hasAdminAccess)
         return InvokeHttp404();

      var model = new NewsItemModel();
      model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

      //display "edit" (manage) link
      if (hasAdminAccess)
         DisplayEditLink(Url.Action("NewsItemEdit", "News", new { id = newsItem.Id, area = AreaNames.Admin }));

      return View(model);
   }

   [HttpPost, ActionName("NewsItem")]
   [FormValueRequired("add-comment")]
   [ValidateCaptcha]
   public virtual async Task<IActionResult> NewsCommentAdd(long newsItemId, NewsItemModel model, bool captchaValid)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
         return Challenge();

      if (!_newsSettings.Enabled)
         return RedirectToRoute("Homepage");

      var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
      if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
         return RedirectToRoute("Homepage");

      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      var user = await _workContext.GetCurrentUserAsync();
      if (await _userService.IsGuestAsync(user) && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("News.Comments.OnlyRegisteredUsersLeaveComments"));

      if (ModelState.IsValid)
      {
         var comment = new NewsComment
         {
            NewsItemId = newsItem.Id,
            UserId = user.Id,
            CommentTitle = model.AddNewComment.CommentTitle,
            CommentText = model.AddNewComment.CommentText,
            IsApproved = !_newsSettings.NewsCommentsMustBeApproved,
            CreatedOnUtc = DateTime.UtcNow,
         };

         await _newsService.InsertNewsCommentAsync(comment);

         //notify a platform owner;
         if (_newsSettings.NotifyAboutNewNewsComments)
            await _workflowMessageService.SendNewsCommentNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

         //activity log
         await _userActivityService.InsertActivityAsync("PublicPlatform.AddNewsComment",
             await _localizationService.GetResourceAsync("ActivityLog.PublicPlatform.AddNewsComment"), comment);

         //raise event
         if (comment.IsApproved)
            await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

         //The text boxes should be cleared after a comment has been posted
         //That' why we reload the page
         TempData["hub.news.addcomment.result"] = comment.IsApproved
             ? await _localizationService.GetResourceAsync("News.Comments.SuccessfullyAdded")
             : await _localizationService.GetResourceAsync("News.Comments.SeeAfterApproving");

         return RedirectToRoute("NewsItem", new { SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
      }

      //If we got this far, something failed, redisplay form
      model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
      return View(model);
   }

   #endregion
}