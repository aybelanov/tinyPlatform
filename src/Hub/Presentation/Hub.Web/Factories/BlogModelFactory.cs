using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Blogs;
using Hub.Services.Common;
using Hub.Services.Helpers;
using Hub.Services.Media;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Web.Infrastructure.Cache;
using Hub.Web.Models.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the blog model factory
/// </summary>
public partial class BlogModelFactory : IBlogModelFactory
{
   #region Fields

   private readonly BlogSettings _blogSettings;
   private readonly CaptchaSettings _captchaSettings;
   private readonly UserSettings _userSettings;
   private readonly IBlogService _blogService;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IPictureService _pictureService;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWorkContext _workContext;
   private readonly MediaSettings _mediaSettings;

   #endregion

   #region Ctor

   public BlogModelFactory(BlogSettings blogSettings,
       CaptchaSettings captchaSettings,
       UserSettings userSettings,
       IBlogService blogService,
       IUserService userService,
       IDateTimeHelper dateTimeHelper,
       IGenericAttributeService genericAttributeService,
       IPictureService pictureService,
       IStaticCacheManager staticCacheManager,
       IUrlRecordService urlRecordService,
       IWorkContext workContext,
       MediaSettings mediaSettings)
   {
      _blogSettings = blogSettings;
      _captchaSettings = captchaSettings;
      _userSettings = userSettings;
      _blogService = blogService;
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _genericAttributeService = genericAttributeService;
      _pictureService = pictureService;
      _staticCacheManager = staticCacheManager;
      _urlRecordService = urlRecordService;
      _workContext = workContext;
      _mediaSettings = mediaSettings;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare blog post model
   /// </summary>
   /// <param name="model">Blog post model</param>
   /// <param name="blogPost">Blog post entity</param>
   /// <param name="prepareComments">Whether to prepare blog comments</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool prepareComments)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      if (blogPost == null)
         throw new ArgumentNullException(nameof(blogPost));

      model.Id = blogPost.Id;
      model.MetaTitle = blogPost.MetaTitle;
      model.MetaDescription = blogPost.MetaDescription;
      model.MetaKeywords = blogPost.MetaKeywords;
      model.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
      model.Title = blogPost.Title;
      model.Body = blogPost.Body;
      model.BodyOverview = blogPost.BodyOverview;
      model.AllowComments = blogPost.AllowComments;

      model.PreventNotRegisteredUsersToLeaveComments =
          await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()) &&
          !_blogSettings.AllowNotRegisteredUsersToLeaveComments;

