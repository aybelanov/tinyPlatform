using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Topics;
using Hub.Data;
using Hub.Services.Security;
using Hub.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Topics;

/// <summary>
/// ForumPost service
/// </summary>
public partial class TopicService : ITopicService
{
   #region Fields

   private readonly IAclService _aclService;
   private readonly IUserService _userService;
   private readonly IRepository<Topic> _topicRepository;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public TopicService(
       IAclService aclService,
       IUserService userService,
       IRepository<Topic> topicRepository,
       IStaticCacheManager staticCacheManager,
       IWorkContext workContext)
   {
      _aclService = aclService;
      _userService = userService;
      _topicRepository = topicRepository;
      _staticCacheManager = staticCacheManager;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Deletes a topic
   /// </summary>
   /// <param name="topic">ForumPost</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteTopicAsync(Topic topic)
   {
      await _topicRepository.DeleteAsync(topic);
   }

   /// <summary>
   /// Gets a topic
   /// </summary>
   /// <param name="topicId">The topic identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic
   /// </returns>
   public virtual async Task<Topic> GetTopicByIdAsync(long topicId)
   {
      return await _topicRepository.GetByIdAsync(topicId, cache => default);
   }

   /// <summary>
   /// Gets a topic
   /// </summary>
   /// <param name="systemName">The topic system name</param>
   /// <param name="showHidden">A value indicating whether to show hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic
   /// </returns>
   public virtual async Task<Topic> GetTopicBySystemNameAsync(string systemName, bool showHidden = false)
   {
      if (string.IsNullOrEmpty(systemName))
         return null;

      var user = await _workContext.GetCurrentUserAsync();
      var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppTopicDefaults.TopicBySystemNameCacheKey, systemName, userRoleIds);

      var topic = await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         var query = _topicRepository.Table;

         if (!showHidden)
            query = query.Where(t => t.Published);

         //apply ACL constraints
         if (!showHidden)
            query = await _aclService.ApplyAcl(query, userRoleIds);

         return query.Where(t => t.SystemName == systemName)
                .OrderBy(t => t.Id)
                .FirstOrDefault();
      });

      return topic;
   }

   /// <summary>
   /// Gets all topics
   /// </summary>
   /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
   /// <param name="showHidden">A value indicating whether to show hidden topics</param>
   /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opics
   /// </returns>
   public virtual async Task<IList<Topic>> GetAllTopicsAsync(bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

      return await _topicRepository.GetAllAsync(async query =>
      {
         if (!showHidden)
            query = query.Where(t => t.Published);

         //apply ACL constraints
         if (!showHidden && !ignoreAcl)
            query = await _aclService.ApplyAcl(query, userRoleIds);

         if (onlyIncludedInTopMenu)
            query = query.Where(t => t.IncludeInTopMenu);

         return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
      }, cache =>
      {
         return ignoreAcl
                ? cache.PrepareKeyForDefaultCache(AppTopicDefaults.TopicsAllCacheKey, showHidden, onlyIncludedInTopMenu)
                : cache.PrepareKeyForDefaultCache(AppTopicDefaults.TopicsAllWithACLCacheKey, showHidden, onlyIncludedInTopMenu, userRoleIds);
      });
   }

   /// <summary>
   /// Gets all topics
   /// </summary>
   /// <param name="keywords">Keywords to search into body or title</param>
   /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
   /// <param name="showHidden">A value indicating whether to show hidden topics</param>
   /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opics
   /// </returns>
   public virtual async Task<IList<Topic>> GetAllTopicsAsync(string keywords, bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
   {
      var topics = await GetAllTopicsAsync(ignoreAcl: ignoreAcl, showHidden: showHidden, onlyIncludedInTopMenu: onlyIncludedInTopMenu);

      if (!string.IsNullOrWhiteSpace(keywords))
      {
         return topics
             .Where(topic => (topic.Title?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                 (topic.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false))
             .ToList();
      }

      return topics;
   }

   /// <summary>
   /// Inserts a topic
   /// </summary>
   /// <param name="topic">ForumPost</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertTopicAsync(Topic topic)
   {
      await _topicRepository.InsertAsync(topic);
   }

   /// <summary>
   /// Updates the topic
   /// </summary>
   /// <param name="topic">ForumPost</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateTopicAsync(Topic topic)
   {
      await _topicRepository.UpdateAsync(topic);
   }

   #endregion
}