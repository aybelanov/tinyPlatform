﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Forums;

namespace Hub.Services.Forums;

/// <summary>
/// Forum service interface
/// </summary>
public partial interface IForumService
{
   /// <summary>
   /// Deletes a forum group
   /// </summary>
   /// <param name="forumGroup">Forum group</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteForumGroupAsync(ForumGroup forumGroup);

   /// <summary>
   /// Gets a forum group
   /// </summary>
   /// <param name="forumGroupId">The forum group identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum group
   /// </returns>
   Task<ForumGroup> GetForumGroupByIdAsync(long forumGroupId);

   /// <summary>
   /// Gets all forum groups
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum groups
   /// </returns>
   Task<IList<ForumGroup>> GetAllForumGroupsAsync();

   /// <summary>
   /// Inserts a forum group
   /// </summary>
   /// <param name="forumGroup">Forum group</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertForumGroupAsync(ForumGroup forumGroup);

   /// <summary>
   /// Updates the forum group
   /// </summary>
   /// <param name="forumGroup">Forum group</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateForumGroupAsync(ForumGroup forumGroup);

   /// <summary>
   /// Deletes a forum
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteForumAsync(Forum forum);

   /// <summary>
   /// Gets a forum
   /// </summary>
   /// <param name="forumId">The forum identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum
   /// </returns>
   Task<Forum> GetForumByIdAsync(long forumId);

   /// <summary>
   /// Gets forums by group identifier
   /// </summary>
   /// <param name="forumGroupId">The forum group identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forums
   /// </returns>
   Task<IList<Forum>> GetAllForumsByGroupIdAsync(long forumGroupId);

   /// <summary>
   /// Inserts a forum
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertForumAsync(Forum forum);

   /// <summary>
   /// Updates the forum
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateForumAsync(Forum forum);

   /// <summary>
   /// Deletes a forum topic
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteTopicAsync(ForumTopic forumTopic);

   /// <summary>
   /// Gets a forum topic
   /// </summary>
   /// <param name="forumTopicId">The forum topic identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum Topic
   /// </returns>
   Task<ForumTopic> GetTopicByIdAsync(long forumTopicId);

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
   Task<IPagedList<ForumTopic>> GetAllTopicsAsync(long forumId = 0,
       long userId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All,
       int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue);

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
   Task<IPagedList<ForumTopic>> GetActiveTopicsAsync(long forumId = 0,
       int pageIndex = 0, int pageSize = int.MaxValue);

   /// <summary>
   /// Inserts a forum topic
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed users</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertTopicAsync(ForumTopic forumTopic, bool sendNotifications);

   /// <summary>
   /// Updates the forum topic
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateTopicAsync(ForumTopic forumTopic);

   /// <summary>
   /// Moves the forum topic
   /// </summary>
   /// <param name="forumTopicId">The forum topic identifier</param>
   /// <param name="newForumId">New forum identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the moved forum topic
   /// </returns>
   Task<ForumTopic> MoveTopicAsync(long forumTopicId, long newForumId);

   /// <summary>
   /// Deletes a forum post
   /// </summary>
   /// <param name="forumPost">Forum post</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeletePostAsync(ForumPost forumPost);

   /// <summary>
   /// Gets a forum post
   /// </summary>
   /// <param name="forumPostId">The forum post identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum Post
   /// </returns>
   Task<ForumPost> GetPostByIdAsync(long forumPostId);

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
   Task<IPagedList<ForumPost>> GetAllPostsAsync(long forumTopicId = 0,
       long userId = 0, string keywords = "",
       int pageIndex = 0, int pageSize = int.MaxValue);

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
   Task<IPagedList<ForumPost>> GetAllPostsAsync(long forumTopicId = 0, long userId = 0,
       string keywords = "", bool ascSort = false,
       int pageIndex = 0, int pageSize = int.MaxValue);

   /// <summary>
   /// Inserts a forum post
   /// </summary>
   /// <param name="forumPost">The forum post</param>
   /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed users</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertPostAsync(ForumPost forumPost, bool sendNotifications);

   /// <summary>
   /// Updates the forum post
   /// </summary>
   /// <param name="forumPost">Forum post</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdatePostAsync(ForumPost forumPost);

   /// <summary>
   /// Deletes a private message
   /// </summary>
   /// <param name="privateMessage">Private message</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeletePrivateMessageAsync(PrivateMessage privateMessage);

   /// <summary>
   /// Gets a private message
   /// </summary>
   /// <param name="privateMessageId">The private message identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the private message
   /// </returns>
   Task<PrivateMessage> GetPrivateMessageByIdAsync(long privateMessageId);

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
   Task<IPagedList<PrivateMessage>> GetAllPrivateMessagesAsync(long fromUserId,
       long toUserId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
       string keywords, int pageIndex = 0, int pageSize = int.MaxValue);

   /// <summary>
   /// Inserts a private message
   /// </summary>
   /// <param name="privateMessage">Private message</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertPrivateMessageAsync(PrivateMessage privateMessage);

