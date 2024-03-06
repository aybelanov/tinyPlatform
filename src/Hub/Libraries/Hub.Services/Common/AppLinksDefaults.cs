namespace Hub.Services.Common;

/// <summary>
/// Represents default links values
/// </summary>
public static partial class AppLinksDefaults
{
   /// <summary>
   /// Represents default values of the official site URLs
   /// </summary>
   public static partial class OfficialSite
   {
      /// <summary>
      /// Gets the main site page
      /// </summary>
      public static string Main => "https://www.tinyplat.com/";

      /// <summary>
      /// Gets the copyright removal key page
      /// </summary>
      public static string CopyrightRemovalKey => "https://www.tinyplat.com/license";

      /// <summary>
      /// Gets the premium support services page
      /// </summary>
      public static string PremiumSupportServices => "https://www.tinyplat.com/request-quote";

      /// <summary>
      /// Gets the marketplace page
      /// </summary>
      public static string Marketplace => "https://www.tinyplat.com/marketplace/";

      /// <summary>
      /// Gets the states and provinces package page
      /// </summary>
      public static string StatesPackage => "https://www.tinyplat.com/all-states-provinces-package-tinyPlatform-team/";
   }

   /// <summary>
   /// Represents default values of the docs site URLs
   /// </summary>
   public static partial class Docs
   {
      /// <summary>
      /// Gets a URL of the main docs page
      /// </summary>
      public static string Main => "https://docs.tinyplat.com/";

      /// <summary>
      /// Gets a URL of the platform installation page
      /// </summary>
      public static string PlatformInstallation => "https://docs.tinyplat.com/installation/hub.html";

      /// <summary>
      /// Gets a URL of the platform installation page
      /// </summary>
      public static string ClientInstallation => "https://docs.tinyplat.com/installation/client.html";

      /// <summary>
      /// Gets a URL of the platform installation page
      /// </summary>
      public static string DispatcherInstallation => "https://docs.tinyplat.com/installation/dispatcher.html";

      /// <summary>
      /// Gets a URL of the users docs page
      /// </summary>
      public static string Devices => "https://docs.tinyplat.com/hub/devices.html";

      /// <summary>
      /// Gets a URL of the online devices docs page
      /// </summary>
      public static string OnlineDevices => "https://docs.tinyplat.com/hub/devices.html#online-devices";

      /// <summary>
      /// Gets a URL of the users docs page
      /// </summary>
      public static string Users => "https://docs.tinyplat.com/hub/users.html";

      /// <summary>
      /// Gets a URL of the user roles docs page
      /// </summary>
      public static string UserRoles => "https://docs.tinyplat.com/hub/users.html#user-roles";

      /// <summary>
      /// Gets a URL of the online users docs page
      /// </summary>
      public static string OnlineUsers => "https://docs.tinyplat.com/hub/users.html#online-users";

      /// <summary>
      /// Gets a URL of the activity log docs page
      /// </summary>
      public static string ActivityLog => "https://docs.tinyplat.com/hub/logging.html#activity-log";

      /// <summary>
      /// Gets a URL of the GDPR docs page
      /// </summary>
      public static string Gdpr => "https://docs.tinyplat.com/hub/logging.html#gdpr";

      /// <summary>
      /// Gets a URL of the email campaigns docs page
      /// </summary>
      public static string EmailCampaigns => "https://docs.tinyplat.com/hub/other-features.html#email-campaigns";

      /// <summary>
      /// Gets a URL of the topics and pages docs page
      /// </summary>
      public static string TopicsPages => "https://docs.tinyplat.com/hub/other-features.html#topics-pages.html";

      /// <summary>
      /// Gets a URL of the message templates docs page
      /// </summary>
      public static string MessageTemplates => "https://docs.tinyplat.com/hub/other-features.html#message-templates";

      /// <summary>
      /// Gets a URL of the news docs page
      /// </summary>
      public static string News => "https://docs.tinyplat.com/hub/other-features.html#news";

      /// <summary>
      /// Gets a URL of the blog docs page
      /// </summary>
      public static string Blog => "https://docs.tinyplat.com/hub/other-features.html#blog";

      /// <summary>
      /// Gets a URL of the polls docs page
      /// </summary>
      public static string Polls => "https://docs.tinyplat.com/hub/other-features.html#polls";

      /// <summary>
      /// Gets a URL of the forums docs page
      /// </summary>
      public static string Forums => "https://docs.tinyplat.com/hub/other-features.html#forums";

      /// <summary>
      /// Gets a URL of the email accounts docs page
      /// </summary>
      public static string EmailAccounts => "https://docs.tinyplat.com/hub/settings.html#email-accounts";

