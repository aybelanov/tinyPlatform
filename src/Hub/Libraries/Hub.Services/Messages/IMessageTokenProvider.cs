using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Users;

namespace Hub.Services.Messages;

/// <summary>
/// Message token provider
/// </summary>
public partial interface IMessageTokenProvider
{
   /// <summary>
   /// Add store tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="emailAccount">Email account</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddApplicationTokensAsync(IList<Token> tokens, EmailAccount emailAccount);

   /// <summary>
   /// Add user tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="userId">User identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddUserTokensAsync(IList<Token> tokens, long userId);

   /// <summary>
   /// Add user tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddUserTokensAsync(IList<Token> tokens, User user);

   /// <summary>
   /// Add newsletter subscription tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="subscription">Newsletter subscription</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription);

   /// <summary>
   /// Get collection of allowed (supported) message tokens for campaigns
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the collection of allowed (supported) message tokens for campaigns
   /// </returns>
   Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync();

   /// <summary>
   /// Get collection of allowed (supported) message tokens
   /// </summary>
   /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the collection of allowed message tokens
   /// </returns>
   Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null);

   /// <summary>
   /// Get token groups of message template
   /// </summary>
   /// <param name="messageTemplate">Message template</param>
   /// <returns>Collection of token group names</returns>
   IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);

   /// <summary>
   /// Add private message tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="privateMessage">Private message</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage);

   /// <summary>
   /// Add forum tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="forum">Forum</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddForumTokensAsync(IList<Token> tokens, Forum forum);

   /// <summary>
   /// Add forum post tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="forumPost">Forum post</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost);

   /// <summary>
   /// Add forum topic tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="forumTopic">Forum topic</param>
   /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
   /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic, int? friendlyForumTopicPageIndex = null, long? appendedPostIdentifierAnchor = null);


   /// <summary>
   /// Add blog comment tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="blogComment">Blog post comment</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment);

   /// <summary>
   /// Add news comment tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="newsComment">News comment</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment);

   /// <summary>
   /// Add device tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="device">Device</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddDeviceTokensAsync(IList<Token> tokens, Device device);

   /// <summary>
   /// Add device tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task AddDeviceTokensAsync(IList<Token> tokens, long deviceId);
}