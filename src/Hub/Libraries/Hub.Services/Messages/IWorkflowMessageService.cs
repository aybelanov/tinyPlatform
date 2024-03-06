using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Messages;

/// <summary>
/// Workflow message service
/// </summary>
public partial interface IWorkflowMessageService
{
   #region User workflow

   /// <summary>
   /// Sends 'New user' notification message to a platform owner
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendUserRegisteredNotificationMessageAsync(User user, long languageId);

   /// <summary>
   /// Sends a welcome message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendUserWelcomeMessageAsync(User user, long languageId);

   /// <summary>
   /// Sends an email validation message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendUserEmailValidationMessageAsync(User user, long languageId);

   /// <summary>
   /// Sends an email re-validation message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendUserEmailRevalidationMessageAsync(User user, long languageId);

   /// <summary>
   /// Sends password recovery message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendUserPasswordRecoveryMessageAsync(User user, long languageId);

   #endregion

   #region Device workflow

   /// <summary>
   /// Sends 'New device' notification message to a platform owner
   /// </summary>
   /// <param name="device">Device instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendDeviceRegisteredNotificationMessageAsync(Device device, long languageId);

   #endregion

   #region Newsletter workflow

   /// <summary>
   /// Sends a newsletter subscription activation message
   /// </summary>
   /// <param name="subscription">Newsletter subscription</param>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendNewsLetterSubscriptionActivationMessageAsync(NewsLetterSubscription subscription, long languageId);

   /// <summary>
   /// Sends a newsletter subscription deactivation message
   /// </summary>
   /// <param name="subscription">Newsletter subscription</param>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendNewsLetterSubscriptionDeactivationMessageAsync(NewsLetterSubscription subscription, long languageId);

   #endregion

   #region Send a message to a friend

   /// <summary>
   /// Sends wishlist "email a friend" message
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="languageId">Message language identifier</param>
   /// <param name="userEmail">User's email</param>
   /// <param name="friendsEmail">Friend's email</param>
   /// <param name="personalMessage">Personal message</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendEmailAFriendMessageAsync(User user, long languageId, string userEmail, string friendsEmail, string personalMessage);

   #endregion

   #region Forum Notifications

   /// <summary>
   /// Sends a forum subscription message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="forumTopic">Forum Topic</param>
   /// <param name="forum">Forum</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendNewForumTopicMessageAsync(User user, ForumTopic forumTopic, Forum forum, long languageId);

   /// <summary>
   /// Sends a forum subscription message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="forumPost">Forum post</param>
   /// <param name="forumTopic">Forum Topic</param>
   /// <param name="forum">Forum</param>
   /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendNewForumPostMessageAsync(User user, ForumPost forumPost, ForumTopic forumTopic, Forum forum, int friendlyForumTopicPageIndex, long languageId);

   /// <summary>
   /// Sends a private message notification
   /// </summary>
   /// <param name="privateMessage">Private message</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendPrivateMessageNotificationAsync(PrivateMessage privateMessage, long languageId);

   #endregion

   #region Misc

   /// <summary>
   /// Sends a "new VAT submitted" notification to a platform owner
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="vatName">Received VAT name</param>
   /// <param name="vatAddress">Received VAT address</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendNewVatSubmittedApplicationOwnerNotificationAsync(User user, string vatName, string vatAddress, long languageId);

   /// <summary>
   /// Sends a blog comment notification message to a platform owner
   /// </summary>
   /// <param name="blogComment">Blog comment</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendBlogCommentNotificationMessageAsync(BlogComment blogComment, long languageId);

   /// <summary>
   /// Sends a news comment notification message to a platform owner
   /// </summary>
   /// <param name="newsComment">News comment</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendNewsCommentNotificationMessageAsync(NewsComment newsComment, long languageId);

   /// <summary>
   /// Sends "contact us" message
   /// </summary>
   /// <param name="languageId">Message language identifier</param>
   /// <param name="senderEmail">Sender email</param>
   /// <param name="senderName">Sender name</param>
   /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
   /// <param name="body">Email body</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendContactUsMessageAsync(long languageId, string senderEmail, string senderName, string subject, string body);

   /// <summary>
   /// Sends a test email
   /// </summary>
   /// <param name="messageTemplateId">Message template identifier</param>
   /// <param name="sendToEmail">Send to email</param>
   /// <param name="tokens">Tokens</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<long> SendTestEmailAsync(long messageTemplateId, string sendToEmail, List<Token> tokens, long languageId);

   #endregion

   #region Common

   /// <summary>
   /// Send notification
   /// </summary>
   /// <param name="messageTemplate">Message template</param>
   /// <param name="emailAccount">Email account</param>
   /// <param name="languageId">Language identifier</param>
   /// <param name="tokens">Tokens</param>
   /// <param name="toEmailAddress">Recipient email address</param>
   /// <param name="toName">Recipient name</param>
   /// <param name="attachmentFilePath">Attachment file path</param>
   /// <param name="attachmentFileName">Attachment file name</param>
   /// <param name="replyToEmailAddress">"Reply to" email</param>
   /// <param name="replyToName">"Reply to" name</param>
   /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
   /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
   /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<long> SendNotificationAsync(MessageTemplate messageTemplate,
       EmailAccount emailAccount, long languageId, IList<Token> tokens,
       string toEmailAddress, string toName,
       string attachmentFilePath = null, string attachmentFileName = null,
       string replyToEmailAddress = null, string replyToName = null,
       string fromEmail = null, string fromName = null, string subject = null);

   #endregion

   /// <summary>
   /// Sends wishlist "email a friend" message
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="languageId">Message language identifier</param>
   /// <param name="userEmail">User's email</param>
   /// <param name="friendsEmail">Friend's email</param>
   /// <param name="personalMessage">Personal message</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   Task<IList<long>> SendWishlistEmailAFriendMessageAsync(User user, long languageId, string userEmail, string friendsEmail, string personalMessage);
}