      /// <summary>
      /// Gets a URL of the localization docs page
      /// </summary>
      public static string Localization => "https://docs.tinyplat.com/hub/settings.html#localization";

      /// <summary>
      /// Gets a URL of the ACL docs page
      /// </summary>
      public static string Acl => "https://docs.tinyplat.com/hub/settings.html#access-control-list";

      /// <summary>
      /// Gets a URL of the external authentication docs page
      /// </summary>
      public static string ExternalAuthentication => "https://docs.tinyplat.com/hub/settings.html#external-authentication-methods";

      /// <summary>
      /// Gets a URL of the plugins docs page
      /// </summary>
      public static string Plugins => "https://docs.tinyplat.com/hub/plugins.html";

      /// <summary>
      /// Gets a URL of the log docs page
      /// </summary>
      public static string Log => "https://docs.tinyplat.com/hub/logging.html#system-log";

      /// <summary>
      /// Gets a URL of the maintenance docs page
      /// </summary>
      public static string Maintenance => "https://docs.tinyplat.com/hub/maintenance.html";

      /// <summary>
      /// Gets a URL of the message queue docs page
      /// </summary>
      public static string MessageQueue => "https://docs.tinyplat.com/hub/maintenance.html#message-queue";

      /// <summary>
      /// Gets a URL of the schedule tasks docs page
      /// </summary>
      public static string ScheduleTasks => "https://docs.tinyplat.com/hub/scheduletasks.html";

      /// <summary>
      /// Gets a URL of the reports docs page
      /// </summary>
      public static string Reports => "https://docs.tinyplat.com/hub/maintenance.html#reports";

      /// <summary>
      /// Gets a URL of the app settings docs page
      /// </summary>
      public static string AppSettings => "https://docs.tinyplat.com/hub/settings.html";
   }

   /// <summary>
   /// Represents default values of the UTM parameters
   /// </summary>
   public static partial class Utm
   {
      /// <summary>
      /// Gets parameters used on admin area
      /// </summary>
      public static string OnAdmin => "?utm_source=admin-panel&utm_medium=admin-page&utm_campaign=documentation&utm_content=doc-reference";

      /// <summary>
      /// Gets parameters used on admin area tour
      /// </summary>
      public static string OnAdminTour => "?utm_source=admin-panel&utm_medium=tour&utm_campaign=marketplace&utm_content=tooltip";

      /// <summary>
      /// Gets parameters used on admin area tour
      /// </summary>
      public static string OnAdminTourDocs => "?utm_source=admin-panel&utm_medium=tour&utm_campaign=documentation&utm_content=tooltip";

      /// <summary>
      /// Gets parameters used on admin area log
      /// </summary>
      public static string OnAdminLog => "?utm_source=admin-panel&utm_medium=menu&utm_campaign=premium_support&utm_content=log-details";

      /// <summary>
      /// Gets parameters used on admin area log list
      /// </summary>
      public static string OnAdminLogList => "?utm_source=admin-panel&utm_medium=menu&utm_campaign=premium_support&utm_content=log-list";

      /// <summary>
      /// Gets parameters used on admin area footer
      /// </summary>
      public static string OnAdminFooter => "?utm_source=admin-panel&utm_medium=footer&utm_campaign=admin-panel";

      /// <summary>
      /// Gets parameters used on admin area countries page
      /// </summary>
      public static string OnAdminCountries => "?utm_source=admin-panel&utm_medium=countries&utm_campaign=admin-panel";

      /// <summary>
      /// Gets parameters used on admin area all plugins page
      /// </summary>
      public static string OnAdminAllPlugins => "?utm_source=admin-panel&utm_medium=menu&utm_campaign=marketplace&utm_content=all-plugins";

      /// <summary>
      /// Gets parameters used on admin area plugins feed page
      /// </summary>
      public static string OnAdminPluginsFeed => "?utm_source=admin-panel&utm_medium=menu&utm_campaign=marketplace&utm_content=official-plugins";

      /// <summary>
      /// Gets parameters used on admin area 'choose a theme' page
      /// </summary>
      public static string OnAdminThemes => "?utm_source=admin-panel&utm_medium=menu&utm_campaign=marketplace&utm_content=general-common-theme";

      /// <summary>
      /// Gets parameters used on install
      /// </summary>
      public static string OnInstall => "?utm_source=installation-page&utm_medium=footer&utm_campaign=installation-page";

      /// <summary>
      /// Gets parameters used on admin area configuration steps block
      /// </summary>
      public static string OnAdminConfigurationSteps => "?utm_source=admin-panel&utm_medium=tour&utm_campaign=powered_by_tinyPlatform&utm_content=dashboard";
   }
}