using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Services.Blogs;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Forums;
using Hub.Services.Localization;
using Hub.Services.News;
using Hub.Services.Seo;
using Hub.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Hub.Services.Messages;

/// <summary>
/// Message token provider
/// </summary>
public partial class MessageTokenProvider : IMessageTokenProvider
{
   #region Fields

   private readonly AppInfoSettings _appSettings;
   private readonly IActionContextAccessor _actionContextAccessor;
   private readonly IBlogService _blogService;
   private readonly IUserAttributeFormatter _userAttributeFormatter;
   private readonly IHubDeviceService _deviceService; 
   private readonly IUserService _userService;
   private readonly IEventPublisher _eventPublisher;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly INewsService _newsService;
   private readonly IUrlHelperFactory _urlHelperFactory;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWorkContext _workContext;
   private readonly MessageTemplatesSettings _templatesSettings;

   private Dictionary<string, IEnumerable<string>> _allowedTokens;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public MessageTokenProvider(AppInfoSettings appSettings,
       IActionContextAccessor actionContextAccessor,
       IBlogService blogService,
       IUserAttributeFormatter userAttributeFormatter,
       IUserService userService,
       IHubDeviceService deviceService,
       IEventPublisher eventPublisher,
       IGenericAttributeService genericAttributeService,
       ILanguageService languageService,
       ILocalizationService localizationService,
       INewsService newsService,
       IUrlHelperFactory urlHelperFactory,
       IUrlRecordService urlRecordService,
       IWorkContext workContext,
       MessageTemplatesSettings templatesSettings)
   {
      _appSettings = appSettings;
      _actionContextAccessor = actionContextAccessor;
      _blogService = blogService;
      _userAttributeFormatter = userAttributeFormatter;
      _userService = userService;
      _eventPublisher = eventPublisher;
      _genericAttributeService = genericAttributeService;
      _languageService = languageService;
      _localizationService = localizationService;
      _newsService = newsService;
      _urlHelperFactory = urlHelperFactory;
      _urlRecordService = urlRecordService;
      _workContext = workContext;
      _templatesSettings = templatesSettings;
      _deviceService = deviceService;
   }

   #endregion

   #region Allowed tokens

