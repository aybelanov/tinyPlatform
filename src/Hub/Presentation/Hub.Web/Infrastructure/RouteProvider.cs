using Hub.Services.Installation;
using Hub.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Hub.Web.Infrastructure;

/// <summary>
/// Represents provider that provided basic routes
/// </summary>
public partial class RouteProvider : BaseRouteProvider, IRouteProvider
{
   #region Methods

   /// <summary>
   /// Register routes
   /// </summary>
   /// <param name="endpointRouteBuilder">Route builder</param>
   public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
   {
      //get language pattern
      //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
      //use it only for URLs of pages that the user can go to
      var lang = GetLanguageRoutePattern();

      //areas
      endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
          pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

      //home page
      endpointRouteBuilder.MapControllerRoute(name: "Homepage",
          pattern: $"{lang}",
          defaults: new { controller = "Home", action = "Index" });

      //login
      endpointRouteBuilder.MapControllerRoute(name: "Login",
          pattern: $"{lang}/login/",
          defaults: new { controller = "User", action = "Login" });

      // multi-factor verification digit code page
      endpointRouteBuilder.MapControllerRoute(name: "MultiFactorVerification",
          pattern: $"{lang}/multi-factor-verification/",
          defaults: new { controller = "User", action = "MultiFactorVerification" });

      //register
      endpointRouteBuilder.MapControllerRoute(name: "Register",
          pattern: $"{lang}/register/",
          defaults: new { controller = "User", action = "Register" });

      //logout
      endpointRouteBuilder.MapControllerRoute(name: "Logout",
          pattern: $"{lang}/logout/",
          defaults: new { controller = "User", action = "Logout" });

      //user account links
      endpointRouteBuilder.MapControllerRoute(name: "UserInfo",
          pattern: $"{lang}/user/info",
          defaults: new { controller = "User", action = "Info" });

      endpointRouteBuilder.MapControllerRoute(name: "UserAddresses",
          pattern: $"{lang}/user/addresses",
          defaults: new { controller = "User", action = "Addresses" });

      //contact us
      endpointRouteBuilder.MapControllerRoute(name: "ContactUs",
          pattern: $"{lang}/contactus",
          defaults: new { controller = "Common", action = "ContactUs" });

      //change currency
      endpointRouteBuilder.MapControllerRoute(name: "ChangeCurrency",
          pattern: $"{lang}/changecurrency/{{usercurrency:min(0)}}",
          defaults: new { controller = "Common", action = "SetCurrency" });

      //change language
      endpointRouteBuilder.MapControllerRoute(name: "ChangeLanguage",
          pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
          defaults: new { controller = "Common", action = "SetLanguage" });

      //blog
      endpointRouteBuilder.MapControllerRoute(name: "Blog",
          pattern: $"{lang}/blog",
          defaults: new { controller = "Blog", action = "List" });

      //news
      endpointRouteBuilder.MapControllerRoute(name: "NewsArchive",
          pattern: $"{lang}/news",
          defaults: new { controller = "News", action = "List" });

      //forum
      endpointRouteBuilder.MapControllerRoute(name: "Boards",
          pattern: $"{lang}/boards",
          defaults: new { controller = "Boards", action = "Index" });

      //downloads (file result)
      endpointRouteBuilder.MapControllerRoute(name: "GetSampleDownload",
          pattern: $"download/sample/{{productid:min(0)}}",
          defaults: new { controller = "Download", action = "Sample" });

      //subscribe newsletters (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "SubscribeNewsletter",
          pattern: $"subscribenewsletter",
          defaults: new { controller = "Newsletter", action = "SubscribeNewsletter" });

      //login page for checkout as guest
      endpointRouteBuilder.MapControllerRoute(name: "LoginCheckoutAsGuest",
          pattern: $"{lang}/login/checkoutasguest",
          defaults: new { controller = "User", action = "Login", checkoutAsGuest = true });