   /// <summary>
   /// Updates the private message
   /// </summary>
   /// <param name="privateMessage">Private message</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdatePrivateMessageAsync(PrivateMessage privateMessage);

   /// <summary>
   /// Deletes a forum subscription
   /// </summary>
   /// <param name="forumSubscription">Forum subscription</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteSubscriptionAsync(ForumSubscription forumSubscription);

   /// <summary>
   /// Gets a forum subscription
   /// </summary>
   /// <param name="forumSubscriptionId">The forum subscription identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum subscription
   /// </returns>
   Task<ForumSubscription> GetSubscriptionByIdAsync(long forumSubscriptionId);

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
   Task<IPagedList<ForumSubscription>> GetAllSubscriptionsAsync(long userId = 0, long forumId = 0,
       long topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

   /// <summary>
   /// Inserts a forum subscription
   /// </summary>
   /// <param name="forumSubscription">Forum subscription</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertSubscriptionAsync(ForumSubscription forumSubscription);

   /// <summary>
   /// Check whether user is allowed to create new topics
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="forum">Forum</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToCreateTopicAsync(User user, Forum forum);

   /// <summary>
   /// Check whether user is allowed to edit topic
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="topic">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToEditTopicAsync(User user, ForumTopic topic);

   /// <summary>
   /// Check whether user is allowed to move topic
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="topic">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToMoveTopicAsync(User user, ForumTopic topic);

   /// <summary>
   /// Check whether user is allowed to delete topic
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="topic">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToDeleteTopicAsync(User user, ForumTopic topic);

   /// <summary>
   /// Check whether user is allowed to create new post
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="topic">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToCreatePostAsync(User user, ForumTopic topic);

   /// <summary>
   /// Check whether user is allowed to edit post
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="post">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToEditPostAsync(User user, ForumPost post);

   /// <summary>
   /// Check whether user is allowed to delete post
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="post">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToDeletePostAsync(User user, ForumPost post);

   /// <summary>
   /// Check whether user is allowed to set topic priority
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToSetTopicPriorityAsync(User user);

   /// <summary>
   /// Check whether user is allowed to watch topics
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if allowed, otherwise false
   /// </returns>
   Task<bool> IsUserAllowedToSubscribeAsync(User user);

   /// <summary>
   /// Calculates topic page index by post identifier
   /// </summary>
   /// <param name="forumTopicId">Topic identifier</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="postId">Post identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the page index
   /// </returns>
   Task<int> CalculateTopicPageIndexAsync(long forumTopicId, int pageSize, long postId);

   /// <summary>
   /// Get a post vote 
   /// </summary>
   /// <param name="postId">Post identifier</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the post vote
   /// </returns>
   Task<ForumPostVote> GetPostVoteAsync(long postId, User user);

   /// <summary>
   /// Get post vote made since the parameter date
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="createdFromUtc">Date</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the post votes count
   /// </returns>
   Task<int> GetNumberOfPostVotesAsync(User user, DateTime createdFromUtc);

   /// <summary>
   /// Insert a post vote
   /// </summary>
   /// <param name="postVote">Post vote</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertPostVoteAsync(ForumPostVote postVote);

   /// <summary>
   /// Delete a post vote
   /// </summary>
   /// <param name="postVote">Post vote</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeletePostVoteAsync(ForumPostVote postVote);

   /// <summary>
   /// Formats the forum post text
   /// </summary>
   /// <param name="forumPost">Forum post</param>
   /// <returns>Formatted text</returns>
   string FormatPostText(ForumPost forumPost);

   /// <summary>
   /// Strips the topic subject
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <returns>Formatted subject</returns>
   string StripTopicSubject(ForumTopic forumTopic);

   /// <summary>
   /// Formats the forum signature text
   /// </summary>
   /// <param name="text">Text</param>
   /// <returns>Formatted text</returns>
   string FormatForumSignatureText(string text);

   /// <summary>
   /// Formats the private message text
   /// </summary>
   /// <param name="pm">Private message</param>
   /// <returns>Formatted text</returns>
   string FormatPrivateMessageText(PrivateMessage pm);

   /// <summary>
   /// Get first post
   /// </summary>
   /// <param name="forumTopic">Forum topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum post
   /// </returns>
   Task<ForumPost> GetFirstPostAsync(ForumTopic forumTopic);

   /// <summary>
   /// Gets ForumGroup SE (search engine) name
   /// </summary>
   /// <param name="forumGroup">ForumGroup</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forumGroup SE (search engine) name
   /// </returns>
   Task<string> GetForumGroupSeNameAsync(ForumGroup forumGroup);

   /// <summary>
   /// Gets Forum SE (search engine) name
   /// </summary>
   /// <param name="forum">Forum</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum SE (search engine) name
   /// </returns>
   Task<string> GetForumSeNameAsync(Forum forum);

   /// <summary>
   /// Gets ForumTopic SE (search engine) name
   /// </summary>
   /// <param name="forumTopic">ForumTopic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forumTopic SE (search engine) name
   /// </returns>
   Task<string> GetTopicSeNameAsync(ForumTopic forumTopic);
}