   /// <summary>
   /// Get all available tokens by token groups
   /// </summary>
   protected Dictionary<string, IEnumerable<string>> AllowedTokens
   {
      get
      {
         if (_allowedTokens != null)
            return _allowedTokens;

         _allowedTokens = new Dictionary<string, IEnumerable<string>>
         {
            //store tokens
            {
               TokenGroupNames.AppTokens,
               new[]
               {
                 "%App.Name%",
                 "%App.URL%",
                 "%App.Email%",
                 "%App.CompanyName%",
                 "%App.CompanyAddress%",
                 "%App.CompanyPhoneNumber%",
                 "%App.CompanyVat%",
                 "%Facebook.URL%",
                 "%Twitter.URL%",
                 "%YouTube.URL%",
                 "%Vk.URL%",
                 "%Telegramm.URL%",
                 "%GitHub.URL%",
                 "%RuTube.URL%"
                }
            },

            //user tokens
            {
               TokenGroupNames.UserTokens,
               new[]
               {
                 "%User.Email%",
                 "%User.Username%",
                 "%User.FullName%",
                 "%User.FirstName%",
                 "%User.LastName%",
                 "%User.VatNumber%",
                 "%User.VatNumberStatus%",
                 "%User.CustomAttributes%",
                 "%User.PasswordRecoveryURL%",
                 "%User.AccountActivationURL%",
                 "%User.EmailRevalidationURL%",
                 "%Wishlist.URLForUser%"
               }
            },
            // device tokens
            {
               TokenGroupNames.DeviceTokens,
               new[]
               { 
                  "%Device.SystemName%",
                  "%Device.Name%"
               }
            },
            //newsletter subscription tokens
            {
               TokenGroupNames.SubscriptionTokens,
               new[]
               {
                 "%NewsLetterSubscription.Email%",
                 "%NewsLetterSubscription.ActivationUrl%",
                 "%NewsLetterSubscription.DeactivationUrl%"
               }
            },

            //forum tokens
            {
               TokenGroupNames.ForumTokens,
               new[]
               {
                 "%Forums.ForumURL%",
                 "%Forums.ForumName%"
               }
            },

            //forum topic tokens
            {
               TokenGroupNames.ForumTopicTokens,
               new[]
               {
                 "%Forums.TopicURL%",
                 "%Forums.TopicName%"
               }
            },

            //forum post tokens
            {
               TokenGroupNames.ForumPostTokens,
               new[]
               {
                 "%Forums.PostAuthor%",
                 "%Forums.PostBody%"
               }
            },

            //private message tokens
            {
               TokenGroupNames.PrivateMessageTokens,
               new[]
               {
                 "%PrivateMessage.Subject%",
                 "%PrivateMessage.Text%"
               }
            },

            //blog comment tokens
            {
               TokenGroupNames.BlogCommentTokens,
               new[]
               {
                 "%BlogComment.BlogPostTitle%"
               }
            },

            //news comment tokens
            {
               TokenGroupNames.NewsCommentTokens,
               new[]
               {
                 "%NewsComment.NewsTitle%"
               }
            },

            //email a friend tokens
            {
               TokenGroupNames.EmailAFriendTokens,
               new[]
               {
                 "%EmailAFriend.PersonalMessage%",
                 "%EmailAFriend.Email%"
               }
            },

            //wishlist to friend tokens
            {
               TokenGroupNames.WishlistToFriendTokens,
               new[]
               {
                 "%Wishlist.PersonalMessage%",
                 "%Wishlist.Email%"
               }
            },

            //VAT validation tokens
            {
               TokenGroupNames.VatValidation,
               new[]
               {
                 "%VatValidationResult.Name%",
                 "%VatValidationResult.Address%"
               }
            },

            //contact us tokens
            {
               TokenGroupNames.ContactUs,
               new[]
               {
                 "%ContactUs.SenderEmail%",
                 "%ContactUs.SenderName%",
                 "%ContactUs.Body%"
               }
            }
         };

         return _allowedTokens;
      }
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Generates an absolute URL for the platform, routeName and route values
   /// </summary>
   /// <param name="routeName">The name of the route that is used to generate URL</param>
   /// <param name="routeValues">An object that contains route values</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the generated URL
   /// </returns>
   protected virtual async Task<string> RouteUrlAsync(string routeName = null, object routeValues = null)
   {
      //ensure that the platfrom URL is specified
      if (string.IsNullOrEmpty(_appSettings.CompanyWebsiteUrl))
         throw new Exception("URL cannot be null");

      //generate a URL with an absolute path
      var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
      var url = new PathString(urlHelper.RouteUrl(routeName, routeValues));

      //remove the application path from the generated URL if exists
      var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
      url.StartsWithSegments(pathBase, out url);

      //compose the result
      return await Task.FromResult(new Uri(WebUtility.UrlDecode($"{_appSettings.CompanyWebsiteUrl.TrimEnd('/')}{url}"), UriKind.Absolute).AbsoluteUri);
   }

   #endregion

   #region Methods

   /// <summary>
   /// Add platform tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="emailAccount">Email account</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddApplicationTokensAsync(IList<Token> tokens, EmailAccount emailAccount)
   {
      if (emailAccount == null)
         throw new ArgumentNullException(nameof(emailAccount));

      tokens.Add(new Token("App.Name", _appSettings.Name));
      tokens.Add(new Token("App.URL", _appSettings.CompanyWebsiteUrl, true));
      tokens.Add(new Token("App.Email", emailAccount.Email));
      tokens.Add(new Token("App.CompanyName", _appSettings.CompanyName));
      tokens.Add(new Token("App.CompanyAddress", _appSettings.CompanyAddress));
      tokens.Add(new Token("App.CompanyPhoneNumber", _appSettings.CompanyPhoneNumber));
      tokens.Add(new Token("App.CompanyVat", _appSettings.CompanyVat));

      tokens.Add(new Token("Facebook.URL", _appSettings.FacebookLink));
      tokens.Add(new Token("Twitter.URL", _appSettings.TwitterLink));
      tokens.Add(new Token("YouTube.URL", _appSettings.YoutubeLink));
      tokens.Add(new Token("Telegram.URL", _appSettings.TelegramLink));
      tokens.Add(new Token("Ok.URL", _appSettings.OkLink));
      tokens.Add(new Token("Vk.URL", _appSettings.VkLink));
      tokens.Add(new Token("Rutube.URL", _appSettings.RutubeLink));
      tokens.Add(new Token("GitHub.URL", _appSettings.GitHubLink));

      //TODO event notification
      await Task.CompletedTask;
   }

   /// <summary>
   /// Add user tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="userId">User identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddUserTokensAsync(IList<Token> tokens, long userId)
   {
      if (userId <= 0)
         throw new ArgumentOutOfRangeException(nameof(userId));

      var user = await _userService.GetUserByIdAsync(userId);

      await AddUserTokensAsync(tokens, user);
   }

   /// <summary>
   /// Add user tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddUserTokensAsync(IList<Token> tokens, User user)
   {
      tokens.Add(new Token("User.Email", user.Email));
      tokens.Add(new Token("User.Username", user.Username));
      tokens.Add(new Token("User.FullName", await _userService.GetUserFullNameAsync(user)));
      tokens.Add(new Token("User.FirstName", await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FirstNameAttribute)));
      tokens.Add(new Token("User.LastName", await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastNameAttribute)));
      tokens.Add(new Token("User.VatNumber", await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.VatNumberAttribute)));
      tokens.Add(new Token("User.VatNumberStatus", (await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.VatNumberStatusIdAttribute)).ToString()));

      var customAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CustomUserAttributes);
      tokens.Add(new Token("User.CustomAttributes", await _userAttributeFormatter.FormatAttributesAsync(customAttributesXml), true));

      //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
      var passwordRecoveryUrl = await RouteUrlAsync(routeName: "PasswordRecoveryConfirm", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.PasswordRecoveryTokenAttribute), guid = user.UserGuid });
      var accountActivationUrl = await RouteUrlAsync(routeName: "AccountActivation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.AccountActivationTokenAttribute), guid = user.UserGuid });
      var emailRevalidationUrl = await RouteUrlAsync(routeName: "EmailRevalidation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.EmailRevalidationTokenAttribute), guid = user.UserGuid });
      var wishlistUrl = await RouteUrlAsync(routeName: "Wishlist", routeValues: new { userGuid = user.UserGuid });
      tokens.Add(new Token("User.PasswordRecoveryURL", passwordRecoveryUrl, true));
      tokens.Add(new Token("User.AccountActivationURL", accountActivationUrl, true));
      tokens.Add(new Token("User.EmailRevalidationURL", emailRevalidationUrl, true));
      tokens.Add(new Token("Wishlist.URLForUser", wishlistUrl, true));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(user, tokens);
   }

   /// <summary>
   /// Add device tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddDeviceTokensAsync(IList<Token> tokens, long deviceId)
   {
      if (deviceId <= 0)
         throw new ArgumentOutOfRangeException(nameof(deviceId));

      var device = await _deviceService.GetDeviceByIdAsync(deviceId);

      await AddDeviceTokensAsync(tokens, device);
   }

   /// <summary>
   /// Add device tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="device">Device</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddDeviceTokensAsync(IList<Token> tokens, Device device)
   {
      var user = await _userService.GetUserByIdAsync(device.OwnerId);

      tokens.Add(new Token("User.Email", user.Email));
      tokens.Add(new Token("User.Username", user.Username));
      tokens.Add(new Token("User.FullName", await _userService.GetUserFullNameAsync(user)));
      tokens.Add(new Token("User.FirstName", await _genericAttributeService.GetAttributeAsync<string>(device, AppUserDefaults.FirstNameAttribute)));
      tokens.Add(new Token("User.LastName", await _genericAttributeService.GetAttributeAsync<string>(device, AppUserDefaults.LastNameAttribute)));
      tokens.Add(new Token("Device.SystemName", device.SystemName));
      
      //event notification
      await _eventPublisher.EntityTokensAddedAsync(device, tokens);
   }


   /// <summary>
   /// Add newsletter subscription tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="subscription">Newsletter subscription</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription)
   {
      tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

      var activationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "true" });
      tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

      var deactivationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "false" });
      tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deactivationUrl, true));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(subscription, tokens);
   }

   /// <summary>
   /// Add blog comment tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="blogComment">Blog post comment</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment)
   {
      var blogPost = await _blogService.GetBlogPostByIdAsync(blogComment.BlogPostId);

      tokens.Add(new Token("BlogComment.BlogPostTitle", blogPost.Title));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(blogComment, tokens);
   }

   /// <summary>
   /// Add news comment tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="newsComment">News comment</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment)
   {
      var newsItem = await _newsService.GetNewsByIdAsync(newsComment.NewsItemId);

      tokens.Add(new Token("NewsComment.NewsTitle", newsItem.Title));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(newsComment, tokens);
   }

   /// <summary>
   /// Add forum topic tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="forumTopic">Forum topic</param>
   /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
   /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
       int? friendlyForumTopicPageIndex = null, long? appendedPostIdentifierAnchor = null)
   {
      //attributes
      //we cannot inject IForumService into constructor because it'll cause circular references.
      //that's why we resolve it here this way
      var forumService = EngineContext.Current.Resolve<IForumService>();

      string topicUrl;
      if (friendlyForumTopicPageIndex.HasValue && friendlyForumTopicPageIndex.Value > 1)
         topicUrl = await RouteUrlAsync(routeName: "TopicSlugPaged", routeValues: new { id = forumTopic.Id, slug = await forumService.GetTopicSeNameAsync(forumTopic), pageNumber = friendlyForumTopicPageIndex.Value });
      else
         topicUrl = await RouteUrlAsync(routeName: "TopicSlug", routeValues: new { id = forumTopic.Id, slug = await forumService.GetTopicSeNameAsync(forumTopic) });
      if (appendedPostIdentifierAnchor.HasValue && appendedPostIdentifierAnchor.Value > 0)
         topicUrl = $"{topicUrl}#{appendedPostIdentifierAnchor.Value}";
      tokens.Add(new Token("Forums.TopicURL", topicUrl, true));
      tokens.Add(new Token("Forums.TopicName", forumTopic.Subject));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(forumTopic, tokens);
   }

   /// <summary>
   /// Add forum post tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="forumPost">Forum post</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost)
   {
      //attributes
      //we cannot inject IForumService into constructor because it'll cause circular references.
      //that's why we resolve it here this way
      var forumService = EngineContext.Current.Resolve<IForumService>();

      var user = await _userService.GetUserByIdAsync(forumPost.UserId);

      tokens.Add(new Token("Forums.PostAuthor", await _userService.FormatUsernameAsync(user)));
      tokens.Add(new Token("Forums.PostBody", forumService.FormatPostText(forumPost), true));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(forumPost, tokens);
   }

   /// <summary>
   /// Add forum tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="forum">Forum</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddForumTokensAsync(IList<Token> tokens, Forum forum)
   {
      //attributes
      //we cannot inject IForumService into constructor because it'll cause circular references.
      //that's why we resolve it here this way
      var forumService = EngineContext.Current.Resolve<IForumService>();

      var forumUrl = await RouteUrlAsync(routeName: "ForumSlug", routeValues: new { id = forum.Id, slug = await forumService.GetForumSeNameAsync(forum) });
      tokens.Add(new Token("Forums.ForumURL", forumUrl, true));
      tokens.Add(new Token("Forums.ForumName", forum.Name));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(forum, tokens);
   }

   /// <summary>
   /// Add private message tokens
   /// </summary>
   /// <param name="tokens">List of already added tokens</param>
   /// <param name="privateMessage">Private message</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage)
   {
      //attributes
      //we cannot inject IForumService into constructor because it'll cause circular references.
      //that's why we resolve it here this way
      var forumService = EngineContext.Current.Resolve<IForumService>();

      tokens.Add(new Token("PrivateMessage.Subject", privateMessage.Subject));
      tokens.Add(new Token("PrivateMessage.Text", forumService.FormatPrivateMessageText(privateMessage), true));

      //event notification
      await _eventPublisher.EntityTokensAddedAsync(privateMessage, tokens);
   }

   
   /// <summary>
   /// Get collection of allowed (supported) message tokens for campaigns
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the collection of allowed (supported) message tokens for campaigns
   /// </returns>
   public virtual async Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync()
   {
      var additionalTokens = new CampaignAdditionalTokensAddedEvent();
      await _eventPublisher.PublishAsync(additionalTokens);

      var allowedTokens = (await GetListOfAllowedTokensAsync(new[] { TokenGroupNames.AppTokens, TokenGroupNames.SubscriptionTokens })).ToList();
      allowedTokens.AddRange(additionalTokens.AdditionalTokens);

      return allowedTokens.Distinct();
   }

   /// <summary>
   /// Get collection of allowed (supported) message tokens
   /// </summary>
   /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the collection of allowed message tokens
   /// </returns>
   public virtual async Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null)
   {
      var additionalTokens = new AdditionalTokensAddedEvent();
      await _eventPublisher.PublishAsync(additionalTokens);

      var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
          .SelectMany(x => x.Value).ToList();

      allowedTokens.AddRange(additionalTokens.AdditionalTokens);

      return allowedTokens.Distinct();
   }

   /// <summary>
   /// Get token groups of message template
   /// </summary>
   /// <param name="messageTemplate">Message template</param>
   /// <returns>Collection of token group names</returns>
   public virtual IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate)
   {
      //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
      return messageTemplate.Name switch
      {
         MessageTemplateSystemNames.UserRegisteredNotification or
         MessageTemplateSystemNames.UserWelcomeMessage or
         MessageTemplateSystemNames.UserEmailValidationMessage or
         MessageTemplateSystemNames.UserEmailRevalidationMessage or
         MessageTemplateSystemNames.UserPasswordRecoveryMessage => new[] { TokenGroupNames.AppTokens, TokenGroupNames.UserTokens },

         MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage or
         MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage => new[] { TokenGroupNames.AppTokens, TokenGroupNames.SubscriptionTokens },

         MessageTemplateSystemNames.WishlistToFriendMessage => new[] { TokenGroupNames.AppTokens, TokenGroupNames.UserTokens, TokenGroupNames.WishlistToFriendTokens },

         MessageTemplateSystemNames.NewForumTopicMessage => new[] { TokenGroupNames.AppTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.UserTokens },
         MessageTemplateSystemNames.NewForumPostMessage => new[] { TokenGroupNames.AppTokens, TokenGroupNames.ForumPostTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.UserTokens },
         MessageTemplateSystemNames.PrivateMessageNotification => new[] { TokenGroupNames.AppTokens, TokenGroupNames.PrivateMessageTokens, TokenGroupNames.UserTokens },

         MessageTemplateSystemNames.NewVatSubmittedPlatformOwnerNotification => new[] { TokenGroupNames.AppTokens, TokenGroupNames.UserTokens, TokenGroupNames.VatValidation },
         MessageTemplateSystemNames.BlogCommentNotification => new[] { TokenGroupNames.AppTokens, TokenGroupNames.BlogCommentTokens, TokenGroupNames.UserTokens },
         MessageTemplateSystemNames.NewsCommentNotification => new[] { TokenGroupNames.AppTokens, TokenGroupNames.NewsCommentTokens, TokenGroupNames.UserTokens },
         MessageTemplateSystemNames.ContactUsMessage => new[] { TokenGroupNames.AppTokens, TokenGroupNames.ContactUs },
         _ => Array.Empty<string>(),
      };
   }

   #endregion
}