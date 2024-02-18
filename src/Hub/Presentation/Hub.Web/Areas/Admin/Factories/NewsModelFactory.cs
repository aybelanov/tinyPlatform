using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Helpers;
using Hub.Services.Html;
using Hub.Services.Localization;
using Hub.Services.News;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.News;
using Hub.Web.Framework.Extensions;
using Hub.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the news model factory implementation
/// </summary>
public partial class NewsModelFactory : INewsModelFactory
{
   #region Fields

   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IHtmlFormatter _htmlFormatter;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly INewsService _newsService;
   private readonly IUrlRecordService _urlRecordService;

   #endregion

   #region Ctor

   public NewsModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
       IUserService userService,
       IDateTimeHelper dateTimeHelper,
       IHtmlFormatter htmlFormatter,
       ILanguageService languageService,
       ILocalizationService localizationService,
       INewsService newsService,
       IUrlRecordService urlRecordService)
   {
      _userService = userService;
      _baseAdminModelFactory = baseAdminModelFactory;
      _dateTimeHelper = dateTimeHelper;
      _htmlFormatter = htmlFormatter;
      _languageService = languageService;
      _localizationService = localizationService;
      _newsService = newsService;
      _urlRecordService = urlRecordService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare news content model
   /// </summary>
   /// <param name="newsContentModel">News content model</param>
   /// <param name="filterByNewsItemId">Filter by news item ID</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news content model
   /// </returns>
   public virtual async Task<NewsContentModel> PrepareNewsContentModelAsync(NewsContentModel newsContentModel, long? filterByNewsItemId)
   {
      if (newsContentModel == null)
         throw new ArgumentNullException(nameof(newsContentModel));

      //prepare nested search models
      await PrepareNewsItemSearchModelAsync(newsContentModel.NewsItems);
      var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
      await PrepareNewsCommentSearchModelAsync(newsContentModel.NewsComments, newsItem);

      return newsContentModel;
   }

   /// <summary>
   /// Prepare paged news item list model
   /// </summary>
   /// <param name="searchModel">News item search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news item list model
   /// </returns>
   public virtual async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsItemSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get news items
      var newsItems = await _newsService.GetAllNewsAsync(showHidden: true, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, title: searchModel.SearchTitle);

      //prepare list model
      var model = await new NewsItemListModel().PrepareToGridAsync(searchModel, newsItems, () =>
      {
         return newsItems.SelectAwait(async newsItem =>
            {
               //fill in model values from the entity
               var newsItemModel = newsItem.ToModel<NewsItemModel>();

               //little performance optimization: ensure that "Full" is not returned
               newsItemModel.Full = string.Empty;

               //convert dates to the user time
               if (newsItem.StartDateUtc.HasValue)
                  newsItemModel.StartDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc.Value, DateTimeKind.Utc);
               if (newsItem.EndDateUtc.HasValue)
                  newsItemModel.EndDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.EndDateUtc.Value, DateTimeKind.Utc);
               newsItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.CreatedOnUtc, DateTimeKind.Utc);

               //fill in additional values (not existing in the entity)
               newsItemModel.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
               newsItemModel.LanguageName = (await _languageService.GetLanguageByIdAsync(newsItem.LanguageId))?.Name;
               newsItemModel.ApprovedComments = await _newsService.GetNewsCommentsCountAsync(newsItem, isApproved: true);
               newsItemModel.NotApprovedComments = await _newsService.GetNewsCommentsCountAsync(newsItem, isApproved: false);

               return newsItemModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare news item model
   /// </summary>
   /// <param name="model">News item model</param>
   /// <param name="newsItem">News item</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news item model
   /// </returns>
   public virtual async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool excludeProperties = false)
   {
      //fill in model values from the entity
      if (newsItem != null)
      {
         if (model == null)
         {
            model = newsItem.ToModel<NewsItemModel>();
            model.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
         }

         model.StartDateUtc = newsItem.StartDateUtc;
         model.EndDateUtc = newsItem.EndDateUtc;
      }

      //set default values for the new model
      if (newsItem == null)
      {
         model.Published = true;
         model.AllowComments = true;
      }

      //prepare available languages
      await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

      // TODO nake news implementation to show for all languages
      //model.AvailableLanguages.Add(new() { Text = "For all languages", Value = "0" });

      return model;
   }

   /// <summary>
   /// Prepare news comment search model
   /// </summary>
   /// <param name="searchModel">News comment search model</param>
   /// <param name="newsItem">News item</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news comment search model
   /// </returns>
   public virtual async Task<NewsCommentSearchModel> PrepareNewsCommentSearchModelAsync(NewsCommentSearchModel searchModel, NewsItem newsItem)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
      searchModel.AvailableApprovedOptions.Add(new SelectListItem
      {
         Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.All"),
         Value = "0"
      });
      searchModel.AvailableApprovedOptions.Add(new SelectListItem
      {
         Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly"),
         Value = "1"
      });
      searchModel.AvailableApprovedOptions.Add(new SelectListItem
      {
         Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly"),
         Value = "2"
      });

      searchModel.NewsItemId = newsItem?.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged news comment list model
   /// </summary>
   /// <param name="searchModel">News comment search model</param>
   /// <param name="newsItemId">News item Id; pass null to prepare comment models for all news items</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news comment list model
   /// </returns>
   public virtual async Task<NewsCommentListModel> PrepareNewsCommentListModelAsync(NewsCommentSearchModel searchModel, long? newsItemId)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get parameters to filter comments
      var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
      var createdOnToValue = searchModel.CreatedOnTo == null ? null
          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
      var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

      //get comments
      var comments = (await _newsService.GetAllCommentsAsync(newsItemId: newsItemId,
          approved: isApprovedOnly,
          fromUtc: createdOnFromValue,
          toUtc: createdOnToValue,
          commentText: searchModel.SearchText)).ToPagedList(searchModel);

      //prepare list model
      var model = await new NewsCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
      {
         return comments.SelectAwait(async newsComment =>
            {
               //fill in model values from the entity
               var commentModel = newsComment.ToModel<NewsCommentModel>();

               //convert dates to the user time
               commentModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc);

               //fill in additional values (not existing in the entity)
               commentModel.NewsItemTitle = (await _newsService.GetNewsByIdAsync(newsComment.NewsItemId))?.Title;

               if (await _userService.GetUserByIdAsync(newsComment.UserId) is User user)
                  commentModel.UserInfo = await _userService.IsRegisteredAsync(user)
                         ? user.Email
                         : await _localizationService.GetResourceAsync("Admin.Users.Guest");

               commentModel.CommentText = _htmlFormatter.FormatText(newsComment.CommentText, false, true, false, false, false, false);

               return commentModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare news item search model
   /// </summary>
   /// <param name="searchModel">News item search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news item search model
   /// </returns>
   public virtual Task<NewsItemSearchModel> PrepareNewsItemSearchModelAsync(NewsItemSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   #endregion
}