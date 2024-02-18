using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Data.Extensions;
using Hub.Services.Common;
using Hub.Services.Localization;
using Hub.Services.Users;

namespace Hub.Services.Messages;

/// <summary>
/// Workflow message service
/// </summary>
public partial class WorkflowMessageService : IWorkflowMessageService
{
   #region Fields

   private readonly CommonSettings _commonSettings;
   private readonly EmailAccountSettings _emailAccountSettings;
   private readonly IAddressService _addressService;
   private readonly IUserService _userService;
   private readonly IEmailAccountService _emailAccountService;
   private readonly IEventPublisher _eventPublisher;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly IMessageTemplateService _messageTemplateService;
   private readonly IMessageTokenProvider _messageTokenProvider;
   private readonly IQueuedEmailService _queuedEmailService;
   private readonly ITokenizer _tokenizer;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public WorkflowMessageService(CommonSettings commonSettings,
       EmailAccountSettings emailAccountSettings,
       IAddressService addressService,
       IUserService userService,
       IEmailAccountService emailAccountService,
       IEventPublisher eventPublisher,
       ILanguageService languageService,
       ILocalizationService localizationService,
       IMessageTemplateService messageTemplateService,
       IMessageTokenProvider messageTokenProvider,
       IQueuedEmailService queuedEmailService,
       ITokenizer tokenizer)
   {
      _commonSettings = commonSettings;
      _emailAccountSettings = emailAccountSettings;
      _addressService = addressService;
      _userService = userService;
      _emailAccountService = emailAccountService;
      _eventPublisher = eventPublisher;
      _languageService = languageService;
      _localizationService = localizationService;
      _messageTemplateService = messageTemplateService;
      _messageTokenProvider = messageTokenProvider;
      _queuedEmailService = queuedEmailService;
      _tokenizer = tokenizer;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Get active message templates by the name
   /// </summary>
   /// <param name="messageTemplateName">Message template name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of message templates
   /// </returns>
   protected virtual async Task<IList<MessageTemplate>> GetActiveMessageTemplatesAsync(string messageTemplateName)
   {
      //get message templates by the name
      var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(messageTemplateName);

      //no template found
      if (!messageTemplates?.Any() ?? true)
         return new List<MessageTemplate>();

      //filter active templates
      messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

      return messageTemplates;
   }

   /// <summary>
   /// Get EmailAccount to use with a message templates
   /// </summary>
   /// <param name="messageTemplate">Message template</param>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the emailAccount
   /// </returns>
   protected virtual async Task<EmailAccount> GetEmailAccountOfMessageTemplateAsync(MessageTemplate messageTemplate, long languageId)
   {
      var emailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.EmailAccountId, languageId);
      //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
      if (emailAccountId == 0)
         emailAccountId = messageTemplate.EmailAccountId;

      var emailAccount = (await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId) ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)) ??
                         (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
      return emailAccount;
   }

   /// <summary>
   /// Ensure language is active
   /// </summary>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the return a value language identifier
   /// </returns>
   protected virtual async Task<long> EnsureLanguageIsActiveAsync(long languageId)
   {
      //load language by specified ID
      var language = await _languageService.GetLanguageByIdAsync(languageId);

      if (language == null || !language.Published)
      {
         language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
      }

      if (language == null || !language.Published)
      {
         //load any language
         language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
      }

      if (language == null)
         throw new Exception("No active language could be loaded");

      return language.Id;
   }

   #endregion

   #region Methods

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
   public virtual async Task<IList<long>> SendUserRegisteredNotificationMessageAsync(User user, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserRegisteredNotification);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = emailAccount.Email;
         var toName = emailAccount.DisplayName;

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends 'New device' notification message to a platform owner
   /// </summary>
   /// <param name="device">Device instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifiers
   /// </returns>
   public virtual async Task<IList<long>> SendDeviceRegisteredNotificationMessageAsync(Device device, long languageId)
   {
      if (device == null)
         throw new ArgumentNullException(nameof(device));

      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.DeviceRegisteredNotification);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddDeviceTokensAsync(commonTokens, device);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = emailAccount.Email;
         var toName = emailAccount.DisplayName;

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends a welcome message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendUserWelcomeMessageAsync(User user, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserWelcomeMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = user.Email;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends an email validation message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendUserEmailValidationMessageAsync(User user, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserEmailValidationMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = user.Email;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends an email re-validation message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendUserEmailRevalidationMessageAsync(User user, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserEmailRevalidationMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         //email to re-validate
         var toEmail = user.EmailToRevalidate;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends password recovery message to a user
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendUserPasswordRecoveryMessageAsync(User user, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserPasswordRecoveryMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = user.Email;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

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
   public virtual async Task<IList<long>> SendNewsLetterSubscriptionActivationMessageAsync(NewsLetterSubscription subscription, long languageId)
   {
      if (subscription == null)
         throw new ArgumentNullException(nameof(subscription));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(commonTokens, subscription);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends a newsletter subscription deactivation message
   /// </summary>
   /// <param name="subscription">Newsletter subscription</param>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendNewsLetterSubscriptionDeactivationMessageAsync(NewsLetterSubscription subscription, long languageId)
   {
      if (subscription == null)
         throw new ArgumentNullException(nameof(subscription));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(commonTokens, subscription);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
      }).ToListAsync();
   }

   #endregion

   #region Send a message to a friend

   /// <summary>
   /// Sends "email a friend" message
   /// </summary>
   /// <param name="user">User instance</param>
   /// <param name="languageId">Message language identifier</param>
   /// <param name="userEmail">User's email</param>
   /// <param name="friendsEmail">Friend's email</param>
   /// <param name="personalMessage">Personal message</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendEmailAFriendMessageAsync(User user, long languageId,string userEmail, string friendsEmail, string personalMessage)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.EmailAFriendMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
      commonTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
      commonTokens.Add(new Token("EmailAFriend.Email", userEmail));

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
      }).ToListAsync();
   }

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
   public virtual async Task<IList<long>> SendWishlistEmailAFriendMessageAsync(User user, long languageId,
        string userEmail, string friendsEmail, string personalMessage)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.WishlistToFriendMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
      commonTokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
      commonTokens.Add(new Token("Wishlist.Email", userEmail));

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
      }).ToListAsync();
   }

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
   public virtual async Task<IList<long>> SendNewForumTopicMessageAsync(User user, ForumTopic forumTopic, Forum forum, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewForumTopicMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddForumTopicTokensAsync(commonTokens, forumTopic);
      await _messageTokenProvider.AddForumTokensAsync(commonTokens, forum);
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = user.Email;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

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
   public virtual async Task<IList<long>> SendNewForumPostMessageAsync(User user, ForumPost forumPost, ForumTopic forumTopic,
       Forum forum, int friendlyForumTopicPageIndex, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewForumPostMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddForumPostTokensAsync(commonTokens, forumPost);
      await _messageTokenProvider.AddForumTopicTokensAsync(commonTokens, forumTopic, friendlyForumTopicPageIndex, forumPost.Id);
      await _messageTokenProvider.AddForumTokensAsync(commonTokens, forum);
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = user.Email;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends a private message notification
   /// </summary>
   /// <param name="privateMessage">Private message</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendPrivateMessageNotificationAsync(PrivateMessage privateMessage, long languageId)
   {
      if (privateMessage == null)
         throw new ArgumentNullException(nameof(privateMessage));

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.PrivateMessageNotification);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddPrivateMessageTokensAsync(commonTokens, privateMessage);
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, privateMessage.ToUserId);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var user = await _userService.GetUserByIdAsync(privateMessage.ToUserId);
         var toEmail = user.Email;
         var toName = await _userService.GetUserFullNameAsync(user);

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

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
   public virtual async Task<IList<long>> SendNewVatSubmittedApplicationOwnerNotificationAsync(User user, string vatName, string vatAddress, long languageId)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));
      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewVatSubmittedPlatformOwnerNotification);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
      commonTokens.Add(new Token("VatValidatio.Name", vatName));
      commonTokens.Add(new Token("VatValidatio.Address", vatAddress));

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = emailAccount.Email;
         var toName = emailAccount.DisplayName;

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends a blog comment notification message to a platform owner
   /// </summary>
   /// <param name="blogComment">Blog comment</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of queued email identifiers
   /// </returns>
   public virtual async Task<IList<long>> SendBlogCommentNotificationMessageAsync(BlogComment blogComment, long languageId)
   {
      if (blogComment == null)
         throw new ArgumentNullException(nameof(blogComment));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.BlogCommentNotification);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddBlogCommentTokensAsync(commonTokens, blogComment);
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, blogComment.UserId);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = emailAccount.Email;
         var toName = emailAccount.DisplayName;

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

   /// <summary>
   /// Sends a news comment notification message to a platform owner
   /// </summary>
   /// <param name="newsComment">News comment</param>
   /// <param name="languageId">Message language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the queued email identifier
   /// </returns>
   public virtual async Task<IList<long>> SendNewsCommentNotificationMessageAsync(NewsComment newsComment, long languageId)
   {
      if (newsComment == null)
         throw new ArgumentNullException(nameof(newsComment));

      
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewsCommentNotification);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>();
      await _messageTokenProvider.AddNewsCommentTokensAsync(commonTokens, newsComment);
      await _messageTokenProvider.AddUserTokensAsync(commonTokens, newsComment.UserId);

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = emailAccount.Email;
         var toName = emailAccount.DisplayName;

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
      }).ToListAsync();
   }

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
   public virtual async Task<IList<long>> SendContactUsMessageAsync(long languageId, string senderEmail, string senderName, string subject, string body)
   {
      languageId = await EnsureLanguageIsActiveAsync(languageId);

      var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ContactUsMessage);
      if (!messageTemplates.Any())
         return new List<long>();

      //tokens
      var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName)
            };

      return await messageTemplates.SelectAwait(async messageTemplate =>
      {
         //email account
         var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

         var tokens = new List<Token>(commonTokens);
         await _messageTokenProvider.AddApplicationTokensAsync(tokens, emailAccount);

         string fromEmail;
         string fromName;
         //required for some SMTP servers
         if (_commonSettings.UseSystemEmailForContactUsForm)
         {
            fromEmail = emailAccount.Email;
            fromName = emailAccount.DisplayName;
            body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
         }
         else
         {
            fromEmail = senderEmail;
            fromName = senderName;
         }

         tokens.Add(new Token("ContactUs.Body", body, true));

         //event notification
         await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

         var toEmail = emailAccount.Email;
         var toName = emailAccount.DisplayName;

         return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
             fromEmail: fromEmail,
             fromName: fromName,
             subject: subject,
             replyToEmailAddress: senderEmail,
             replyToName: senderName);
      }).ToListAsync();
   }

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
   public virtual async Task<long> SendTestEmailAsync(long messageTemplateId, string sendToEmail, List<Token> tokens, long languageId)
   {
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(messageTemplateId);
      if (messageTemplate == null)
         throw new ArgumentException("Template cannot be loaded");

      //email account
      var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

      //event notification
      await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

      //force sending
      messageTemplate.DelayBeforeSend = null;

      return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null);
   }

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
   public virtual async Task<long> SendNotificationAsync(MessageTemplate messageTemplate,
       EmailAccount emailAccount, long languageId, IList<Token> tokens,
       string toEmailAddress, string toName,
       string attachmentFilePath = null, string attachmentFileName = null,
       string replyToEmailAddress = null, string replyToName = null,
       string fromEmail = null, string fromName = null, string subject = null)
   {
      if (messageTemplate == null)
         throw new ArgumentNullException(nameof(messageTemplate));

      if (emailAccount == null)
         throw new ArgumentNullException(nameof(emailAccount));

      //retrieve localized message template data
      var bcc = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.BccEmailAddresses, languageId);
      if (string.IsNullOrEmpty(subject))
         subject = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Subject, languageId);
      var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);

      //Replace subject and body tokens 
      var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
      var bodyReplaced = _tokenizer.Replace(body, tokens, true);

      //limit name length
      toName = CommonHelper.EnsureMaximumLength(toName, 300);

      var email = new QueuedEmail
      {
         Priority = QueuedEmailPriority.High,
         From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
         FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
         To = toEmailAddress,
         ToName = toName,
         ReplyTo = replyToEmailAddress,
         ReplyToName = replyToName,
         CC = string.Empty,
         Bcc = bcc,
         Subject = subjectReplaced,
         Body = bodyReplaced,
         AttachmentFilePath = attachmentFilePath,
         AttachmentFileName = attachmentFileName,
         AttachedDownloadId = messageTemplate.AttachedDownloadId,
         CreatedOnUtc = DateTime.UtcNow,
         EmailAccountId = emailAccount.Id,
         DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
              : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
      };

      await _queuedEmailService.InsertQueuedEmailAsync(email);
      return email.Id;
   }

   #endregion

   #endregion
}