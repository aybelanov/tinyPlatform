using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Services.Common;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.News;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.News;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class NewsController : BaseAdminController
{
   #region Fields

   private readonly IUserActivityService _userActivityService;
   private readonly IEventPublisher _eventPublisher;
   private readonly ILocalizationService _localizationService;
   private readonly INewsModelFactory _newsModelFactory;
   private readonly INewsService _newsService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public NewsController(IUserActivityService userActivityService,
       IEventPublisher eventPublisher,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       IWorkContext workContext,
       INewsModelFactory newsModelFactory,
       INewsService newsService,
       INotificationService notificationService,
       IPermissionService permissionService,
       IUrlRecordService urlRecordService)
   {
      _userActivityService = userActivityService;
      _eventPublisher = eventPublisher;
      _localizationService = localizationService;
      _newsModelFactory = newsModelFactory;
      _newsService = newsService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _urlRecordService = urlRecordService;
      _workContext = workContext;
      _genericAttributeService = genericAttributeService;

   }

   #endregion

   #region Methods        

   #region News items

   public virtual IActionResult Index()
   {
      return RedirectToAction("NewsItems");
   }

   public virtual async Task<IActionResult> NewsItems(long? filterByNewsItemId, [FromQuery] bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //prepare model
      var model = await _newsModelFactory.PrepareNewsContentModelAsync(new NewsContentModel(), filterByNewsItemId);

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
   public virtual async Task<IActionResult> List(NewsItemSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _newsModelFactory.PrepareNewsItemListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> NewsItemCreate()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //prepare model
      var model = await _newsModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> NewsItemCreate(NewsItemModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var newsItem = model.ToEntity<NewsItem>();
         newsItem.CreatedOnUtc = DateTime.UtcNow;
         await _newsService.InsertNewsAsync(newsItem);

         //activity log
         await _userActivityService.InsertActivityAsync("AddNewNews",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

         //search engine name
         var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
         await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Added"));

         if (!continueEditing)
            return RedirectToAction("NewsItems");

         return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
      }

      //prepare model
      model = await _newsModelFactory.PrepareNewsItemModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> NewsItemEdit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //try to get a news item with the specified id
      var newsItem = await _newsService.GetNewsByIdAsync(id);
      if (newsItem == null)
         return RedirectToAction("NewsItems");

      //prepare model
      var model = await _newsModelFactory.PrepareNewsItemModelAsync(null, newsItem);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> NewsItemEdit(NewsItemModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //try to get a news item with the specified id
      var newsItem = await _newsService.GetNewsByIdAsync(model.Id);
      if (newsItem == null)
         return RedirectToAction("NewsItems");

      if (ModelState.IsValid)
      {
         newsItem = model.ToEntity(newsItem);
         await _newsService.UpdateNewsAsync(newsItem);

         //activity log
         await _userActivityService.InsertActivityAsync("EditNews",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNews"), newsItem.Id), newsItem);

         //search engine name
         var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
         await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Updated"));

         if (!continueEditing)
            return RedirectToAction("NewsItems");

         return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
      }

      //prepare model
      model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //try to get a news item with the specified id
      var newsItem = await _newsService.GetNewsByIdAsync(id);
      if (newsItem == null)
         return RedirectToAction("NewsItems");

      await _newsService.DeleteNewsAsync(newsItem);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteNews",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Deleted"));

      return RedirectToAction("NewsItems");
   }

   #endregion

   #region Comments

   public virtual async Task<IActionResult> NewsComments(long? filterByNewsItemId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //try to get a news item with the specified id
      var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
      if (newsItem == null && filterByNewsItemId.HasValue)
         return RedirectToAction("NewsComments");

      //prepare model
      var model = await _newsModelFactory.PrepareNewsCommentSearchModelAsync(new NewsCommentSearchModel(), newsItem);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Comments(NewsCommentSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _newsModelFactory.PrepareNewsCommentListModelAsync(searchModel, searchModel.NewsItemId);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> CommentUpdate(NewsCommentModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //try to get a news comment with the specified id
      var comment = await _newsService.GetNewsCommentByIdAsync(model.Id)
          ?? throw new ArgumentException("No comment found with the specified id");

      var previousIsApproved = comment.IsApproved;

      //fill entity from model
      comment = model.ToEntity(comment);

      await _newsService.UpdateNewsCommentAsync(comment);

      //activity log
      await _userActivityService.InsertActivityAsync("EditNewsComment",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), comment.Id), comment);

      //raise event (only if it wasn't approved before and is approved now)
      if (!previousIsApproved && comment.IsApproved)
         await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> CommentDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      //try to get a news comment with the specified id
      var comment = await _newsService.GetNewsCommentByIdAsync(id)
          ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

      await _newsService.DeleteNewsCommentAsync(comment);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteNewsComment",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), comment.Id), comment);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      var comments = await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray());

      await _newsService.DeleteNewsCommentsAsync(comments);

      //activity log
      foreach (var newsComment in comments)
         await _userActivityService.InsertActivityAsync("DeleteNewsComment",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), newsComment.Id), newsComment);

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> ApproveSelected(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      //filter not approved comments
      var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

      foreach (var newsComment in newsComments)
      {
         newsComment.IsApproved = true;

         await _newsService.UpdateNewsCommentAsync(newsComment);

         //raise event 
         await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(newsComment));

         //activity log
         await _userActivityService.InsertActivityAsync("EditNewsComment",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
      }

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> DisapproveSelected(ICollection<long> selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
         return AccessDeniedView();

      if (selectedIds == null || selectedIds.Count == 0)
         return NoContent();

      //filter approved comments
      var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved);

      foreach (var newsComment in newsComments)
      {
         newsComment.IsApproved = false;

         await _newsService.UpdateNewsCommentAsync(newsComment);

         //activity log
         await _userActivityService.InsertActivityAsync("EditNewsComment",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
      }

      return Json(new { Result = true });
   }

   #endregion

   #endregion
}