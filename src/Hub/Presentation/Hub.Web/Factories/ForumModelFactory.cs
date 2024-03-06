using Hub.Core;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Services.Forums;
using Hub.Services.Helpers;
using Hub.Services.Html;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.Users;
using Hub.Web.Framework.Extensions;
using Hub.Web.Models.Boards;
using Hub.Web.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the forum model factory
/// </summary>
public partial class ForumModelFactory : IForumModelFactory
{
   #region Fields

   private readonly CaptchaSettings _captchaSettings;
   private readonly UserSettings _userSettings;
   private readonly ForumSettings _forumSettings;
   private readonly IBBCodeHelper _bbCodeHelper;
   private readonly ICountryService _countryService;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IForumService _forumService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly ILocalizationService _localizationService;
   private readonly IPictureService _pictureService;
   private readonly IWorkContext _workContext;
   private readonly MediaSettings _mediaSettings;

   #endregion

   #region Ctor

   public ForumModelFactory(CaptchaSettings captchaSettings,
       UserSettings userSettings,
       ForumSettings forumSettings,
       IBBCodeHelper bbCodeHelper,
       ICountryService countryService,
       IUserService userService,
       IDateTimeHelper dateTimeHelper,
       IForumService forumService,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       IPictureService pictureService,
       IWorkContext workContext,
       MediaSettings mediaSettings)
   {
      _captchaSettings = captchaSettings;
      _userSettings = userSettings;
      _forumSettings = forumSettings;
      _bbCodeHelper = bbCodeHelper;
      _countryService = countryService;
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _forumService = forumService;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _pictureService = pictureService;
      _workContext = workContext;
      _mediaSettings = mediaSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Get the list of forum topic types
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the collection of the select list item
   /// </returns>
   protected virtual async Task<IEnumerable<SelectListItem>> ForumTopicTypesListAsync()
   {
      var list = new List<SelectListItem>
         {
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Normal"),
                 Value = ((int)ForumTopicType.Normal).ToString()
             },

             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Sticky"),
                 Value = ((int)ForumTopicType.Sticky).ToString()
             },

             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Announcement"),
                 Value = ((int)ForumTopicType.Announcement).ToString()
             }
         };

      return list;
   }

   /// <summary>
   /// Get the list of forum groups
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the collection of the select list item
   /// </returns>
   protected virtual async Task<IEnumerable<SelectListItem>> ForumGroupsForumsListAsync()
   {
      var forumsList = new List<SelectListItem>();
      var separator = "--";
      var forumGroups = await _forumService.GetAllForumGroupsAsync();

      foreach (var fg in forumGroups)
      {
         // Add the forum group with Value of 0 so it won't be used as a target forum
         forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

         var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
         foreach (var f in forums)
            forumsList.Add(new SelectListItem { Text = $"{separator}{f.Name}", Value = f.Id.ToString() });
      }

      return forumsList;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare the forum group model
   /// </summary>
   /// <param name="forumGroup">Forum group</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum group model
   /// </returns>
   public virtual async Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroup forumGroup)
   {
      if (forumGroup == null)
         throw new ArgumentNullException(nameof(forumGroup));

      var forumGroupModel = new ForumGroupModel
      {
         Id = forumGroup.Id,
         Name = forumGroup.Name,
         SeName = await _forumService.GetForumGroupSeNameAsync(forumGroup),
      };
      var forums = await _forumService.GetAllForumsByGroupIdAsync(forumGroup.Id);
      foreach (var forum in forums)
      {
         var forumModel = await PrepareForumRowModelAsync(forum);
         forumGroupModel.Forums.Add(forumModel);
      }

      return forumGroupModel;
   }

   /// <summary>
   /// Prepare the boards index model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the boards index model
   /// </returns>
   public virtual async Task<BoardsIndexModel> PrepareBoardsIndexModelAsync()
   {
      var model = new BoardsIndexModel();

      var forumGroups = await _forumService.GetAllForumGroupsAsync();
      foreach (var forumGroup in forumGroups)
      {
         var forumGroupModel = await PrepareForumGroupModelAsync(forumGroup);
         model.ForumGroups.Add(forumGroupModel);
      }
      return model;
   }

   /// <summary>
   /// Prepare the active discussions model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the active discussions model
   /// </returns>
   public virtual async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync()
   {
      var model = new ActiveDiscussionsModel
      {
         ViewAllLinkEnabled = true,
         ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
         PostsPageSize = _forumSettings.PostsPageSize,
         AllowPostVoting = _forumSettings.AllowPostVoting
      };

      var topics = await _forumService.GetActiveTopicsAsync(0, 0, _forumSettings.HomepageActiveDiscussionsTopicCount);
      foreach (var topic in topics)
      {
         var topicModel = await PrepareForumTopicRowModelAsync(topic);
         model.ForumTopics.Add(topicModel);
      }

      return model;
   }

   /// <summary>
   /// Prepare the active discussions model
   /// </summary>
   /// <param name="forumId">Forum identifier</param>
   /// <param name="page">Number of forum topics page</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the active discussions model
   /// </returns>
   public virtual async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync(long forumId, int page)
   {
      var model = new ActiveDiscussionsModel
      {
         ViewAllLinkEnabled = false,
         ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
         PostsPageSize = _forumSettings.PostsPageSize,
         AllowPostVoting = _forumSettings.AllowPostVoting
      };

      var pageSize = _forumSettings.ActiveDiscussionsPageSize > 0 ? _forumSettings.ActiveDiscussionsPageSize : 50;

      var topics = await _forumService.GetActiveTopicsAsync(forumId, page - 1, pageSize);
      model.TopicPageSize = topics.PageSize;
      model.TopicTotalRecords = topics.TotalCount;
      model.TopicPageIndex = topics.PageIndex;
      foreach (var topic in topics)
      {
         var topicModel = await PrepareForumTopicRowModelAsync(topic);
         model.ForumTopics.Add(topicModel);
      }

      return model;
   }

   /// <summary>
   /// Prepare the forum page model
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <param name="page">Number of forum topics page</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum page model
   /// </returns>
   public virtual async Task<ForumPageModel> PrepareForumPageModelAsync(Forum forum, int page)
   {
      if (forum == null)
         throw new ArgumentNullException(nameof(forum));

      var model = new ForumPageModel
      {
         Id = forum.Id,
         Name = forum.Name,
         SeName = await _forumService.GetForumSeNameAsync(forum),
         Description = forum.Description
      };

      var pageSize = _forumSettings.TopicsPageSize > 0 ? _forumSettings.TopicsPageSize : 10;

      model.AllowPostVoting = _forumSettings.AllowPostVoting;

      //subscription
      var user = await _workContext.GetCurrentUserAsync();
      if (await _forumService.IsUserAllowedToSubscribeAsync(user))
      {
         model.WatchForumText = await _localizationService.GetResourceAsync("Forum.WatchForum");

         var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id, forum.Id, 0, 0, 1)).FirstOrDefault();
         if (forumSubscription != null)
            model.WatchForumText = await _localizationService.GetResourceAsync("Forum.UnwatchForum");
      }

      var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty, ForumSearchType.All, 0, page - 1, pageSize);
      model.TopicPageSize = topics.PageSize;
      model.TopicTotalRecords = topics.TotalCount;
      model.TopicPageIndex = topics.PageIndex;
      foreach (var topic in topics)
      {
         var topicModel = await PrepareForumTopicRowModelAsync(topic);
         model.ForumTopics.Add(topicModel);
      }
      model.IsUserAllowedToSubscribe = await _forumService.IsUserAllowedToSubscribeAsync(user);
      model.ForumFeedsEnabled = _forumSettings.ForumFeedsEnabled;
      model.PostsPageSize = _forumSettings.PostsPageSize;
      return model;
   }

   /// <summary>
   /// Prepare the forum topic page model
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <param name="page">Number of forum posts page</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum topic page model
   /// </returns>
   public virtual async Task<ForumTopicPageModel> PrepareForumTopicPageModelAsync(ForumTopic forumTopic, int page)
   {
      if (forumTopic == null)
         throw new ArgumentNullException(nameof(forumTopic));

      //load posts
      var posts = await _forumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty,
          page - 1, _forumSettings.PostsPageSize);

      //prepare model
      var currentUser = await _workContext.GetCurrentUserAsync();
      var model = new ForumTopicPageModel
      {
         Id = forumTopic.Id,
         Subject = forumTopic.Subject,
         SeName = await _forumService.GetTopicSeNameAsync(forumTopic),

         IsUserAllowedToEditTopic = await _forumService.IsUserAllowedToEditTopicAsync(currentUser, forumTopic),
         IsUserAllowedToDeleteTopic = await _forumService.IsUserAllowedToDeleteTopicAsync(currentUser, forumTopic),
         IsUserAllowedToMoveTopic = await _forumService.IsUserAllowedToMoveTopicAsync(currentUser, forumTopic),
         IsUserAllowedToSubscribe = await _forumService.IsUserAllowedToSubscribeAsync(currentUser)
      };

      if (model.IsUserAllowedToSubscribe)
      {
         model.WatchTopicText = await _localizationService.GetResourceAsync("Forum.WatchTopic");

         var forumTopicSubscription = (await _forumService.GetAllSubscriptionsAsync(currentUser.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
         if (forumTopicSubscription != null)
            model.WatchTopicText = await _localizationService.GetResourceAsync("Forum.UnwatchTopic");
      }
      model.PostsPageIndex = posts.PageIndex;
      model.PostsPageSize = posts.PageSize;
      model.PostsTotalRecords = posts.TotalCount;
      foreach (var post in posts)
      {
         var user = await _userService.GetUserByIdAsync(post.UserId);

         var userIsGuest = await _userService.IsGuestAsync(user);
         var userIsModerator = !userIsGuest && await _userService.IsForumModeratorAsync(user);

         var forumPostModel = new ForumPostModel
         {
            Id = post.Id,
            ForumTopicId = post.ForumTopicId,
            ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
            FormattedText = _forumService.FormatPostText(post),
            IsCurrentUserAllowedToEditPost = await _forumService.IsUserAllowedToEditPostAsync(currentUser, post),
            IsCurrentUserAllowedToDeletePost = await _forumService.IsUserAllowedToDeletePostAsync(currentUser, post),
            UserId = post.UserId,
            AllowViewingProfiles = _userSettings.AllowViewingProfiles && !userIsGuest,
            UserName = await _userService.FormatUsernameAsync(user),
            IsUserForumModerator = userIsModerator,
            ShowUsersPostCount = _forumSettings.ShowUsersPostCount,
            ForumPostCount = await _genericAttributeService.GetAttributeAsync<User, int>(post.UserId, AppUserDefaults.ForumPostCountAttribute),
            ShowUsersJoinDate = _userSettings.ShowUsersJoinDate && !userIsGuest,
            UserJoinDate = user?.CreatedOnUtc ?? DateTime.Now,
            AllowPrivateMessages = _forumSettings.AllowPrivateMessages && !userIsGuest,
            SignaturesEnabled = _forumSettings.SignaturesEnabled,
            FormattedSignature = _forumService.FormatForumSignatureText(await _genericAttributeService.GetAttributeAsync<User, string>(post.UserId, AppUserDefaults.SignatureAttribute)),
         };
         //created on string
         var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
         if (_forumSettings.RelativeDateTimeFormattingEnabled)
         {
            var postCreatedAgo = post.CreatedOnUtc.RelativeFormat(languageCode);
            forumPostModel.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
         }
         else
            forumPostModel.PostCreatedOnStr =
                (await _dateTimeHelper.ConvertToUserTimeAsync(post.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
         //avatar
         if (_userSettings.AllowUsersToUploadAvatars)
            forumPostModel.UserAvatarUrl = await _pictureService.GetPictureUrlAsync(
                //await _genericAttributeService.GetAttributeAsync<User, int>(post.UserId, AppUserDefaults.AvatarPictureIdAttribute),
                user.AvatarPictureId,
                _mediaSettings.AvatarPictureSize,
                _userSettings.DefaultAvatarEnabled,
                defaultPictureType: PictureType.Avatar);
         //location
         forumPostModel.ShowUsersLocation = _userSettings.ShowUsersLocation && !userIsGuest;
         if (_userSettings.ShowUsersLocation)
         {
            var countryId = await _genericAttributeService.GetAttributeAsync<User, int>(post.UserId, AppUserDefaults.CountryIdAttribute);
            var country = await _countryService.GetCountryByIdAsync(countryId);
            forumPostModel.UserLocation = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name) : string.Empty;
         }

         //votes
         if (_forumSettings.AllowPostVoting)
         {
            forumPostModel.AllowPostVoting = true;
            forumPostModel.VoteCount = post.VoteCount;
            var postVote = await _forumService.GetPostVoteAsync(post.Id, currentUser);
            if (postVote != null)
               forumPostModel.VoteIsUp = postVote.IsUp;
         }

         // page number is needed for creating post link in _ForumPost partial view
         forumPostModel.CurrentTopicPage = page;
         model.ForumPostModels.Add(forumPostModel);
      }

      return model;
   }

   /// <summary>
   /// Prepare the topic move model
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic move model
   /// </returns>
   public virtual async Task<TopicMoveModel> PrepareTopicMoveAsync(ForumTopic forumTopic)
   {
      if (forumTopic == null)
         throw new ArgumentNullException(nameof(forumTopic));

      var model = new TopicMoveModel
      {
         ForumList = await ForumGroupsForumsListAsync(),
         Id = forumTopic.Id,
         TopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
         ForumSelected = forumTopic.ForumId
      };

      return model;
   }

   /// <summary>
   /// Prepare the forum topic create model
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <param name="model">Edit forum topic model</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task PrepareTopicCreateModelAsync(Forum forum, EditForumTopicModel model)
   {
      if (forum == null)
         throw new ArgumentNullException(nameof(forum));

      if (model == null)
         throw new ArgumentNullException(nameof(model));

      var user = await _workContext.GetCurrentUserAsync();
      model.IsEdit = false;
      model.ForumId = forum.Id;
      model.ForumName = forum.Name;
      model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
      model.ForumEditor = _forumSettings.ForumEditor;
      model.IsUserAllowedToSetTopicPriority = await _forumService.IsUserAllowedToSetTopicPriorityAsync(user);
      model.TopicPriorities = await ForumTopicTypesListAsync();
      model.IsUserAllowedToSubscribe = await _forumService.IsUserAllowedToSubscribeAsync(user);
      model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum;
   }

   /// <summary>
   /// Prepare the forum topic edit model
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <param name="model">Edit forum topic model</param>
   /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task PrepareTopicEditModelAsync(ForumTopic forumTopic, EditForumTopicModel model, bool excludeProperties)
   {
      if (forumTopic == null)
         throw new ArgumentNullException(nameof(forumTopic));

      if (model == null)
         throw new ArgumentNullException(nameof(model));

      var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
      if (forum == null)
         throw new ArgumentException("forum cannot be loaded");

      var user = await _workContext.GetCurrentUserAsync();
      model.IsEdit = true;
      model.Id = forumTopic.Id;
      model.TopicPriorities = await ForumTopicTypesListAsync();
      model.ForumName = forum.Name;
      model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
      model.ForumId = forum.Id;
      model.ForumEditor = _forumSettings.ForumEditor;
      model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum;
      model.IsUserAllowedToSetTopicPriority = await _forumService.IsUserAllowedToSetTopicPriorityAsync(user);
      model.IsUserAllowedToSubscribe = await _forumService.IsUserAllowedToSubscribeAsync(user);

      if (!excludeProperties)
      {
         var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
         model.Text = firstPost.Text;
         model.Subject = forumTopic.Subject;
         model.TopicTypeId = forumTopic.TopicTypeId;
         //subscription            
         if (model.IsUserAllowedToSubscribe)
         {
            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
            model.Subscribed = forumSubscription != null;
         }
      }
   }

   /// <summary>
   /// Prepare the forum post create model
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <param name="quote">Identifier of the quoted post; pass null to load the empty text</param>
   /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the edit forum post model
   /// </returns>
   public virtual async Task<EditForumPostModel> PreparePostCreateModelAsync(ForumTopic forumTopic, int? quote, bool excludeProperties)
   {
      if (forumTopic == null)
         throw new ArgumentNullException(nameof(forumTopic));

      var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
      if (forum == null)
         throw new ArgumentException("forum cannot be loaded");

      var currentUser = await _workContext.GetCurrentUserAsync();
      var model = new EditForumPostModel
      {
         ForumTopicId = forumTopic.Id,
         IsEdit = false,
         ForumEditor = _forumSettings.ForumEditor,
         ForumName = forum.Name,
         ForumTopicSubject = forumTopic.Subject,
         ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
         IsUserAllowedToSubscribe = await _forumService.IsUserAllowedToSubscribeAsync(currentUser),
         DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum
      };

      if (!excludeProperties)
      {
         //subscription            
         if (model.IsUserAllowedToSubscribe)
         {
            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(currentUser.Id,
                0, forumTopic.Id, 0, 1)).FirstOrDefault();
            model.Subscribed = forumSubscription != null;
         }

         // Insert the quoted text
         var text = string.Empty;
         if (quote.HasValue)
         {
            var quotePost = await _forumService.GetPostByIdAsync(quote.Value);

            if (quotePost != null && quotePost.ForumTopicId == forumTopic.Id)
            {
               var user = await _userService.GetUserByIdAsync(quotePost.UserId);

               var quotePostText = quotePost.Text;

               switch (_forumSettings.ForumEditor)
               {
                  case EditorType.SimpleTextBox:
                     text = $"{await _userService.FormatUsernameAsync(user)}:\n{quotePostText}\n";
                     break;
                  case EditorType.BBCodeEditor:
                     text = $"[quote={await _userService.FormatUsernameAsync(user)}]{_bbCodeHelper.RemoveQuotes(quotePostText)}[/quote]";
                     break;
               }
               model.Text = text;
            }
         }
      }

      return model;
   }

   /// <summary>
   /// Prepare the forum post edit model
   /// </summary>
   /// <param name="forumPost">Forum post</param>
   /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the edit forum post model
   /// </returns>
   public virtual async Task<EditForumPostModel> PreparePostEditModelAsync(ForumPost forumPost, bool excludeProperties)
   {
      if (forumPost == null)
         throw new ArgumentNullException(nameof(forumPost));

      var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.ForumTopicId);
      if (forumTopic == null)
         throw new ArgumentException("forum topic cannot be loaded");

      var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
      if (forum == null)
         throw new ArgumentException("forum cannot be loaded");

      var user = await _workContext.GetCurrentUserAsync();
      var model = new EditForumPostModel
      {
         Id = forumPost.Id,
         ForumTopicId = forumTopic.Id,
         IsEdit = true,
         ForumEditor = _forumSettings.ForumEditor,
         ForumName = forum.Name,
         ForumTopicSubject = forumTopic.Subject,
         ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
         IsUserAllowedToSubscribe = await _forumService.IsUserAllowedToSubscribeAsync(user),
         DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum
      };

      if (!excludeProperties)
      {
         model.Text = forumPost.Text;
         //subscription
         if (model.IsUserAllowedToSubscribe)
         {
            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(user.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
            model.Subscribed = forumSubscription != null;
         }
      }

      return model;
   }

   /// <summary>
   /// Prepare the search model
   /// </summary>
   /// <param name="searchterms">Search terms</param>
   /// <param name="advs">Whether to use the advanced search</param>
   /// <param name="forumId">Forum identifier</param>
   /// <param name="within">String representation of int value of ForumSearchType</param>
   /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
   /// <param name="page">Number of items page</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the search model
   /// </returns>
   public virtual async Task<SearchModel> PrepareSearchModelAsync(string searchterms, bool? advs, string forumId, string within, string limitDays, int page)
   {
      var model = new SearchModel();

      var pageSize = 10;

      // Create the values for the "Limit results to previous" select list
      var limitList = new List<SelectListItem>
         {
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.AllResults"),
                 Value = "0"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1day"),
                 Value = "1"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.7days"),
                 Value = "7"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.2weeks"),
                 Value = "14"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1month"),
                 Value = "30"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.3months"),
                 Value = "92"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.6months"),
                 Value = "183"
             },
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1year"),
                 Value = "365"
             }
         };
      model.LimitList = limitList;

      // Create the values for the "Search in forum" select list
      var forumsSelectList = new List<SelectListItem>
         {
             new SelectListItem
             {
                 Text = await _localizationService.GetResourceAsync("Forum.Search.SearchInForum.All"),
                 Value = "0",
                 Selected = true,
             }
         };

      var separator = "--";
      var forumGroups = await _forumService.GetAllForumGroupsAsync();
      foreach (var fg in forumGroups)
      {
         // Add the forum group with value as '-' so it can't be used as a target forum id
         forumsSelectList.Add(new SelectListItem { Text = fg.Name, Value = "-" });

         var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
         foreach (var f in forums)
            forumsSelectList.Add(
                new SelectListItem
                {
                   Text = $"{separator}{f.Name}",
                   Value = f.Id.ToString()
                });
      }
      model.ForumList = forumsSelectList;

      // Create the values for "Search within" select list            
      var withinList = new List<SelectListItem>
         {
             new SelectListItem
             {
                 Value = ((int) ForumSearchType.All).ToString(),
                 Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.All")
             },
             new SelectListItem
             {
                 Value = ((int) ForumSearchType.TopicTitlesOnly).ToString(),
                 Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.TopicTitlesOnly")
             },
             new SelectListItem
             {
                 Value = ((int) ForumSearchType.PostTextOnly).ToString(),
                 Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.PostTextOnly")
             }
         };
      model.WithinList = withinList;

      _ = int.TryParse(forumId, out var forumIdSelected);
      model.ForumIdSelected = forumIdSelected;

      _ = int.TryParse(within, out var withinSelected);
      model.WithinSelected = withinSelected;

      _ = int.TryParse(limitDays, out var limitDaysSelected);
      model.LimitDaysSelected = limitDaysSelected;

      var searchTermMinimumLength = _forumSettings.ForumSearchTermMinimumLength;

      model.ShowAdvancedSearch = advs.GetValueOrDefault();
      model.SearchResultsVisible = false;
      model.NoResultsVisisble = false;
      model.PostsPageSize = _forumSettings.PostsPageSize;

      model.AllowPostVoting = _forumSettings.AllowPostVoting;

      try
      {
         if (!string.IsNullOrWhiteSpace(searchterms))
         {
            searchterms = searchterms.Trim();
            model.SearchTerms = searchterms;

            if (searchterms.Length < searchTermMinimumLength)
               throw new AppException(string.Format(await _localizationService.GetResourceAsync("Forum.SearchTermMinimumLengthIsNCharacters"),
                   searchTermMinimumLength));

            ForumSearchType searchWithin = 0;
            var limitResultsToPrevious = 0;
            if (advs.GetValueOrDefault())
            {
               searchWithin = (ForumSearchType)withinSelected;
               limitResultsToPrevious = limitDaysSelected;
            }

            if (_forumSettings.SearchResultsPageSize > 0)
               pageSize = _forumSettings.SearchResultsPageSize;

            var topics = await _forumService.GetAllTopicsAsync(forumIdSelected, 0, searchterms, searchWithin,
                limitResultsToPrevious, page - 1, pageSize);
            model.TopicPageSize = topics.PageSize;
            model.TopicTotalRecords = topics.TotalCount;
            model.TopicPageIndex = topics.PageIndex;
            foreach (var topic in topics)
            {
               var topicModel = await PrepareForumTopicRowModelAsync(topic);
               model.ForumTopics.Add(topicModel);
            }

            model.SearchResultsVisible = topics.Any();
            model.NoResultsVisisble = !model.SearchResultsVisible;

            return model;
         }
         model.SearchResultsVisible = false;
      }
      catch (Exception ex)
      {
         model.Error = ex.Message;
      }

      //some exception raised
      model.TopicPageSize = pageSize;
      model.TopicTotalRecords = 0;
      model.TopicPageIndex = page - 1;

      return model;
   }

   /// <summary>
   /// Prepare the last post model
   /// </summary>
   /// <param name="forumPost">Forum post</param>
   /// <param name="showTopic">Whether to show topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the last post model
   /// </returns>
   public virtual async Task<LastPostModel> PrepareLastPostModelAsync(ForumPost forumPost, bool showTopic)
   {
      var model = new LastPostModel
      {
         ShowTopic = showTopic
      };

      //do not throw an exception here
      if (forumPost == null)
         return model;

      var topic = await _forumService.GetTopicByIdAsync(forumPost.ForumTopicId);

      if (topic is null)
         return model;

      var user = await _userService.GetUserByIdAsync(forumPost.UserId);

      model.Id = forumPost.Id;
      model.ForumTopicId = topic.Id;
      model.ForumTopicSeName = await _forumService.GetTopicSeNameAsync(topic);
      model.ForumTopicSubject = _forumService.StripTopicSubject(topic);
      model.UserId = forumPost.UserId;
      model.AllowViewingProfiles = _userSettings.AllowViewingProfiles && !await _userService.IsGuestAsync(user);
      model.UserName = await _userService.FormatUsernameAsync(user);
      //created on string
      var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
      if (_forumSettings.RelativeDateTimeFormattingEnabled)
      {
         var postCreatedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
         model.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
      }
      else
         model.PostCreatedOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");

      return model;
   }

   /// <summary>
   /// Prepare the forum breadcrumb model
   /// </summary>
   /// <param name="forumGroupId">Forum group identifier; pass null to load nothing</param>
   /// <param name="forumId">Forum identifier; pass null to load breadcrumbs up to forum group</param>
   /// <param name="forumTopicId">Forum topic identifier; pass null to load breadcrumbs up to forum</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum breadcrumb model
   /// </returns>
   public virtual async Task<ForumBreadcrumbModel> PrepareForumBreadcrumbModelAsync(long? forumGroupId, long? forumId, long? forumTopicId)
   {
      var model = new ForumBreadcrumbModel();

      ForumTopic forumTopic = null;
      if (forumTopicId.HasValue)
      {
         forumTopic = await _forumService.GetTopicByIdAsync(forumTopicId.Value);
         if (forumTopic != null)
         {
            model.ForumTopicId = forumTopic.Id;
            model.ForumTopicSubject = forumTopic.Subject;
            model.ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic);
         }
      }

      var forum = await _forumService.GetForumByIdAsync(forumTopic != null ? forumTopic.ForumId : forumId ?? 0);
      if (forum != null)
      {
         model.ForumId = forum.Id;
         model.ForumName = forum.Name;
         model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
      }

      var forumGroup = await _forumService.GetForumGroupByIdAsync(forum != null ? forum.ForumGroupId : forumGroupId ?? 0);
      if (forumGroup != null)
      {
         model.ForumGroupId = forumGroup.Id;
         model.ForumGroupName = forumGroup.Name;
         model.ForumGroupSeName = await _forumService.GetForumGroupSeNameAsync(forumGroup);
      }

      return model;
   }

   /// <summary>
   /// Prepare the user forum subscriptions model
   /// </summary>
   /// <param name="page">Number of items page; pass null to load the first page</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user forum subscriptions model
   /// </returns>
   public virtual async Task<UserForumSubscriptionsModel> PrepareUserForumSubscriptionsModelAsync(int? page)
   {
      var pageIndex = 0;
      if (page > 0)
         pageIndex = page.Value - 1;

      var user = await _workContext.GetCurrentUserAsync();

      var pageSize = _forumSettings.ForumSubscriptionsPageSize;

      var list = await _forumService.GetAllSubscriptionsAsync(user.Id, 0, 0, pageIndex, pageSize);

      var model = new UserForumSubscriptionsModel();

      foreach (var forumSubscription in list)
      {
         var forumTopicId = forumSubscription.TopicId;
         var forumId = forumSubscription.ForumId;
         var topicSubscription = false;
         var title = string.Empty;
         var slug = string.Empty;

         if (forumTopicId > 0)
         {
            topicSubscription = true;
            var forumTopic = await _forumService.GetTopicByIdAsync(forumTopicId);
            if (forumTopic != null)
            {
               title = forumTopic.Subject;
               slug = await _forumService.GetTopicSeNameAsync(forumTopic);
            }
         }
         else
         {
            var forum = await _forumService.GetForumByIdAsync(forumId);
            if (forum != null)
            {
               title = forum.Name;
               slug = await _forumService.GetForumSeNameAsync(forum);
            }
         }

         model.ForumSubscriptions.Add(new UserForumSubscriptionsModel.ForumSubscriptionModel
         {
            Id = forumSubscription.Id,
            ForumTopicId = forumTopicId,
            ForumId = forumSubscription.ForumId,
            TopicSubscription = topicSubscription,
            Title = title,
            Slug = slug,
         });
      }

      model.PagerModel = new PagerModel(_localizationService)
      {
         PageSize = list.PageSize,
         TotalRecords = list.TotalCount,
         PageIndex = list.PageIndex,
         ShowTotalSummary = false,
         RouteActionName = "UserForumSubscriptions",
         UseRouteLinks = true,
         RouteValues = new ForumSubscriptionsRouteValues { pageNumber = pageIndex }
      };

      return model;
   }

   /// <summary>
   /// Prepare the forum topic row model
   /// </summary>
   /// <param name="topic">Forum topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum topic row model
   /// </returns>
   public virtual async Task<ForumTopicRowModel> PrepareForumTopicRowModelAsync(ForumTopic topic)
   {
      if (topic == null)
         throw new ArgumentNullException(nameof(topic));

      var user = await _userService.GetUserByIdAsync(topic.UserId);

      var topicModel = new ForumTopicRowModel
      {
         Id = topic.Id,
         Subject = topic.Subject,
         SeName = await _forumService.GetTopicSeNameAsync(topic),
         LastPostId = topic.LastPostId,
         NumPosts = topic.NumPosts,
         Views = topic.Views,
         NumReplies = topic.NumPosts > 0 ? topic.NumPosts - 1 : 0,
         ForumTopicType = topic.ForumTopicType,
         UserId = topic.UserId,
         AllowViewingProfiles = _userSettings.AllowViewingProfiles && !await _userService.IsGuestAsync(user),
         UserName = await _userService.FormatUsernameAsync(user)
      };

      var forumPosts = await _forumService.GetAllPostsAsync(topic.Id, 0, string.Empty, 1, _forumSettings.PostsPageSize);
      topicModel.TotalPostPages = forumPosts.TotalPages;

      var firstPost = await _forumService.GetFirstPostAsync(topic);
      topicModel.Votes = firstPost != null ? firstPost.VoteCount : 0;

      return topicModel;
   }

   /// <summary>
   /// Prepare the forum row model
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum row model
   /// </returns>
   public virtual async Task<ForumRowModel> PrepareForumRowModelAsync(Forum forum)
   {
      if (forum == null)
         throw new ArgumentNullException(nameof(forum));

      var forumModel = new ForumRowModel
      {
         Id = forum.Id,
         Name = forum.Name,
         SeName = await _forumService.GetForumSeNameAsync(forum),
         Description = forum.Description,
         NumTopics = forum.NumTopics,
         NumPosts = forum.NumPosts,
         LastPostId = forum.LastPostId,
      };

      return forumModel;
   }

   #endregion
}