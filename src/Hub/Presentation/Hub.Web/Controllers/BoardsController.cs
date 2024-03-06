using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Security;
using Hub.Core.Rss;
using Hub.Services.Forums;
using Hub.Services.Localization;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Factories;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Hub.Web.Models.Boards;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class BoardsController : BasePublicController
{
   #region Fields

   private readonly CaptchaSettings _captchaSettings;
   private readonly ForumSettings _forumSettings;
   private readonly AppInfoSettings _appSettings;
   private readonly IUserService _userService;
   private readonly IForumModelFactory _forumModelFactory;
   private readonly IForumService _forumService;
   private readonly ILocalizationService _localizationService;
   private readonly IWebHelper _webHelper;
   private readonly IWorkContext _workContext;
   private readonly IPermissionService _permissionService;

   #endregion

   #region Ctor

   public BoardsController(CaptchaSettings captchaSettings,
       ForumSettings forumSettings,
       AppInfoSettings appSettings,
       IUserService userService,
       IForumModelFactory forumModelFactory,
       IForumService forumService,
       ILocalizationService localizationService,
       IPermissionService permissionService,
       IWebHelper webHelper,
       IWorkContext workContext)
   {
      _captchaSettings = captchaSettings;
      _forumSettings = forumSettings;
      _appSettings = appSettings;
      _userService = userService;
      _forumModelFactory = forumModelFactory;
      _forumService = forumService;
      _localizationService = localizationService;
      _webHelper = webHelper;
      _permissionService = permissionService;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> Index()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var model = await _forumModelFactory.PrepareBoardsIndexModelAsync();

      return View(model);
   }


   public virtual async Task<IActionResult> ActiveDiscussions(long forumId = 0, int pageNumber = 1)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync(forumId, pageNumber);

      return View(model);
   }


   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> ActiveDiscussionsRss(long forumId = 0)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      if (!_forumSettings.ActiveDiscussionsFeedEnabled)
         return RedirectToRoute("Boards");

      var topics = await _forumService.GetActiveTopicsAsync(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
      var url = Url.RouteUrl("ActiveDiscussionsRSS", null, _webHelper.GetCurrentRequestProtocol());

      var feedTitle = await _localizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedTitle");
      var feedDescription = await _localizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedDescription");

      var feed = new RssFeed(
         string.Format(feedTitle, _appSettings.Name),
          feedDescription,
          new Uri(url),
          DateTime.UtcNow);

      var items = new List<RssItem>();

      var viewsText = await _localizationService.GetResourceAsync("Forum.Views");
      var repliesText = await _localizationService.GetResourceAsync("Forum.Replies");

      foreach (var topic in topics)
      {
         var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
         var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

         items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl),
             $"urn:store:{_appSettings.Name}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
      }
      feed.Items = items;

      return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
   }


   public virtual async Task<IActionResult> ForumGroup(int id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
      if (forumGroup == null)
         return RedirectToRoute("Boards");

      var model = await _forumModelFactory.PrepareForumGroupModelAsync(forumGroup);

      return View(model);
   }


   public virtual async Task<IActionResult> Forum(long id, int pageNumber = 1)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forum = await _forumService.GetForumByIdAsync(id);
      if (forum == null)
         return RedirectToRoute("Boards");

      var model = await _forumModelFactory.PrepareForumPageModelAsync(forum, pageNumber);

      return View(model);
   }


   public virtual async Task<IActionResult> ForumRss(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      if (!_forumSettings.ForumFeedsEnabled)
         return RedirectToRoute("Boards");

      var topicLimit = _forumSettings.ForumFeedCount;
      var forum = await _forumService.GetForumByIdAsync(id);

      if (forum != null)
      {
         //Order by newest topic posts & limit the number of topics to return
         var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty,
              ForumSearchType.All, 0, 0, topicLimit);

         var url = Url.RouteUrl("ForumRSS", new { id = forum.Id }, _webHelper.GetCurrentRequestProtocol());

         var feedTitle = await _localizationService.GetResourceAsync("Forum.ForumFeedTitle");
         var feedDescription = await _localizationService.GetResourceAsync("Forum.ForumFeedDescription");

         var feed = new RssFeed(
             string.Format(feedTitle, _appSettings.Name, forum.Name),
             feedDescription,
             new Uri(url),
             DateTime.UtcNow);

         var items = new List<RssItem>();

         var viewsText = await _localizationService.GetResourceAsync("Forum.Views");
         var repliesText = await _localizationService.GetResourceAsync("Forum.Replies");

         foreach (var topic in topics)
         {
            var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
            var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

            items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl), $"urn:store:{_appSettings.Name}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
         }

         feed.Items = items;

         return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
      }

      return new RssActionResult(new RssFeed(new Uri(_webHelper.GetAppLocation())), _webHelper.GetThisPageUrl(false));
   }


   [HttpPost]
   public virtual async Task<IActionResult> ForumWatch(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      var watchTopic = await _localizationService.GetResourceAsync("Forum.WatchForum");
      var unwatchTopic = await _localizationService.GetResourceAsync("Forum.UnwatchForum");
      var returnText = watchTopic;

      var forum = await _forumService.GetForumByIdAsync(id);
      if (forum == null)
         return Json(new { Subscribed = false, Text = returnText, Error = true });

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _forumService.IsUserAllowedToSubscribeAsync(user))
         return Json(new { Subscribed = false, Text = returnText, Error = true });

      var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id,
          forum.Id, 0, 0, 1)).FirstOrDefault();

      bool subscribed;
      if (forumSubscription == null)
      {
         forumSubscription = new ForumSubscription
         {
            SubscriptionGuid = Guid.NewGuid(),
            UserId = user.Id,
            ForumId = forum.Id,
            CreatedOnUtc = DateTime.UtcNow
         };
         await _forumService.InsertSubscriptionAsync(forumSubscription);
         subscribed = true;
         returnText = unwatchTopic;
      }
      else
      {
         await _forumService.DeleteSubscriptionAsync(forumSubscription);
         subscribed = false;
      }

      return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
   }


   public virtual async Task<IActionResult> Topic(long id, int pageNumber = 1)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(id);
      if (forumTopic == null)
         return RedirectToRoute("Boards");

      var model = await _forumModelFactory.PrepareForumTopicPageModelAsync(forumTopic, pageNumber);
      //if no posts loaded, redirect to the first page
      if (!model.ForumPostModels.Any() && pageNumber > 1)
         return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });

      //update view count
      forumTopic.Views += 1;
      await _forumService.UpdateTopicAsync(forumTopic);

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> TopicWatch(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      var watchTopic = await _localizationService.GetResourceAsync("Forum.WatchTopic");
      var unwatchTopic = await _localizationService.GetResourceAsync("Forum.UnwatchTopic");
      var returnText = watchTopic;

      var forumTopic = await _forumService.GetTopicByIdAsync(id);
      if (forumTopic == null)
         return Json(new { Subscribed = false, Text = returnText, Error = true });

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _forumService.IsUserAllowedToSubscribeAsync(user))
         return Json(new { Subscribed = false, Text = returnText, Error = true });

      var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id,
          0, forumTopic.Id, 0, 1)).FirstOrDefault();

      bool subscribed;
      if (forumSubscription == null)
      {
         forumSubscription = new ForumSubscription
         {
            SubscriptionGuid = Guid.NewGuid(),
            UserId = user.Id,
            TopicId = forumTopic.Id,
            CreatedOnUtc = DateTime.UtcNow
         };
         await _forumService.InsertSubscriptionAsync(forumSubscription);
         subscribed = true;
         returnText = unwatchTopic;
      }
      else
      {
         await _forumService.DeleteSubscriptionAsync(forumSubscription);
         subscribed = false;
      }

      return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
   }


   public virtual async Task<IActionResult> TopicMove(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(id);
      if (forumTopic == null)
         return RedirectToRoute("Boards");

      var model = await _forumModelFactory.PrepareTopicMoveAsync(forumTopic);

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> TopicMove(TopicMoveModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

      if (forumTopic == null)
         return RedirectToRoute("Boards");

      var newForumId = model.ForumSelected;
      var forum = await _forumService.GetForumByIdAsync(newForumId);

      if (forum != null && forumTopic.ForumId != newForumId)
         await _forumService.MoveTopicAsync(forumTopic.Id, newForumId);

      return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
   }


   [HttpPost]
   public virtual async Task<IActionResult> TopicDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return Json(new
         {
            redirect = Url.RouteUrl("Homepage"),
         });

      var forumTopic = await _forumService.GetTopicByIdAsync(id);
      if (forumTopic != null)
      {
         if (!await _forumService.IsUserAllowedToDeleteTopicAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
            return Challenge();

         var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);

         await _forumService.DeleteTopicAsync(forumTopic);

         if (forum != null)
            return Json(new
            {
               redirect = Url.RouteUrl("ForumSlug", new { id = forum.Id, slug = await _forumService.GetForumSeNameAsync(forum) }),
            });
      }

      return Json(new
      {
         redirect = Url.RouteUrl("Boards"),
      });
   }


   public virtual async Task<IActionResult> TopicCreate(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forum = await _forumService.GetForumByIdAsync(id);
      if (forum == null)
         return RedirectToRoute("Boards");

      if (await _forumService.IsUserAllowedToCreateTopicAsync(await _workContext.GetCurrentUserAsync(), forum) == false)
         return Challenge();

      var model = new EditForumTopicModel();
      await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);
      return View(model);
   }


   [HttpPost]
   [ValidateCaptcha]
   public virtual async Task<IActionResult> TopicCreate(EditForumTopicModel model, bool captchaValid)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forum = await _forumService.GetForumByIdAsync(model.ForumId);
      if (forum == null)
         return RedirectToRoute("Boards");

      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      if (ModelState.IsValid)
         try
         {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _forumService.IsUserAllowedToCreateTopicAsync(user, forum))
               return Challenge();

            var subject = model.Subject;
            var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
            if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
               subject = subject[0..maxSubjectLength];

            var text = model.Text;
            var maxPostLength = _forumSettings.PostMaxLength;
            if (maxPostLength > 0 && text.Length > maxPostLength)
               text = text[0..maxPostLength];

            var topicType = ForumTopicType.Normal;
            var ipAddress = _webHelper.GetCurrentIpAddress();
            var nowUtc = DateTime.UtcNow;

            if (await _forumService.IsUserAllowedToSetTopicPriorityAsync(user))
               topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

            //forum topic
            var forumTopic = new ForumTopic
            {
               ForumId = forum.Id,
               UserId = user.Id,
               TopicTypeId = (int)topicType,
               Subject = subject,
               CreatedOnUtc = nowUtc,
               UpdatedOnUtc = nowUtc
            };
            await _forumService.InsertTopicAsync(forumTopic, true);

            //forum post
            var forumPost = new ForumPost
            {
               ForumTopicId = forumTopic.Id,
               UserId = user.Id,
               Text = text,
               IPAddress = ipAddress,
               CreatedOnUtc = nowUtc,
               UpdatedOnUtc = nowUtc,
            };
            await _forumService.InsertPostAsync(forumPost, false);

            //update forum topic
            forumTopic.NumPosts = 1;
            forumTopic.LastPostId = forumPost.Id;
            forumTopic.LastPostUserId = forumPost.UserId;
            forumTopic.LastPostTime = forumPost.CreatedOnUtc;
            forumTopic.UpdatedOnUtc = nowUtc;
            await _forumService.UpdateTopicAsync(forumTopic);

            //subscription                
            if (await _forumService.IsUserAllowedToSubscribeAsync(user))
               if (model.Subscribed)
               {
                  var forumSubscription = new ForumSubscription
                  {
                     SubscriptionGuid = Guid.NewGuid(),
                     UserId = user.Id,
                     TopicId = forumTopic.Id,
                     CreatedOnUtc = nowUtc
                  };

                  await _forumService.InsertSubscriptionAsync(forumSubscription);
               }

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
         }
         catch (Exception ex)
         {
            ModelState.AddModelError("", ex.Message);
         }

      //redisplay form
      await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);

      return View(model);
   }


   public virtual async Task<IActionResult> TopicEdit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(id);
      if (forumTopic == null)
         return RedirectToRoute("Boards");

      if (!await _forumService.IsUserAllowedToEditTopicAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
         return Challenge();

      var model = new EditForumTopicModel();
      await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, false);

      return View(model);
   }


   [HttpPost]
   [ValidateCaptcha]
   public virtual async Task<IActionResult> TopicEdit(EditForumTopicModel model, bool captchaValid)
   {

      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

      if (forumTopic == null)
         return RedirectToRoute("Boards");

      var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
      if (forum == null)
         return RedirectToRoute("Boards");

      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      if (ModelState.IsValid)
         try
         {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _forumService.IsUserAllowedToEditTopicAsync(user, forumTopic))
               return Challenge();

            var subject = model.Subject;
            var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
            if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
               subject = subject[0..maxSubjectLength];

            var text = model.Text;
            var maxPostLength = _forumSettings.PostMaxLength;
            if (maxPostLength > 0 && text.Length > maxPostLength)
               text = text[0..maxPostLength];

            var topicType = ForumTopicType.Normal;
            var ipAddress = _webHelper.GetCurrentIpAddress();
            var nowUtc = DateTime.UtcNow;

            if (await _forumService.IsUserAllowedToSetTopicPriorityAsync(user))
               topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

            //forum topic
            forumTopic.TopicTypeId = (int)topicType;
            forumTopic.Subject = subject;
            forumTopic.UpdatedOnUtc = nowUtc;
            await _forumService.UpdateTopicAsync(forumTopic);

            //forum post                
            var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
            if (firstPost != null)
            {
               firstPost.Text = text;
               firstPost.UpdatedOnUtc = nowUtc;
               await _forumService.UpdatePostAsync(firstPost);
            }
            else
            {
               //error (not possible)
               firstPost = new ForumPost
               {
                  ForumTopicId = forumTopic.Id,
                  UserId = forumTopic.UserId,
                  Text = text,
                  IPAddress = ipAddress,
                  UpdatedOnUtc = nowUtc
               };

               await _forumService.InsertPostAsync(firstPost, false);
            }

            //subscription
            if (await _forumService.IsUserAllowedToSubscribeAsync(user))
            {
               var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id,
                   0, forumTopic.Id, 0, 1)).FirstOrDefault();
               if (model.Subscribed)
                  if (forumSubscription == null)
                  {
                     forumSubscription = new ForumSubscription
                     {
                        SubscriptionGuid = Guid.NewGuid(),
                        UserId = user.Id,
                        TopicId = forumTopic.Id,
                        CreatedOnUtc = nowUtc
                     };

                     await _forumService.InsertSubscriptionAsync(forumSubscription);
                  }
                  else
                  if (forumSubscription != null)
                     await _forumService.DeleteSubscriptionAsync(forumSubscription);
            }

            // redirect to the topic page with the topic slug
            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
         }
         catch (Exception ex)
         {
            ModelState.AddModelError("", ex.Message);
         }

      //redisplay form
      await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, true);

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> PostDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return Json(new
         {
            redirect = Url.RouteUrl("Homepage"),
         });

      var forumPost = await _forumService.GetPostByIdAsync(id);

      if (forumPost == null)
         return Json(new { redirect = Url.RouteUrl("Boards") });

      if (!await _forumService.IsUserAllowedToDeletePostAsync(await _workContext.GetCurrentUserAsync(), forumPost))
         return Challenge();

      var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.ForumTopicId);
      var forumId = forumTopic.ForumId;
      var forum = await _forumService.GetForumByIdAsync(forumId);
      var forumSlug = await _forumService.GetForumSeNameAsync(forum);

      await _forumService.DeletePostAsync(forumPost);

      //get topic one more time because it can be deleted (first or only post deleted)
      forumTopic = await _forumService.GetTopicByIdAsync(forumPost.ForumTopicId);
      if (forumTopic == null)
         return Json(new
         {
            redirect = Url.RouteUrl("ForumSlug", new { id = forumId, slug = forumSlug }),
         });

      return Json(new
      {
         redirect = Url.RouteUrl("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) }),
      });

   }


   public virtual async Task<IActionResult> PostCreate(long id, int? quote)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(id);
      if (forumTopic == null)
         return RedirectToRoute("Boards");

      if (!await _forumService.IsUserAllowedToCreatePostAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
         return Challenge();

      var model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, quote, false);

      return View(model);
   }


   [HttpPost]
   [ValidateCaptcha]
   public virtual async Task<IActionResult> PostCreate(EditForumPostModel model, bool captchaValid)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumTopic = await _forumService.GetTopicByIdAsync(model.ForumTopicId);
      if (forumTopic == null)
         return RedirectToRoute("Boards");

      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      if (ModelState.IsValid)
         try
         {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _forumService.IsUserAllowedToCreatePostAsync(user, forumTopic))
               return Challenge();

            var text = model.Text;
            var maxPostLength = _forumSettings.PostMaxLength;
            if (maxPostLength > 0 && text.Length > maxPostLength)
               text = text[0..maxPostLength];

            var ipAddress = _webHelper.GetCurrentIpAddress();

            var nowUtc = DateTime.UtcNow;

            var forumPost = new ForumPost
            {
               ForumTopicId = forumTopic.Id,
               UserId = user.Id,
               Text = text,
               IPAddress = ipAddress,
               CreatedOnUtc = nowUtc,
               UpdatedOnUtc = nowUtc
            };
            await _forumService.InsertPostAsync(forumPost, true);

            //subscription
            if (await _forumService.IsUserAllowedToSubscribeAsync(user))
            {
               var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id,
                   0, forumPost.ForumTopicId, 0, 1)).FirstOrDefault();
               if (model.Subscribed)
                  if (forumSubscription == null)
                  {
                     forumSubscription = new ForumSubscription
                     {
                        SubscriptionGuid = Guid.NewGuid(),
                        UserId = user.Id,
                        TopicId = forumPost.ForumTopicId,
                        CreatedOnUtc = nowUtc
                     };

                     await _forumService.InsertSubscriptionAsync(forumSubscription);
                  }
                  else
                  if (forumSubscription != null)
                     await _forumService.DeleteSubscriptionAsync(forumSubscription);
            }

            var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

            var pageIndex = await _forumService.CalculateTopicPageIndexAsync(forumPost.ForumTopicId, pageSize, forumPost.Id) + 1;
            string url;
            if (pageIndex > 1)
               url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.ForumTopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
            else
               url = Url.RouteUrl("TopicSlug", new { id = forumPost.ForumTopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
            return LocalRedirect($"{url}#{forumPost.Id}");
         }
         catch (Exception ex)
         {
            ModelState.AddModelError("", ex.Message);
         }

      //redisplay form
      model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, 0, true);

      return View(model);
   }


   public virtual async Task<IActionResult> PostEdit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumPost = await _forumService.GetPostByIdAsync(id);
      if (forumPost == null)
         return RedirectToRoute("Boards");

      if (!await _forumService.IsUserAllowedToEditPostAsync(await _workContext.GetCurrentUserAsync(), forumPost))
         return Challenge();

      var model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, false);

      return View(model);
   }


   [HttpPost]
   [ValidateCaptcha]
   public virtual async Task<IActionResult> PostEdit(EditForumPostModel model, bool captchaValid)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var forumPost = await _forumService.GetPostByIdAsync(model.Id);
      if (forumPost == null)
         return RedirectToRoute("Boards");

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _forumService.IsUserAllowedToEditPostAsync(user, forumPost))
         return Challenge();

      var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.ForumTopicId);
      if (forumTopic == null)
         return RedirectToRoute("Boards");

      var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
      if (forum == null)
         return RedirectToRoute("Boards");

      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      if (ModelState.IsValid)
         try
         {
            var nowUtc = DateTime.UtcNow;

            var text = model.Text;
            var maxPostLength = _forumSettings.PostMaxLength;
            if (maxPostLength > 0 && text.Length > maxPostLength)
               text = text[0..maxPostLength];

            forumPost.UpdatedOnUtc = nowUtc;
            forumPost.Text = text;
            await _forumService.UpdatePostAsync(forumPost);

            //subscription
            if (await _forumService.IsUserAllowedToSubscribeAsync(user))
            {
               var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id,
                   0, forumPost.ForumTopicId, 0, 1)).FirstOrDefault();
               if (model.Subscribed)
                  if (forumSubscription == null)
                  {
                     forumSubscription = new ForumSubscription
                     {
                        SubscriptionGuid = Guid.NewGuid(),
                        UserId = user.Id,
                        TopicId = forumPost.ForumTopicId,
                        CreatedOnUtc = nowUtc
                     };
                     await _forumService.InsertSubscriptionAsync(forumSubscription);
                  }
                  else
                  if (forumSubscription != null)
                     await _forumService.DeleteSubscriptionAsync(forumSubscription);
            }

            var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
            var pageIndex = await _forumService.CalculateTopicPageIndexAsync(forumPost.ForumTopicId, pageSize, forumPost.Id) + 1;
            string url;
            if (pageIndex > 1)
               url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.ForumTopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
            else
               url = Url.RouteUrl("TopicSlug", new { id = forumPost.ForumTopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
            return LocalRedirect($"{url}#{forumPost.Id}");
         }
         catch (Exception ex)
         {
            ModelState.AddModelError("", ex.Message);
         }

      //redisplay form
      model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, true);

      return View(model);
   }


   public virtual async Task<IActionResult> Search(string searchterms, bool? advs, string forumId,
       string within, string limitDays, int pageNumber = 1)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.ForumsEnabled)
         return RedirectToRoute("Homepage");

      var model = await _forumModelFactory.PrepareSearchModelAsync(searchterms, advs, forumId, within, limitDays, pageNumber);

      return View(model);
   }


   public virtual async Task<IActionResult> UserForumSubscriptions(int? pageNumber)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.AllowUsersToManageSubscriptions)
         return RedirectToRoute("UserInfo");

      var model = await _forumModelFactory.PrepareUserForumSubscriptionsModelAsync(pageNumber);

      return View(model);
   }


   [HttpPost, ActionName("UserForumSubscriptions")]
   public virtual async Task<IActionResult> UserForumSubscriptionsPOST(IFormCollection formCollection)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      foreach (var key in formCollection.Keys)
      {
         var value = formCollection[key];

         if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
         {
            var id = key.Replace("fs", "").Trim();
            if (int.TryParse(id, out var forumSubscriptionId))
            {
               var forumSubscription = await _forumService.GetSubscriptionByIdAsync(forumSubscriptionId);
               var user = await _workContext.GetCurrentUserAsync();

               if (forumSubscription != null && forumSubscription.UserId == user.Id)
                  await _forumService.DeleteSubscriptionAsync(forumSubscription);
            }
         }
      }

      return RedirectToRoute("UserForumSubscriptions");
   }

   [HttpPost]
   public virtual async Task<IActionResult> PostVote(long postId, bool isUp)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessForum))
         return Challenge();

      if (!_forumSettings.AllowPostVoting)
         return new NullJsonResult();

      var forumPost = await _forumService.GetPostByIdAsync(postId);
      if (forumPost == null)
         return new NullJsonResult();

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsRegisteredAsync(user))
         return Json(new
         {
            Error = await _localizationService.GetResourceAsync("Forum.Votes.Login"),
            forumPost.VoteCount
         });

      if (user.Id == forumPost.UserId)
         return Json(new
         {
            Error = await _localizationService.GetResourceAsync("Forum.Votes.OwnPost"),
            forumPost.VoteCount
         });

      var forumPostVote = await _forumService.GetPostVoteAsync(postId, user);
      if (forumPostVote != null)
      {
         if (forumPostVote.IsUp && isUp || !forumPostVote.IsUp && !isUp)
            return Json(new
            {
               Error = await _localizationService.GetResourceAsync("Forum.Votes.AlreadyVoted"),
               forumPost.VoteCount
            });

         await _forumService.DeletePostVoteAsync(forumPostVote);
         return Json(new { forumPost.VoteCount });
      }

      if (await _forumService.GetNumberOfPostVotesAsync(user, DateTime.UtcNow.AddDays(-1)) >= _forumSettings.MaxVotesPerDay)
         return Json(new
         {
            Error = string.Format(await _localizationService.GetResourceAsync("Forum.Votes.MaxVotesReached"), _forumSettings.MaxVotesPerDay),
            forumPost.VoteCount
         });

      await _forumService.InsertPostVoteAsync(new ForumPostVote
      {
         UserId = user.Id,
         ForumPostId = postId,
         IsUp = isUp,
         CreatedOnUtc = DateTime.UtcNow
      });

      return Json(new { forumPost.VoteCount, IsUp = isUp });
   }

   #endregion
}