      //register result page
      endpointRouteBuilder.MapControllerRoute(name: "RegisterResult",
          pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
          defaults: new { controller = "User", action = "RegisterResult" });

      //check username availability (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "CheckUsernameAvailability",
          pattern: $"user/checkusernameavailability",
          defaults: new { controller = "User", action = "CheckUsernameAvailability" });

      //passwordrecovery
      endpointRouteBuilder.MapControllerRoute(name: "PasswordRecovery",
          pattern: $"{lang}/passwordrecovery",
          defaults: new { controller = "User", action = "PasswordRecovery" });

      //password recovery confirmation
      endpointRouteBuilder.MapControllerRoute(name: "PasswordRecoveryConfirm",
          pattern: $"{lang}/passwordrecovery/confirm",
          defaults: new { controller = "User", action = "PasswordRecoveryConfirm" });

      //topics (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "TopicPopup",
          pattern: $"t-popup/{{SystemName}}",
          defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

      //blog
      endpointRouteBuilder.MapControllerRoute(name: "BlogByTag",
          pattern: $"{lang}/blog/tag/{{tag}}",
          defaults: new { controller = "Blog", action = "BlogByTag" });

      endpointRouteBuilder.MapControllerRoute(name: "BlogByMonth",
          pattern: $"{lang}/blog/month/{{month}}",
          defaults: new { controller = "Blog", action = "BlogByMonth" });

      //blog RSS (file result)
      endpointRouteBuilder.MapControllerRoute(name: "BlogRSS",
          pattern: $"blog/rss/{{languageId:min(0)}}",
          defaults: new { controller = "Blog", action = "ListRss" });

      //news RSS (file result)
      endpointRouteBuilder.MapControllerRoute(name: "NewsRSS",
          pattern: $"news/rss/{{languageId:min(0)}}",
          defaults: new { controller = "News", action = "ListRss" });

      endpointRouteBuilder.MapControllerRoute(name: "UserChangePassword",
          pattern: $"{lang}/user/changepassword",
          defaults: new { controller = "User", action = "ChangePassword" });

      endpointRouteBuilder.MapControllerRoute(name: "UserAvatar",
          pattern: $"{lang}/user/avatar",
          defaults: new { controller = "User", action = "Avatar" });

      endpointRouteBuilder.MapControllerRoute(name: "AccountActivation",
          pattern: $"{lang}/user/activation",
          defaults: new { controller = "User", action = "AccountActivation" });

      endpointRouteBuilder.MapControllerRoute(name: "EmailRevalidation",
          pattern: $"{lang}/user/revalidateemail",
          defaults: new { controller = "User", action = "EmailRevalidation" });

      endpointRouteBuilder.MapControllerRoute(name: "UserForumSubscriptions",
          pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
          defaults: new { controller = "Boards", action = "UserForumSubscriptions" });

      endpointRouteBuilder.MapControllerRoute(name: "UserAddressEdit",
          pattern: $"{lang}/user/addressedit/{{addressId:min(0)}}",
          defaults: new { controller = "User", action = "AddressEdit" });

      endpointRouteBuilder.MapControllerRoute(name: "UserAddressAdd",
          pattern: $"{lang}/user/addressadd",
          defaults: new { controller = "User", action = "AddressAdd" });

      endpointRouteBuilder.MapControllerRoute(name: "UserMultiFactorAuthenticationProviderConfig",
          pattern: $"{lang}/user/providerconfig",
          defaults: new { controller = "User", action = "ConfigureMultiFactorAuthenticationProvider" });

      //user profile page
      endpointRouteBuilder.MapControllerRoute(name: "UserProfile",
          pattern: $"{lang}/profile/{{id:min(0)}}",
          defaults: new { controller = "Profile", action = "Index" });

      endpointRouteBuilder.MapControllerRoute(name: "UserProfilePaged",
          pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
          defaults: new { controller = "Profile", action = "Index" });

      //user GDPR
      endpointRouteBuilder.MapControllerRoute(name: "GdprTools",
          pattern: $"{lang}/user/gdpr",
          defaults: new { controller = "User", action = "GdprTools" });

      //user multi-factor authentication settings 
      endpointRouteBuilder.MapControllerRoute(name: "MultiFactorAuthenticationSettings",
          pattern: $"{lang}/user/multifactorauthentication",
          defaults: new { controller = "User", action = "MultiFactorAuthentication" });

      //poll vote (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "PollVote",
          pattern: $"poll/vote",
          defaults: new { controller = "Poll", action = "Vote" });

      //get state list by country ID (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "GetStatesByCountryId",
          pattern: $"country/getstatesbycountryid/",
          defaults: new { controller = "Country", action = "GetStatesByCountryId" });

      //EU Cookie law accept button handler (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "EuCookieLawAccept",
          pattern: $"eucookielawaccept",
          defaults: new { controller = "Common", action = "EuCookieLawAccept" });

      //authenticate topic (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "TopicAuthenticate",
          pattern: $"topic/authenticate",
          defaults: new { controller = "Topic", action = "Authenticate" });

      //forums
      endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussions",
          pattern: $"{lang}/boards/activediscussions",
          defaults: new { controller = "Boards", action = "ActiveDiscussions" });

      endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussionsPaged",
          pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
          defaults: new { controller = "Boards", action = "ActiveDiscussions" });

      //forums RSS (file result)
      endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussionsRSS",
          pattern: $"boards/activediscussionsrss",
          defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

      endpointRouteBuilder.MapControllerRoute(name: "PostEdit",
          pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "PostEdit" });

      endpointRouteBuilder.MapControllerRoute(name: "PostDelete",
          pattern: $"{lang}/boards/postdelete/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "PostDelete" });

      endpointRouteBuilder.MapControllerRoute(name: "PostCreate",
          pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "PostCreate" });

      endpointRouteBuilder.MapControllerRoute(name: "PostCreateQuote",
          pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
          defaults: new { controller = "Boards", action = "PostCreate" });

      endpointRouteBuilder.MapControllerRoute(name: "TopicEdit",
          pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "TopicEdit" });

      endpointRouteBuilder.MapControllerRoute(name: "TopicDelete",
          pattern: $"{lang}/boards/topicdelete/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "TopicDelete" });

      endpointRouteBuilder.MapControllerRoute(name: "TopicCreate",
          pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "TopicCreate" });

      endpointRouteBuilder.MapControllerRoute(name: "TopicMove",
          pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "TopicMove" });

      //topic watch (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "TopicWatch",
          pattern: $"boards/topicwatch/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "TopicWatch" });

      endpointRouteBuilder.MapControllerRoute(name: "TopicSlug",
          pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
          defaults: new { controller = "Boards", action = "Topic" });

      endpointRouteBuilder.MapControllerRoute(name: "TopicSlugPaged",
          pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
          defaults: new { controller = "Boards", action = "Topic" });

      //forum watch (AJAX)
      endpointRouteBuilder.MapControllerRoute(name: "ForumWatch",
          pattern: $"boards/forumwatch/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "ForumWatch" });

      //forums RSS (file result)
      endpointRouteBuilder.MapControllerRoute(name: "ForumRSS",
          pattern: $"boards/forumrss/{{id:min(0)}}",
          defaults: new { controller = "Boards", action = "ForumRSS" });

      endpointRouteBuilder.MapControllerRoute(name: "ForumSlug",
          pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
          defaults: new { controller = "Boards", action = "Forum" });

      endpointRouteBuilder.MapControllerRoute(name: "ForumSlugPaged",
          pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
          defaults: new { controller = "Boards", action = "Forum" });

      endpointRouteBuilder.MapControllerRoute(name: "ForumGroupSlug",
          pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
          defaults: new { controller = "Boards", action = "ForumGroup" });

      endpointRouteBuilder.MapControllerRoute(name: "Search",
          pattern: $"{lang}/boards/search",
          defaults: new { controller = "Boards", action = "Search" });

      //private messages
      endpointRouteBuilder.MapControllerRoute(name: "PrivateMessages",
          pattern: $"{lang}/privatemessages/{{tab?}}",
          defaults: new { controller = "PrivateMessages", action = "Index" });

      endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesPaged",
          pattern: $"{lang}/privatemessages/{{tab?}}/page/{{pageNumber:min(0)}}",
          defaults: new { controller = "PrivateMessages", action = "Index" });

      endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesInbox",
          pattern: $"{lang}/inboxupdate",
          defaults: new { controller = "PrivateMessages", action = "InboxUpdate" });

      endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesSent",
          pattern: $"{lang}/sentupdate",
          defaults: new { controller = "PrivateMessages", action = "SentUpdate" });

      endpointRouteBuilder.MapControllerRoute(name: "SendPM",
          pattern: $"{lang}/sendpm/{{toUserId:min(0)}}",
          defaults: new { controller = "PrivateMessages", action = "SendPM" });

      endpointRouteBuilder.MapControllerRoute(name: "SendPMReply",
          pattern: $"{lang}/sendpm/{{toUserId:min(0)}}/{{replyToMessageId:min(0)}}",
          defaults: new { controller = "PrivateMessages", action = "SendPM" });

      endpointRouteBuilder.MapControllerRoute(name: "ViewPM",
          pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
          defaults: new { controller = "PrivateMessages", action = "ViewPM" });

      endpointRouteBuilder.MapControllerRoute(name: "DeletePM",
          pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
          defaults: new { controller = "PrivateMessages", action = "DeletePM" });

      //activate newsletters
      endpointRouteBuilder.MapControllerRoute(name: "NewsletterActivation",
          pattern: $"{lang}/newsletter/subscriptionactivation/{{token:guid}}/{{active}}",
          defaults: new { controller = "Newsletter", action = "SubscriptionActivation" });

      //robots.txt (file result)
      endpointRouteBuilder.MapControllerRoute(name: "robots.txt",
          pattern: $"robots.txt",
          defaults: new { controller = "Common", action = "RobotsTextFile" });

      //sitemap
      endpointRouteBuilder.MapControllerRoute(name: "Sitemap",
          pattern: $"{lang}/sitemap",
          defaults: new { controller = "Common", action = "Sitemap" });

      //sitemap.xml (file result)
      endpointRouteBuilder.MapControllerRoute(name: "sitemap.xml",
          pattern: $"sitemap.xml",
          defaults: new { controller = "Common", action = "SitemapXml" });

      endpointRouteBuilder.MapControllerRoute(name: "sitemap-indexed.xml",
          pattern: $"sitemap-{{Id:min(0)}}.xml",
          defaults: new { controller = "Common", action = "SitemapXml" });

      //platform closed
      endpointRouteBuilder.MapControllerRoute(name: "PlatformClosed",
          pattern: $"{lang}/platformclosed",
          defaults: new { controller = "Common", action = "PlatformClosed" });

      //install
      endpointRouteBuilder.MapControllerRoute(name: "Installation",
          pattern: $"{AppInstallationDefaults.InstallPath}",
          defaults: new { controller = "Install", action = "Index" });

      //error page
      endpointRouteBuilder.MapControllerRoute(name: "Error",
          pattern: $"error",
          defaults: new { controller = "Common", action = "Error" });

      //page not found
      endpointRouteBuilder.MapControllerRoute(name: "PageNotFound",
          pattern: $"{lang}/page-not-found",
          defaults: new { controller = "Common", action = "PageNotFound" });
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets a priority of route provider
   /// </summary>
   public int Priority => 0;

   #endregion
}