      model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc);
      model.Tags = await _blogService.ParseTagsAsync(blogPost);
      model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage;

      model.NumberOfComments = await _blogService.GetBlogCommentsCountAsync(blogPost, true);

      if (prepareComments)
      {
         var blogComments = await _blogService.GetAllCommentsAsync(
             blogPostId: blogPost.Id,
             approved: true);

         foreach (var bc in blogComments)
         {
            var commentModel = await PrepareBlogPostCommentModelAsync(bc);
            model.Comments.Add(commentModel);
         }
      }
   }

   /// <summary>
   /// Prepare blog post list model
   /// </summary>
   /// <param name="command">Blog paging filtering model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the blog post list model
   /// </returns>
   public virtual async Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command)
   {
      if (command == null)
         throw new ArgumentNullException(nameof(command));

      if (command.PageSize <= 0)
         command.PageSize = _blogSettings.PostsPageSize;
      if (command.PageNumber <= 0)
         command.PageNumber = 1;

      var dateFrom = command.GetFromMonth();
      var dateTo = command.GetToMonth();

      var language = await _workContext.GetWorkingLanguageAsync();
      var blogPosts = string.IsNullOrEmpty(command.Tag)
          ? await _blogService.GetAllBlogPostsAsync(language.Id, dateFrom, dateTo, command.PageNumber - 1, command.PageSize)
          : await _blogService.GetAllBlogPostsByTagAsync(language.Id, command.Tag, command.PageNumber - 1, command.PageSize);

      var model = new BlogPostListModel
      {
         PagingFilteringContext = { Tag = command.Tag, Month = command.Month },
         WorkingLanguageId = language.Id,
         BlogPosts = await blogPosts.SelectAwait(async blogPost =>
         {
            var blogPostModel = new BlogPostModel();
            await PrepareBlogPostModelAsync(blogPostModel, blogPost, false);
            return blogPostModel;
         }).ToListAsync()
      };
      model.PagingFilteringContext.LoadPagedList(blogPosts);

      return model;
   }

   /// <summary>
   /// Prepare blog post tag list model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the blog post tag list model
   /// </returns>
   public virtual async Task<BlogPostTagListModel> PrepareBlogPostTagListModelAsync()
   {
      var model = new BlogPostTagListModel();

      //get tags
      var tags = (await _blogService
          .GetAllBlogPostTagsAsync((await _workContext.GetWorkingLanguageAsync()).Id))
          .OrderByDescending(x => x.BlogPostCount)
          .Take(_blogSettings.NumberOfTags);

      //sorting and setting into the model
      model.Tags.AddRange(tags.OrderBy(x => x.Name).Select(tag => new BlogPostTagModel
      {
         Name = tag.Name,
         BlogPostCount = tag.BlogPostCount
      }));

      return model;
   }

   /// <summary>
   /// Prepare blog post year models
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of blog post year model
   /// </returns>
   public virtual async Task<List<BlogPostYearModel>> PrepareBlogPostYearModelAsync()
   {
      var currentLanguage = await _workContext.GetWorkingLanguageAsync();
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.BlogMonthsModelKey, currentLanguage);
      var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         var model = new List<BlogPostYearModel>();

         var blogPosts = await _blogService.GetAllBlogPostsAsync(currentLanguage.Id);
         if (blogPosts.Any())
         {
            var months = new SortedDictionary<DateTime, int>();

            var blogPost = blogPosts[blogPosts.Count - 1];
            var first = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
            while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
            {
               var list = await _blogService.GetPostsByDateAsync(blogPosts, new DateTime(first.Year, first.Month, 1),
                      new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
               if (list.Any())
               {
                  var date = new DateTime(first.Year, first.Month, 1);
                  months.Add(date, list.Count);
               }

               first = first.AddMonths(1);
            }

            var current = 0;
            foreach (var kvp in months)
            {
               var date = kvp.Key;
               var blogPostCount = kvp.Value;
               if (current == 0)
                  current = date.Year;

               if (date.Year > current || !model.Any())
               {
                  var yearModel = new BlogPostYearModel
                  {
                     Year = date.Year
                  };
                  model.Insert(0, yearModel);
               }

               model.First().Months.Insert(0, new BlogPostMonthModel
               {
                  Month = date.Month,
                  BlogPostCount = blogPostCount
               });

               current = date.Year;
            }
         }

         return model;
      });

      return cachedModel;
   }

   /// <summary>
   /// Prepare blog comment model
   /// </summary>
   /// <param name="blogComment">Blog comment entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the blog comment model
   /// </returns>
   public virtual async Task<BlogCommentModel> PrepareBlogPostCommentModelAsync(BlogComment blogComment)
   {
      if (blogComment == null)
         throw new ArgumentNullException(nameof(blogComment));

      var user = await _userService.GetUserByIdAsync(blogComment.UserId);

      var model = new BlogCommentModel
      {
         Id = blogComment.Id,
         UserId = blogComment.UserId,
         UserName = await _userService.FormatUsernameAsync(user),
         CommentText = blogComment.CommentText,
         CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc),
         AllowViewingProfiles = _userSettings.AllowViewingProfiles && user != null && !await _userService.IsGuestAsync(user)
      };

      if (_userSettings.AllowUsersToUploadAvatars)
         model.UserAvatarUrl = await _pictureService.GetPictureUrlAsync(
             //await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.AvatarPictureIdAttribute),
             user.AvatarPictureId,
             _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);

      return model;
   }

   #endregion
}