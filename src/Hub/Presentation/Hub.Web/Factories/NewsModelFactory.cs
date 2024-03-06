using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Common;
using Hub.Services.Helpers;
using Hub.Services.Media;
using Hub.Services.News;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Web.Infrastructure.Cache;
using Hub.Web.Models.News;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the news model factory
/// </summary>
public partial class NewsModelFactory : INewsModelFactory
{
   #region Fields

   private readonly CaptchaSettings _captchaSettings;
   private readonly UserSettings _userSettings;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly INewsService _newsService;
   private readonly IPictureService _pictureService;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWorkContext _workContext;
   private readonly MediaSettings _mediaSettings;
   private readonly NewsSettings _newsSettings;

   #endregion

   #region Ctor

   public NewsModelFactory(CaptchaSettings captchaSettings,
       UserSettings userSettings,
       IUserService userService,
       IDateTimeHelper dateTimeHelper,
       IGenericAttributeService genericAttributeService,
       INewsService newsService,
       IPictureService pictureService,
       IStaticCacheManager staticCacheManager,
       IUrlRecordService urlRecordService,
       IWorkContext workContext,
       MediaSettings mediaSettings,
       NewsSettings newsSettings)
   {
      _captchaSettings = captchaSettings;
      _userSettings = userSettings;
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _genericAttributeService = genericAttributeService;
      _newsService = newsService;
      _pictureService = pictureService;
      _staticCacheManager = staticCacheManager;
      _urlRecordService = urlRecordService;
      _workContext = workContext;
      _mediaSettings = mediaSettings;
      _newsSettings = newsSettings;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare the news item model
   /// </summary>
   /// <param name="model">News item model</param>
   /// <param name="newsItem">News item</param>
   /// <param name="prepareComments">Whether to prepare news comment models</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news item model
   /// </returns>
   public virtual async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool prepareComments)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      if (newsItem == null)
         throw new ArgumentNullException(nameof(newsItem));

      model.Id = newsItem.Id;
      model.MetaTitle = newsItem.MetaTitle;
      model.MetaDescription = newsItem.MetaDescription;
      model.MetaKeywords = newsItem.MetaKeywords;
      model.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
      model.Title = newsItem.Title;
      model.Short = newsItem.Short;
      model.Full = newsItem.Full;
      model.AllowComments = newsItem.AllowComments;

      model.PreventNotRegisteredUsersToLeaveComments =
          await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()) &&
          !_newsSettings.AllowNotRegisteredUsersToLeaveComments;

      model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
      model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;

      model.NumberOfComments = await _newsService.GetNewsCommentsCountAsync(newsItem, true);

      if (prepareComments)
      {
         var newsComments = await _newsService.GetAllCommentsAsync(newsItemId: newsItem.Id, approved: true);

         foreach (var nc in newsComments.OrderBy(comment => comment.CreatedOnUtc))
         {
            var commentModel = await PrepareNewsCommentModelAsync(nc);
            model.Comments.Add(commentModel);
         }
      }

      return model;
   }

   /// <summary>
   /// Prepare the home page news items model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the home page news items model
   /// </returns>
   public virtual async Task<HomepageNewsItemsModel> PrepareHomepageNewsItemsModelAsync()
   {
      var language = await _workContext.GetWorkingLanguageAsync();
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.HomepageNewsModelKey, language);
      var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         var newsItems = await _newsService.GetAllNewsAsync(language.Id, 0, _newsSettings.MainPageNewsCount);

         return new HomepageNewsItemsModel
         {
            WorkingLanguageId = language.Id,
            NewsItems = await newsItems.SelectAwait(async newsItem =>
               {
                  var newsModel = new NewsItemModel();
                  await PrepareNewsItemModelAsync(newsModel, newsItem, false);
                  return newsModel;
               }).ToListAsync()
         };
      });

      //"Comments" property of "NewsItemModel" object depends on the current user.
      //Furthermore, we just don't need it for home page news. So let's reset it.
      //But first we need to clone the cached model (the updated one should not be cached)
      var model = cachedModel with { };
      foreach (var newsItemModel in model.NewsItems)
         newsItemModel.Comments.Clear();

      return model;
   }

   /// <summary>
   /// Prepare the news item list model
   /// </summary>
   /// <param name="command">News paging filtering model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news item list model
   /// </returns>
   public virtual async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsPagingFilteringModel command)
   {
      if (command.PageSize <= 0)
         command.PageSize = _newsSettings.NewsArchivePageSize;
      if (command.PageNumber <= 0)
         command.PageNumber = 1;

      var language = await _workContext.GetWorkingLanguageAsync();
      var newsItems = await _newsService.GetAllNewsAsync(language.Id, command.PageNumber - 1, command.PageSize);

      var model = new NewsItemListModel
      {
         WorkingLanguageId = language.Id,
         NewsItems = await newsItems.SelectAwait(async newsItem =>
         {
            var newsModel = new NewsItemModel();
            await PrepareNewsItemModelAsync(newsModel, newsItem, false);
            return newsModel;
         }).ToListAsync()
      };
      model.PagingFilteringContext.LoadPagedList(newsItems);

      return model;
   }

   /// <summary>
   /// Prepare the news comment model
   /// </summary>
   /// <param name="newsComment">News comment</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news comment model
   /// </returns>
   public virtual async Task<NewsCommentModel> PrepareNewsCommentModelAsync(NewsComment newsComment)
   {
      if (newsComment == null)
         throw new ArgumentNullException(nameof(newsComment));

      var user = await _userService.GetUserByIdAsync(newsComment.UserId);

      var model = new NewsCommentModel
      {
         Id = newsComment.Id,
         UserId = newsComment.UserId,
         UserName = await _userService.FormatUsernameAsync(user),
         CommentTitle = newsComment.CommentTitle,
         CommentText = newsComment.CommentText,
         CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc),
         AllowViewingProfiles = _userSettings.AllowViewingProfiles && newsComment.UserId != 0 && !await _userService.IsGuestAsync(user),
      };

      if (_userSettings.AllowUsersToUploadAvatars)
         model.UserAvatarUrl = await _pictureService.GetPictureUrlAsync(
             //await _genericAttributeService.GetAttributeAsync<User, int>(newsComment.UserId, AppUserDefaults.AvatarPictureIdAttribute),
             user.AvatarPictureId,
             _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);

      return model;
   }

   #endregion
}