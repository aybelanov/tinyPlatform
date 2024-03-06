﻿using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Seo;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Data.Extensions;
using Hub.Services.Common;
using Hub.Services.Html;
using Hub.Services.Messages;
using Hub.Services.Seo;
using Hub.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Forums
{
   /// <summary>
   /// Forum service
   /// </summary>
   public partial class ForumService : IForumService
   {
      #region Fields

      private readonly ForumSettings _forumSettings;
      private readonly IUserService _userService;
      private readonly IGenericAttributeService _genericAttributeService;
      private readonly IHtmlFormatter _htmlFormatter;
      private readonly IRepository<User> _userRepository;
      private readonly IRepository<Forum> _forumRepository;
      private readonly IRepository<ForumGroup> _forumGroupRepository;
      private readonly IRepository<ForumPost> _forumPostRepository;
      private readonly IRepository<ForumPostVote> _forumPostVoteRepository;
      private readonly IRepository<ForumSubscription> _forumSubscriptionRepository;
      private readonly IRepository<ForumTopic> _forumTopicRepository;
      private readonly IRepository<PrivateMessage> _forumPrivateMessageRepository;
      private readonly IStaticCacheManager _staticCacheManager;
      private readonly IUrlRecordService _urlRecordService;
      private readonly IWorkContext _workContext;
      private readonly IWorkflowMessageService _workflowMessageService;
      private readonly SeoSettings _seoSettings;

      #endregion

      #region Ctor

      /// <summary>
      /// Default Ctor
      /// </summary>
      public ForumService(ForumSettings forumSettings,
          IUserService userService,
          IGenericAttributeService genericAttributeService,
          IHtmlFormatter htmlFormatter,
          IRepository<User> userRepository,
          IRepository<Forum> forumRepository,
          IRepository<ForumGroup> forumGroupRepository,
          IRepository<ForumPost> forumPostRepository,
          IRepository<ForumPostVote> forumPostVoteRepository,
          IRepository<ForumSubscription> forumSubscriptionRepository,
          IRepository<ForumTopic> forumTopicRepository,
          IRepository<PrivateMessage> forumPrivateMessageRepository,
          IStaticCacheManager staticCacheManager,
          IUrlRecordService urlRecordService,
          IWorkContext workContext,
          IWorkflowMessageService workflowMessageService,
          SeoSettings seoSettings)
      {
         _forumSettings = forumSettings;
         _userService = userService;
         _genericAttributeService = genericAttributeService;
         _htmlFormatter = htmlFormatter;
         _userRepository = userRepository;
         _forumRepository = forumRepository;
         _forumGroupRepository = forumGroupRepository;
         _forumPostRepository = forumPostRepository;
         _forumPostVoteRepository = forumPostVoteRepository;
         _forumSubscriptionRepository = forumSubscriptionRepository;
         _forumTopicRepository = forumTopicRepository;
         _forumPrivateMessageRepository = forumPrivateMessageRepository;
         _staticCacheManager = staticCacheManager;
         _urlRecordService = urlRecordService;
         _workContext = workContext;
         _workflowMessageService = workflowMessageService;
         _seoSettings = seoSettings;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Update forum stats
      /// </summary>
      /// <param name="forumId">The forum identifier</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      private async Task UpdateForumStatsAsync(long forumId)
      {
         if (forumId == 0)
            return;

         var forum = await GetForumByIdAsync(forumId);
         if (forum == null)
            return;

         //number of topics
         var queryNumTopics = from ft in _forumTopicRepository.Table
                              where ft.ForumId == forumId
                              select ft.Id;
         var numTopics = await queryNumTopics.CountAsync();

         //number of posts
         var queryNumPosts = from ft in _forumTopicRepository.Table
                             join fp in _forumPostRepository.Table on ft.Id equals fp.ForumTopicId
                             where ft.ForumId == forumId
                             select fp.Id;
         var numPosts = await queryNumPosts.CountAsync();

         //last values
         var lastTopicId = 0L;
         var lastPostId = 0L;
         var lastPostUserId = 0L;
         DateTime? lastPostTime = null;
         var queryLastValues = from ft in _forumTopicRepository.Table
                               join fp in _forumPostRepository.Table on ft.Id equals fp.ForumTopicId
                               where ft.ForumId == forumId
                               orderby fp.CreatedOnUtc descending, ft.CreatedOnUtc descending
                               select new
                               {
                                  LastTopicId = ft.Id,
                                  LastPostId = fp.Id,
                                  LastPostUserId = fp.UserId,
                                  LastPostTime = fp.CreatedOnUtc
                               };
         var lastValues = await queryLastValues.FirstOrDefaultAsync();
         if (lastValues != null)
         {
            lastTopicId = lastValues.LastTopicId;
            lastPostId = lastValues.LastPostId;
            lastPostUserId = lastValues.LastPostUserId;
            lastPostTime = lastValues.LastPostTime;
         }

         //update forum
         forum.NumTopics = numTopics;
         forum.NumPosts = numPosts;
         forum.LastTopicId = lastTopicId;
         forum.LastPostId = lastPostId;
         forum.LastPostUserId = lastPostUserId;
         forum.LastPostTime = lastPostTime;
         await UpdateForumAsync(forum);
      }

      /// <summary>
      /// Update forum topic stats
      /// </summary>
      /// <param name="forumTopicId">The forum topic identifier</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      private async Task UpdateForumTopicStatsAsync(long forumTopicId)
      {
         if (forumTopicId == 0)
            return;

         var forumTopic = await GetTopicByIdAsync(forumTopicId);
         if (forumTopic == null)
            return;

         //number of posts
         var queryNumPosts = from fp in _forumPostRepository.Table
                             where fp.ForumTopicId == forumTopicId
                             select fp.Id;
         var numPosts = await queryNumPosts.CountAsync();

         //last values
         var lastPostId = 0L;
         var lastPostUserId = 0L;
         DateTime? lastPostTime = null;
         var queryLastValues = from fp in _forumPostRepository.Table
                               where fp.ForumTopicId == forumTopicId
                               orderby fp.CreatedOnUtc descending
                               select new
                               {
                                  LastPostId = fp.Id,
                                  LastPostUserId = fp.UserId,
                                  LastPostTime = fp.CreatedOnUtc
                               };
         var lastValues = await queryLastValues.FirstOrDefaultAsync();
         if (lastValues != null)
         {
            lastPostId = lastValues.LastPostId;
            lastPostUserId = lastValues.LastPostUserId;
            lastPostTime = lastValues.LastPostTime;
         }

         //update topic
         forumTopic.NumPosts = numPosts;
         forumTopic.LastPostId = lastPostId;
         forumTopic.LastPostUserId = lastPostUserId;
         forumTopic.LastPostTime = lastPostTime;

         await UpdateTopicAsync(forumTopic);
      }

      /// <summary>
      /// Update user stats
      /// </summary>
      /// <param name="userId">The user identifier</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      private async Task UpdateUserStatsAsync(long userId)
      {
         if (userId == 0)
            return;

         var user = await _userService.GetUserByIdAsync(userId);

         if (user == null)
            return;

         var query = from fp in _forumPostRepository.Table
                     where fp.UserId == userId
                     select fp.Id;
         var numPosts = await query.CountAsync();

         await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.ForumPostCountAttribute, numPosts);
      }

      /// <summary>
      /// Gets a forum topic
      /// </summary>
      /// <param name="forumTopicId">The forum topic identifier</param>
      /// <param name="increaseViews">The value indicating whether to increase forum topic views</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum Topic
      /// </returns>
      protected virtual async Task<ForumTopic> GetTopicByIdAsync(long forumTopicId, bool increaseViews)
      {
         var forumTopic = await _forumTopicRepository.GetByIdAsync(forumTopicId, cache => default);// _staticCacheManager.PrepareKeyForDefaultCache(AppEntityCacheDefaults<ForumTopic>.ByIdCacheKey, forumTopicId));

         if (forumTopic == null)
            return null;

         if (!increaseViews)
            return forumTopic;

         forumTopic.Views = ++forumTopic.Views;
         await UpdateTopicAsync(forumTopic);

         return forumTopic;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Deletes a forum group
      /// </summary>
      /// <param name="forumGroup">Forum group</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeleteForumGroupAsync(ForumGroup forumGroup)
      {
         await _forumGroupRepository.DeleteAsync(forumGroup);
      }

      /// <summary>
      /// Gets a forum group
      /// </summary>
      /// <param name="forumGroupId">The forum group identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum group
      /// </returns>
      public virtual async Task<ForumGroup> GetForumGroupByIdAsync(long forumGroupId)
      {
         return await _forumGroupRepository.GetByIdAsync(forumGroupId, cache => default);
      }

      /// <summary>
      /// Gets all forum groups
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum groups
      /// </returns>
      public virtual async Task<IList<ForumGroup>> GetAllForumGroupsAsync()
      {
         return await _forumGroupRepository.GetAllAsync(query =>
         {
            return from fg in query
                   orderby fg.DisplayOrder, fg.Id
                   select fg;
         }, cache => default);
      }

      /// <summary>
      /// Inserts a forum group
      /// </summary>
      /// <param name="forumGroup">Forum group</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertForumGroupAsync(ForumGroup forumGroup)
      {
         await _forumGroupRepository.InsertAsync(forumGroup);
      }

      /// <summary>
      /// Updates the forum group
      /// </summary>
      /// <param name="forumGroup">Forum group</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdateForumGroupAsync(ForumGroup forumGroup)
      {
         await _forumGroupRepository.UpdateAsync(forumGroup);
      }

      /// <summary>
      /// Deletes a forum
      /// </summary>
      /// <param name="forum">Forum</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeleteForumAsync(Forum forum)
      {
         if (forum == null)
            throw new ArgumentNullException(nameof(forum));

         //delete forum subscriptions (topics)
         var queryTopicIds = from ft in _forumTopicRepository.Table
                             where ft.ForumId == forum.Id
                             select ft.Id;
         var queryFs1 = from fs in _forumSubscriptionRepository.Table
                        where queryTopicIds.Contains(fs.TopicId)
                        select fs;

         await _forumSubscriptionRepository.DeleteAsync(queryFs1.ToList());

         //delete forum subscriptions (forum)
         var queryFs2 = from fs in _forumSubscriptionRepository.Table
                        where fs.ForumId == forum.Id
                        select fs;

         await _forumSubscriptionRepository.DeleteAsync(queryFs2.ToList());

         //delete forum
         await _forumRepository.DeleteAsync(forum);
      }

      /// <summary>
      /// Gets a forum
      /// </summary>
      /// <param name="forumId">The forum identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum
      /// </returns>
      public virtual async Task<Forum> GetForumByIdAsync(long forumId)
      {
         return await _forumRepository.GetByIdAsync(forumId, cache => default);
      }

      /// <summary>
      /// Gets forums by forum group identifier
      /// </summary>
      /// <param name="forumGroupId">The forum group identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forums
      /// </returns>
      public virtual async Task<IList<Forum>> GetAllForumsByGroupIdAsync(long forumGroupId)
      {
         var forums = await _forumRepository.GetAllAsync(query =>
         {
            return from f in query
                   orderby f.DisplayOrder, f.Id
                   where f.ForumGroupId == forumGroupId
                   select f;
         }, cache => cache.PrepareKeyForDefaultCache(AppForumDefaults.ForumByForumGroupCacheKey, forumGroupId));

         return forums;
      }

      /// <summary>
      /// Inserts a forum
      /// </summary>
      /// <param name="forum">Forum</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertForumAsync(Forum forum)
      {
         await _forumRepository.InsertAsync(forum);
      }

      /// <summary>
      /// Updates the forum
      /// </summary>
      /// <param name="forum">Forum</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdateForumAsync(Forum forum)
      {
         // if the forum group is changed then clear cache for the previous group 
         // (we can't use the event consumer because it will work after saving the changes in DB)
         var forumToUpdate = await _forumRepository.LoadOriginalCopyAsync(forum);
         if (forumToUpdate.ForumGroupId != forum.ForumGroupId)
            await _staticCacheManager.RemoveAsync(AppForumDefaults.ForumByForumGroupCacheKey, forumToUpdate.ForumGroupId);

         await _forumRepository.UpdateAsync(forum);
      }

      /// <summary>
      /// Deletes a forum topic
      /// </summary>
      /// <param name="forumTopic">Forum topic</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeleteTopicAsync(ForumTopic forumTopic)
      {
         if (forumTopic == null)
            throw new ArgumentNullException(nameof(forumTopic));

         var userId = forumTopic.UserId;
         var forumId = forumTopic.ForumId;

         //delete topic
         await _forumTopicRepository.DeleteAsync(forumTopic);

         //delete forum subscriptions
         var queryFs = from ft in _forumSubscriptionRepository.Table
                       where ft.TopicId == forumTopic.Id
                       select ft;
         var forumSubscriptions = queryFs.ToList();

         await _forumSubscriptionRepository.DeleteAsync(forumSubscriptions);

         //update stats
         await UpdateForumStatsAsync(forumId);
         await UpdateUserStatsAsync(userId);
      }

      /// <summary>
      /// Gets a forum topic
      /// </summary>
      /// <param name="forumTopicId">The forum topic identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum Topic
      /// </returns>
      public virtual async Task<ForumTopic> GetTopicByIdAsync(long forumTopicId)
      {
         return await GetTopicByIdAsync(forumTopicId, false);
      }

      /// <summary>
      /// Gets all forum topics
      /// </summary>
      /// <param name="forumId">The forum identifier</param>
      /// <param name="userId">The user identifier</param>
      /// <param name="keywords">Keywords</param>
      /// <param name="searchType">Search type</param>
      /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum Topics
      /// </returns>
      public virtual async Task<IPagedList<ForumTopic>> GetAllTopicsAsync(long forumId = 0,
          long userId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All,
          int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue)
      {
         DateTime? limitDate = null;
         if (limitDays > 0)
            limitDate = DateTime.UtcNow.AddDays(-limitDays);

         var searchKeywords = !string.IsNullOrEmpty(keywords);
         var searchTopicTitles = searchType == ForumSearchType.All || searchType == ForumSearchType.TopicTitlesOnly;
         var searchPostText = searchType == ForumSearchType.All || searchType == ForumSearchType.PostTextOnly;

         var topics = await _forumTopicRepository.GetAllPagedAsync(query =>
         {
            var query1 = from ft in query
                         join fp in _forumPostRepository.Table on ft.Id equals fp.ForumTopicId
                         where
                             (forumId == 0 || ft.ForumId == forumId) &&
                             (userId == 0 || ft.UserId == userId) &&
                             (!searchKeywords ||
                              (searchTopicTitles && ft.Subject.Contains(keywords)) ||
                              (searchPostText && fp.Text.Contains(keywords))) &&
                             (!limitDate.HasValue || limitDate.Value <= ft.LastPostTime)
                         select ft.Id;

            var query2 = from ft in query
                         where query1.Contains(ft.Id)
                         orderby ft.TopicTypeId descending, ft.LastPostTime descending, ft.Id descending
                         select ft;

            return query2;
         }, pageIndex, pageSize);

         return topics;
      }

      /// <summary>
      /// Gets active forum topics
      /// </summary>
      /// <param name="forumId">The forum identifier</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum Topics
      /// </returns>
      public virtual async Task<IPagedList<ForumTopic>> GetActiveTopicsAsync(long forumId = 0,
          int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var query1 = from ft in _forumTopicRepository.Table
                      where
                      (forumId == 0 || ft.ForumId == forumId) &&
                      ft.LastPostTime.HasValue
                      select ft.Id;

         var query2 = from ft in _forumTopicRepository.Table
                      where query1.Contains(ft.Id)
                      orderby ft.LastPostTime descending
                      select ft;

         var topics = await query2.ToPagedListAsync(pageIndex, pageSize);

         return topics;
      }

      /// <summary>
      /// Inserts a forum topic
      /// </summary>
      /// <param name="forumTopic">Forum topic</param>
      /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed users</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertTopicAsync(ForumTopic forumTopic, bool sendNotifications)
      {
         await _forumTopicRepository.InsertAsync(forumTopic);

         //update stats
         await UpdateForumStatsAsync(forumTopic.ForumId);

         if (!sendNotifications)
            return;

         //send notifications
         var forum = await GetForumByIdAsync(forumTopic.ForumId);
         var subscriptions = await GetAllSubscriptionsAsync(forumId: forum.Id);
         var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

         foreach (var subscription in subscriptions)
         {
            if (subscription.UserId == forumTopic.UserId)
               continue;

            var user = await _userService.GetUserByIdAsync(subscription.UserId);

            if (!string.IsNullOrEmpty(user?.Email))
               await _workflowMessageService.SendNewForumTopicMessageAsync(user, forumTopic, forum, languageId);
         }
      }

      /// <summary>
      /// Updates the forum topic
      /// </summary>
      /// <param name="forumTopic">Forum topic</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdateTopicAsync(ForumTopic forumTopic)
      {
         await _forumTopicRepository.UpdateAsync(forumTopic);
      }

      /// <summary>
      /// Moves the forum topic
      /// </summary>
      /// <param name="forumTopicId">The forum topic identifier</param>
      /// <param name="newForumId">New forum identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the moved forum topic
      /// </returns>
      public virtual async Task<ForumTopic> MoveTopicAsync(long forumTopicId, long newForumId)
      {
         var forumTopic = await GetTopicByIdAsync(forumTopicId);
         if (forumTopic == null)
            return null;

         if (!await IsUserAllowedToMoveTopicAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
            return forumTopic;

         var previousForumId = forumTopic.ForumId;
         var newForum = await GetForumByIdAsync(newForumId);

         if (newForum == null)
            return forumTopic;

         if (previousForumId == newForumId)
            return forumTopic;

         forumTopic.ForumId = newForum.Id;
         forumTopic.UpdatedOnUtc = DateTime.UtcNow;
         await UpdateTopicAsync(forumTopic);

         //update forum stats
         await UpdateForumStatsAsync(previousForumId);
         await UpdateForumStatsAsync(newForumId);
         return forumTopic;
      }

      /// <summary>
      /// Deletes a forum post
      /// </summary>
      /// <param name="forumPost">Forum post</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeletePostAsync(ForumPost forumPost)
      {
         if (forumPost == null)
            throw new ArgumentNullException(nameof(forumPost));

         var forumTopicId = forumPost.ForumTopicId;
         var userId = forumPost.UserId;
         var forumTopic = await GetTopicByIdAsync(forumTopicId);
         var forumId = forumTopic.ForumId;

         //delete topic if it was the first post
         var deleteTopic = false;
         var firstPost = await GetFirstPostAsync(forumTopic);
         if (firstPost != null && firstPost.Id == forumPost.Id)
            deleteTopic = true;

         //delete forum post
         await _forumPostRepository.DeleteAsync(forumPost);

         //delete topic
         if (deleteTopic)
            await DeleteTopicAsync(forumTopic);

         //update stats
         if (!deleteTopic)
            await UpdateForumTopicStatsAsync(forumTopicId);

         await UpdateForumStatsAsync(forumId);
         await UpdateUserStatsAsync(userId);
      }

      /// <summary>
      /// Gets a forum post
      /// </summary>
      /// <param name="forumPostId">The forum post identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum Post
      /// </returns>
      public virtual async Task<ForumPost> GetPostByIdAsync(long forumPostId)
      {
         return await _forumPostRepository.GetByIdAsync(forumPostId, cache => default);
      }

      /// <summary>
      /// Gets all forum posts
      /// </summary>
      /// <param name="forumTopicId">The forum topic identifier</param>
      /// <param name="userId">The user identifier</param>
      /// <param name="keywords">Keywords</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the posts
      /// </returns>
      public virtual async Task<IPagedList<ForumPost>> GetAllPostsAsync(long forumTopicId = 0,
          long userId = 0, string keywords = "",
          int pageIndex = 0, int pageSize = int.MaxValue)
      {
         return await GetAllPostsAsync(forumTopicId, userId, keywords, true,
             pageIndex, pageSize);
      }

      /// <summary>
      /// Gets all forum posts
      /// </summary>
      /// <param name="forumTopicId">The forum topic identifier</param>
      /// <param name="userId">The user identifier</param>
      /// <param name="keywords">Keywords</param>
      /// <param name="ascSort">Sort order</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum Posts
      /// </returns>
      public virtual async Task<IPagedList<ForumPost>> GetAllPostsAsync(long forumTopicId = 0, long userId = 0,
          string keywords = "", bool ascSort = false,
          int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var forumPosts = await _forumPostRepository.GetAllPagedAsync(query =>
         {
            if (forumTopicId > 0)
               query = query.Where(fp => forumTopicId == fp.ForumTopicId);

            if (userId > 0)
               query = query.Where(fp => userId == fp.UserId);

            if (!string.IsNullOrEmpty(keywords))
               query = query.Where(fp => fp.Text.Contains(keywords));

            query = ascSort
                ? query.OrderBy(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id)
                : query.OrderByDescending(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id);

            return query;
         }, pageIndex, pageSize);

         return forumPosts;
      }

      /// <summary>
      /// Inserts a forum post
      /// </summary>
      /// <param name="forumPost">The forum post</param>
      /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed users</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertPostAsync(ForumPost forumPost, bool sendNotifications)
      {
         await _forumPostRepository.InsertAsync(forumPost);

         //update stats
         var userId = forumPost.UserId;
         var forumTopic = await GetTopicByIdAsync(forumPost.ForumTopicId);
         var forumId = forumTopic.ForumId;

         await UpdateForumTopicStatsAsync(forumPost.ForumTopicId);
         await UpdateForumStatsAsync(forumId);
         await UpdateUserStatsAsync(userId);

         //notifications
         if (!sendNotifications)
            return;

         var forum = await GetForumByIdAsync(forumTopic.ForumId);
         var subscriptions = await GetAllSubscriptionsAsync(topicId: forumTopic.Id);

         var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

         var friendlyTopicPageIndex = await CalculateTopicPageIndexAsync(forumPost.ForumTopicId,
                                          _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10,
                                          forumPost.Id) + 1;

         foreach (var subscription in subscriptions)
         {
            if (subscription.UserId == forumPost.UserId)
               continue;

            var user = await _userService.GetUserByIdAsync(subscription.UserId);

            if (!string.IsNullOrEmpty(user?.Email))
               await _workflowMessageService.SendNewForumPostMessageAsync(user, forumPost, forumTopic, forum, friendlyTopicPageIndex, languageId);
         }
      }

      /// <summary>
      /// Updates the forum post
      /// </summary>
      /// <param name="forumPost">Forum post</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdatePostAsync(ForumPost forumPost)
      {
         await _forumPostRepository.UpdateAsync(forumPost);
      }

      /// <summary>
      /// Deletes a private message
      /// </summary>
      /// <param name="privateMessage">Private message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeletePrivateMessageAsync(PrivateMessage privateMessage)
      {
         await _forumPrivateMessageRepository.DeleteAsync(privateMessage);
      }

      /// <summary>
      /// Gets a private message
      /// </summary>
      /// <param name="privateMessageId">The private message identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the private message
      /// </returns>
      public virtual async Task<PrivateMessage> GetPrivateMessageByIdAsync(long privateMessageId)
      {
         return await _forumPrivateMessageRepository.GetByIdAsync(privateMessageId, cache => default);
      }

      /// <summary>
      /// Gets private messages
      /// </summary>
      /// <param name="fromUserId">The user identifier who sent the message</param>
      /// <param name="toUserId">The user identifier who should receive the message</param>
      /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
      /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
      /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
      /// <param name="keywords">Keywords</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the private messages
      /// </returns>
      public virtual async Task<IPagedList<PrivateMessage>> GetAllPrivateMessagesAsync(long fromUserId,
          long toUserId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
          string keywords, int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var privateMessages = await _forumPrivateMessageRepository.GetAllPagedAsync(query =>
         {
            if (fromUserId > 0)
               query = query.Where(pm => fromUserId == pm.FromUserId);
            if (toUserId > 0)
               query = query.Where(pm => toUserId == pm.ToUserId);
            if (isRead.HasValue)
               query = query.Where(pm => isRead.Value == pm.IsRead);
            if (isDeletedByAuthor.HasValue)
               query = query.Where(pm => isDeletedByAuthor.Value == pm.IsDeletedByAuthor);
            if (isDeletedByRecipient.HasValue)
               query = query.Where(pm => isDeletedByRecipient.Value == pm.IsDeletedByRecipient);
            if (!string.IsNullOrEmpty(keywords))
            {
               query = query.Where(pm => pm.Subject.Contains(keywords));
               query = query.Where(pm => pm.Text.Contains(keywords));
            }

            query = query.OrderByDescending(pm => pm.CreatedOnUtc);

            return query;
         }, pageIndex, pageSize);

         return privateMessages;
      }

      /// <summary>
      /// Inserts a private message
      /// </summary>
      /// <param name="privateMessage">Private message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertPrivateMessageAsync(PrivateMessage privateMessage)
      {
         await _forumPrivateMessageRepository.InsertAsync(privateMessage);

         var userTo = await _userService.GetUserByIdAsync(privateMessage.ToUserId);
         if (userTo == null)
            throw new AppException("Recipient could not be loaded");

         //UI notification
         await _genericAttributeService.SaveAttributeAsync(userTo, AppUserDefaults.NotifiedAboutNewPrivateMessagesAttribute, false);

         //Email notification
         if (_forumSettings.NotifyAboutPrivateMessages)
            await _workflowMessageService.SendPrivateMessageNotificationAsync(privateMessage, (await _workContext.GetWorkingLanguageAsync()).Id);
      }

      /// <summary>
      /// Updates the private message
      /// </summary>
      /// <param name="privateMessage">Private message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdatePrivateMessageAsync(PrivateMessage privateMessage)
      {
         if (privateMessage == null)
            throw new ArgumentNullException(nameof(privateMessage));

         if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient)
            await _forumPrivateMessageRepository.DeleteAsync(privateMessage);
         else
            await _forumPrivateMessageRepository.UpdateAsync(privateMessage);
      }

      /// <summary>
      /// Deletes a forum subscription
      /// </summary>
      /// <param name="forumSubscription">Forum subscription</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeleteSubscriptionAsync(ForumSubscription forumSubscription)
      {
         await _forumSubscriptionRepository.DeleteAsync(forumSubscription);
      }

      /// <summary>
      /// Gets a forum subscription
      /// </summary>
      /// <param name="forumSubscriptionId">The forum subscription identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum subscription
      /// </returns>
      public virtual async Task<ForumSubscription> GetSubscriptionByIdAsync(long forumSubscriptionId)
      {
         return await _forumSubscriptionRepository.GetByIdAsync(forumSubscriptionId, cache => default);
      }

      /// <summary>
      /// Gets forum subscriptions
      /// </summary>
      /// <param name="userId">The user identifier</param>
      /// <param name="forumId">The forum identifier</param>
      /// <param name="topicId">The topic identifier</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum subscriptions
      /// </returns>
      public virtual async Task<IPagedList<ForumSubscription>> GetAllSubscriptionsAsync(long userId = 0, long forumId = 0,
          long topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var forumSubscriptions = await _forumSubscriptionRepository.GetAllPagedAsync(query =>
         {
            var fsQuery = from fs in query
                          join c in _userRepository.Table on fs.UserId equals c.Id
                          where
                              (userId == 0 || fs.UserId == userId) &&
                              (forumId == 0 || fs.ForumId == forumId) &&
                              (topicId == 0 || fs.TopicId == topicId) &&
                              c.IsActive &&
                              !c.IsDeleted
                          select fs.SubscriptionGuid;

            var rez = from fs in query
                      where fsQuery.Contains(fs.SubscriptionGuid)
                      orderby fs.CreatedOnUtc descending, fs.SubscriptionGuid descending
                      select fs;

            return rez;
         }, pageIndex, pageSize);

         return forumSubscriptions;
      }

      /// <summary>
      /// Inserts a forum subscription
      /// </summary>
      /// <param name="forumSubscription">Forum subscription</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertSubscriptionAsync(ForumSubscription forumSubscription)
      {
         await _forumSubscriptionRepository.InsertAsync(forumSubscription);
      }

      /// <summary>
      /// Check whether user is allowed to create new topics
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="forum">Forum</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToCreateTopicAsync(User user, Forum forum)
      {
         if (forum == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user) && !_forumSettings.AllowGuestsToCreateTopics)
            return false;

         return true;
      }

      /// <summary>
      /// Check whether user is allowed to edit topic
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="topic">Topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToEditTopicAsync(User user, ForumTopic topic)
      {
         if (topic == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         if (await _userService.IsForumModeratorAsync(user))
            return true;

         if (!_forumSettings.AllowUsersToEditPosts)
            return false;

         var ownTopic = user.Id == topic.UserId;

         return ownTopic;
      }

      /// <summary>
      /// Check whether user is allowed to move topic
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="topic">Topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToMoveTopicAsync(User user, ForumTopic topic)
      {
         if (topic == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         return await _userService.IsForumModeratorAsync(user);
      }

      /// <summary>
      /// Check whether user is allowed to delete topic
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="topic">Topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToDeleteTopicAsync(User user, ForumTopic topic)
      {
         if (topic == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         if (await _userService.IsForumModeratorAsync(user))
            return true;

         if (!_forumSettings.AllowUsersToDeletePosts)
            return false;

         var ownTopic = user.Id == topic.UserId;

         return ownTopic;
      }

      /// <summary>
      /// Check whether user is allowed to create new post
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="topic">Topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToCreatePostAsync(User user, ForumTopic topic)
      {
         if (topic == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user) && !_forumSettings.AllowGuestsToCreatePosts)
            return false;

         return true;
      }

      /// <summary>
      /// Check whether user is allowed to edit post
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="post">Topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToEditPostAsync(User user, ForumPost post)
      {
         if (post == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         if (await _userService.IsForumModeratorAsync(user))
            return true;

         if (!_forumSettings.AllowUsersToEditPosts)
            return false;

         var ownPost = user.Id == post.UserId;

         return ownPost;
      }

      /// <summary>
      /// Check whether user is allowed to delete post
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="post">Topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToDeletePostAsync(User user, ForumPost post)
      {
         if (post == null)
            return false;

         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         if (await _userService.IsForumModeratorAsync(user))
            return true;

         if (!_forumSettings.AllowUsersToDeletePosts)
            return false;

         var ownPost = user.Id == post.UserId;

         return ownPost;
      }

      /// <summary>
      /// Check whether user is allowed to set topic priority
      /// </summary>
      /// <param name="user">User</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToSetTopicPriorityAsync(User user)
      {
         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         return await _userService.IsForumModeratorAsync(user);
      }

      /// <summary>
      /// Check whether user is allowed to watch topics
      /// </summary>
      /// <param name="user">User</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if allowed, otherwise false
      /// </returns>
      public virtual async Task<bool> IsUserAllowedToSubscribeAsync(User user)
      {
         if (user == null)
            return false;

         if (await _userService.IsGuestAsync(user))
            return false;

         return true;
      }

      /// <summary>
      /// Calculates topic page index by post identifier
      /// </summary>
      /// <param name="forumTopicId">Forum topic identifier</param>
      /// <param name="pageSize">Page size</param>
      /// <param name="postId">Post identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the page index
      /// </returns>
      public virtual async Task<int> CalculateTopicPageIndexAsync(long forumTopicId, int pageSize, long postId)
      {
         var pageIndex = 0;
         var forumPosts = await GetAllPostsAsync(forumTopicId, ascSort: true);

         for (var i = 0; i < forumPosts.TotalCount; i++)
         {
            if (forumPosts[i].Id != postId)
               continue;

            if (pageSize > 0)
               pageIndex = i / pageSize;
         }

         return pageIndex;
      }

      /// <summary>
      /// Get a post vote 
      /// </summary>
      /// <param name="postId">Post identifier</param>
      /// <param name="user">User</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the post vote
      /// </returns>
      public virtual async Task<ForumPostVote> GetPostVoteAsync(long postId, User user)
      {
         if (user == null)
            return null;

         return await _forumPostVoteRepository.Table
             .FirstOrDefaultAsync(pv => pv.ForumPostId == postId && pv.UserId == user.Id);
      }

      /// <summary>
      /// Get post vote made since the parameter date
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="createdFromUtc">Date</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the post votes count
      /// </returns>
      public virtual async Task<int> GetNumberOfPostVotesAsync(User user, DateTime createdFromUtc)
      {
         if (user == null)
            return 0;

         return await _forumPostVoteRepository.Table
             .CountAsync(pv => pv.UserId == user.Id && pv.CreatedOnUtc > createdFromUtc);
      }

      /// <summary>
      /// Insert a post vote
      /// </summary>
      /// <param name="postVote">Post vote</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertPostVoteAsync(ForumPostVote postVote)
      {
         await _forumPostVoteRepository.InsertAsync(postVote);

         //update post
         var post = await GetPostByIdAsync(postVote.ForumPostId);
         post.VoteCount = postVote.IsUp ? ++post.VoteCount : --post.VoteCount;

         await UpdatePostAsync(post);
      }

      /// <summary>
      /// Delete a post vote
      /// </summary>
      /// <param name="postVote">Post vote</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeletePostVoteAsync(ForumPostVote postVote)
      {
         if (postVote == null)
            throw new ArgumentNullException(nameof(postVote));

         await _forumPostVoteRepository.DeleteAsync(postVote);

         // update post
         var post = await GetPostByIdAsync(postVote.ForumPostId);
         post.VoteCount = postVote.IsUp ? --post.VoteCount : ++post.VoteCount;

         await UpdatePostAsync(post);
      }

      /// <summary>
      /// Formats the forum post text
      /// </summary>
      /// <param name="forumPost">Forum post</param>
      /// <returns>Formatted text</returns>
      public virtual string FormatPostText(ForumPost forumPost)
      {
         var text = forumPost.Text;

         if (string.IsNullOrEmpty(text))
            return string.Empty;

         switch (_forumSettings.ForumEditor)
         {
            case EditorType.SimpleTextBox:
               {
                  text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);
               }

               break;
            case EditorType.BBCodeEditor:
               {
                  text = _htmlFormatter.FormatText(text, false, true, false, true, false, false);
               }

               break;
            default:
               break;
         }

         return text;
      }

      /// <summary>
      /// Strips the topic subject
      /// </summary>
      /// <param name="forumTopic">Forum topic</param>
      /// <returns>Formatted subject</returns>
      public virtual string StripTopicSubject(ForumTopic forumTopic)
      {
         var subject = forumTopic.Subject;
         if (string.IsNullOrEmpty(subject))
            return subject;

         var strippedTopicMaxLength = _forumSettings.StrippedTopicMaxLength;
         if (strippedTopicMaxLength <= 0)
            return subject;

         if (subject.Length <= strippedTopicMaxLength)
            return subject;

         var index = subject.IndexOf(" ", strippedTopicMaxLength, StringComparison.Ordinal);

         if (index <= 0)
            return subject;

         subject = subject[0..index];
         subject += "...";

         return subject;
      }

      /// <summary>
      /// Formats the forum signature text
      /// </summary>
      /// <param name="text">Text</param>
      /// <returns>Formatted text</returns>
      public virtual string FormatForumSignatureText(string text)
      {
         if (string.IsNullOrEmpty(text))
            return string.Empty;

         text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);
         return text;
      }

      /// <summary>
      /// Formats the private message text
      /// </summary>
      /// <param name="pm">Private message</param>
      /// <returns>Formatted text</returns>
      public virtual string FormatPrivateMessageText(PrivateMessage pm)
      {
         var text = pm.Text;

         if (string.IsNullOrEmpty(text))
            return string.Empty;

         text = _htmlFormatter.FormatText(text, false, true, false, true, false, false);

         return text;
      }

      /// <summary>
      /// Get first post
      /// </summary>
      /// <param name="forumTopic">Forum topic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum post
      /// </returns>
      public virtual async Task<ForumPost> GetFirstPostAsync(ForumTopic forumTopic)
      {
         if (forumTopic == null)
            throw new ArgumentNullException(nameof(forumTopic));

         var forumPosts = await GetAllPostsAsync(forumTopic.Id, 0, string.Empty, 0, 1);
         if (forumPosts.Any())
            return forumPosts[0];

         return null;
      }

      /// <summary>
      /// Gets ForumGroup SE (search engine) name
      /// </summary>
      /// <param name="forumGroup">ForumGroup</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forumGroup SE (search engine) name
      /// </returns>
      public virtual async Task<string> GetForumGroupSeNameAsync(ForumGroup forumGroup)
      {
         if (forumGroup == null)
            throw new ArgumentNullException(nameof(forumGroup));

         var seName = await _urlRecordService.GetSeNameAsync(forumGroup.Name, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

         return seName;
      }

      /// <summary>
      /// Gets Forum SE (search engine) name
      /// </summary>
      /// <param name="forum">Forum</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forum SE (search engine) name
      /// </returns>
      public virtual async Task<string> GetForumSeNameAsync(Forum forum)
      {
         if (forum == null)
            throw new ArgumentNullException(nameof(forum));

         var seName = await _urlRecordService.GetSeNameAsync(forum.Name, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

         return seName;
      }

      /// <summary>
      /// Gets ForumTopic SE (search engine) name
      /// </summary>
      /// <param name="forumTopic">ForumTopic</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the forumTopic SE (search engine) name
      /// </returns>
      public virtual async Task<string> GetTopicSeNameAsync(ForumTopic forumTopic)
      {
         if (forumTopic == null)
            throw new ArgumentNullException(nameof(forumTopic));

         var seName = await _urlRecordService.GetSeNameAsync(forumTopic.Subject, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

         // Trim SE name to avoid URLs that are too long
         var maxLength = AppSeoDefaults.ForumTopicLength;
         if (seName.Length > maxLength)
            seName = seName[0..maxLength];

         return seName;
      }

      #endregion
   }
}