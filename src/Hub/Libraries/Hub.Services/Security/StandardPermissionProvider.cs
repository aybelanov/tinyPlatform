using Hub.Core.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Shared.Clients.Configuration;
using System.Collections.Generic;

namespace Hub.Services.Security;

/// <summary>
/// Standard permission provider
/// </summary>
public partial class StandardPermissionProvider : IPermissionProvider
{
#pragma warning disable CS1591

   //admin area permissions
   public static readonly PermissionRecord AccessAdminPanel = new() { Name = "Access admin area", Category = "Standard", SystemName = nameof(AccessAdminPanel) };
   public static readonly PermissionRecord AllowUserImpersonation = new() { Name = "Admin area. Allow User Impersonation", Category = "Users", SystemName = nameof(AllowUserImpersonation) };
   public static readonly PermissionRecord ManageUsers = new() { Name = "Admin area. Manage Users", Category = "Users", SystemName = nameof(ManageUsers) };
   public static readonly PermissionRecord AllowAdminsDevices = new() { Name = "Admin area. Device administration", Category = "Device management", SystemName = nameof(AllowAdminsDevices) };
   public static readonly PermissionRecord ManageAffiliates = new() { Name = "Admin area. Manage Affiliates", Category = "Promo", SystemName = nameof(ManageAffiliates) };
   public static readonly PermissionRecord ManageCampaigns = new() { Name = "Admin area. Manage Campaigns", Category = "Promo", SystemName = nameof(ManageCampaigns) };
   public static readonly PermissionRecord ManageNewsletterSubscribers = new() { Name = "Admin area. Manage Newsletter Subscribers", Category = "Promo", SystemName = nameof(ManageNewsletterSubscribers) };
   public static readonly PermissionRecord ManagePolls = new() { Name = "Admin area. Manage Polls", Category = "Content Management", SystemName = nameof(ManagePolls) };
   public static readonly PermissionRecord ManageNews = new() { Name = "Admin area. Manage News", Category = "Content Management", SystemName = nameof(ManageNews) };
   public static readonly PermissionRecord ManageBlog = new() { Name = "Admin area. Manage Blog", Category = "Content Management", SystemName = nameof(ManageBlog) };
   public static readonly PermissionRecord ManageWidgets = new() { Name = "Admin area. Manage Widgets", Category = "Content Management", SystemName = nameof(ManageWidgets) };
   public static readonly PermissionRecord ManageTopics = new() { Name = "Admin area. Manage Topics", Category = "Content Management", SystemName = nameof(ManageTopics) };
   public static readonly PermissionRecord ManageForums = new() { Name = "Admin area. Manage Forums", Category = "Content Management", SystemName = nameof(ManageForums) };
   public static readonly PermissionRecord ManageMessageTemplates = new() { Name = "Admin area. Manage Message Templates", Category = "Content Management", SystemName = nameof(ManageMessageTemplates) };
   public static readonly PermissionRecord ManageCountries = new() { Name = "Admin area. Manage Countries", Category = "Configuration", SystemName = nameof(ManageCountries) };
   public static readonly PermissionRecord ManageLanguages = new() { Name = "Admin area. Manage Languages", Category = "Configuration", SystemName = nameof(ManageLanguages) };
   public static readonly PermissionRecord ManageSettings = new() { Name = "Admin area. Manage Settings", Category = "Configuration", SystemName = nameof(ManageSettings) };
   public static readonly PermissionRecord ManageExternalAuthenticationMethods = new() { Name = "Admin area. Manage External Authentication Methods", Category = "Configuration", SystemName = nameof(ManageExternalAuthenticationMethods) };
   public static readonly PermissionRecord ManageMultifactorAuthenticationMethods = new() { Name = "Admin area. Manage Multi-factor Authentication Methods", Category = "Configuration", SystemName = nameof(ManageMultifactorAuthenticationMethods) };
   public static readonly PermissionRecord ManageCurrencies = new() { Name = "Admin area. Manage Currencies", Category = "Configuration", SystemName = nameof(ManageCurrencies) };
   public static readonly PermissionRecord ManageMeasureSettings = new() { Name = "Admin area. Manage Measure Settings", Category = "Configuration", SystemName = nameof(ManageMeasureSettings) };
   public static readonly PermissionRecord ManageActivityLog = new() { Name = "Admin area. Manage Activity Log", Category = "Configuration", SystemName = nameof(ManageActivityLog) };
   public static readonly PermissionRecord ManageAcl = new() { Name = "Admin area. Manage ACL", Category = "Configuration", SystemName = nameof(ManageAcl) };
   public static readonly PermissionRecord ManageEmailAccounts = new() { Name = "Admin area. Manage Email Accounts", Category = "Configuration", SystemName = nameof(ManageEmailAccounts) };
   public static readonly PermissionRecord ManagePlugins = new() { Name = "Admin area. Manage Plugins", Category = "Configuration", SystemName = nameof(ManagePlugins) };
   public static readonly PermissionRecord ManageSystemLog = new() { Name = "Admin area. Manage System Log", Category = "Configuration", SystemName = nameof(ManageSystemLog) };
   public static readonly PermissionRecord ManageMessageQueue = new() { Name = "Admin area. Manage Message Queue", Category = "Configuration", SystemName = nameof(ManageMessageQueue) };
   public static readonly PermissionRecord ManageMaintenance = new() { Name = "Admin area. Manage Maintenance", Category = "Configuration", SystemName = nameof(ManageMaintenance) };
   public static readonly PermissionRecord HtmlEditorManagePictures = new() { Name = "Admin area. HTML Editor. Manage pictures", Category = "Configuration", SystemName = nameof(HtmlEditorManagePictures) };
   public static readonly PermissionRecord ManageScheduleTasks = new() { Name = "Admin area. Manage Schedule Tasks", Category = "Configuration", SystemName = nameof(ManageScheduleTasks) };
   public static readonly PermissionRecord ManageAppSettings = new() { Name = "Admin area. Manage App Settings", Category = "Configuration", SystemName = nameof(ManageAppSettings) };

   // hub methods permission
   public static readonly PermissionRecord AllowManageDevices = new() { Name = "Hub. Allow to manage devices", Category = "Hub", SystemName = nameof(AllowManageDevices) };
   public static readonly PermissionRecord AllowManageMonitors = new() { Name = "Hub. Allow to manage monitors", Category = "Hub", SystemName = nameof(AllowManageMonitors) };
   public static readonly PermissionRecord AllowManageWidgets = new() { Name = "Hub. Allow to manage HMI widgets", Category = "Hub", SystemName = nameof(AllowManageWidgets) };
   public static readonly PermissionRecord AllowManageReports = new() { Name = "Hub. Allow to manage reports", Category = "Hub", SystemName = nameof(AllowManageReports) };
   public static readonly PermissionRecord AllowShareDevices = new() { Name = "Hub. Allow share device to users", Category = "Hub", SystemName = nameof(AllowShareDevices) };
   public static readonly PermissionRecord AllowShareMonitors = new() { Name = "Hub. Allow share monitors to users", Category = "Hub", SystemName = nameof(AllowShareMonitors) };
   public static readonly PermissionRecord AllowShareWidgets = new() { Name = "Hub. Allow share widgets to users", Category = "Hub", SystemName = nameof(AllowShareWidgets) };
   public static readonly PermissionRecord AllowShareReports = new() { Name = "Hub. Allow share reports to users", Category = "Hub", SystemName = nameof(AllowShareReports) };
   public static readonly PermissionRecord AllowGetData = new() { Name = "Hub. Allow users to view reports", Category = "Hub", SystemName = nameof(AllowGetData) };
   public static readonly PermissionRecord AllowGetReports = new() { Name = "Hub. Allow users to get reports", Category = "Hub", SystemName = nameof(AllowGetReports) };

   //public platform permissions
   public static readonly PermissionRecord EnableWishlist = new() { Name = "Public platform. Enable wishlist", Category = "PublicPlatform", SystemName = nameof(EnableWishlist) };
   public static readonly PermissionRecord PublicPagesAllowNavigation = new() { Name = "Public platform. Allow navigation", Category = "PublicPlatform", SystemName = nameof(PublicPagesAllowNavigation) };
   public static readonly PermissionRecord AccessClosedService = new() { Name = "Public platform. Access a closed web service", Category = "PublicPlatform", SystemName = nameof(AccessClosedService) };
   public static readonly PermissionRecord AccessProfiling = new() { Name = "Public platform. Access MiniProfiler results", Category = "PublicPlatform", SystemName = nameof(AccessProfiling) };
   public static readonly PermissionRecord AccessForum = new() { Name = "Public platform. Access Forum", Category = "PublicPlatform", SystemName = nameof(AccessForum) };
   public static readonly PermissionRecord AccessBlog = new() { Name = "Public platform. Access Blog", Category = "PublicPlatform", SystemName = nameof(AccessBlog) };
   public static readonly PermissionRecord AccessDocumentation = new() { Name = "Public platform. Access Documentation", Category = "PublicPlatform", SystemName = nameof(AccessDocumentation) };
   public static readonly PermissionRecord AccessNews = new() { Name = "Public platform. Access News", Category = "PublicPlatform", SystemName = nameof(AccessNews) };
   public static readonly PermissionRecord AccessPolls = new() { Name = "Public platform. Access Polls", Category = "PublicPlatform", SystemName = nameof(AccessPolls) };

   // device permission
   public static readonly PermissionRecord GetDeviceConfig = new() { Name = "Device. Get configuration", Category = "Device", SystemName = nameof(GetDeviceConfig) };
   public static readonly PermissionRecord SaveSensorData = new() { Name = "Device. Save sensro data", Category = "Device", SystemName = nameof(SaveSensorData) };
   public static readonly PermissionRecord SaveVideo = new() { Name = "Device. Save video files", Category = "Device", SystemName = nameof(SaveVideo) };
   public static readonly PermissionRecord SetupP2PChannel = new() { Name = "Device. Setup point-to-point channel", Category = "Device", SystemName = nameof(SetupP2PChannel) };

   //on-client permissions (this section uses for clietn UI access not server resources access)
   public static readonly PermissionRecord AccessToClient = new() { Name = "Client appliaction. Allow to access to client app", Category = "Client", SystemName = nameof(AccessToClient) };
   public static readonly PermissionRecord ManageDevices = new() { Name = "Client appliaction. Allow to manage devices", Category = "Client", SystemName = nameof(ManageDevices) };
   public static readonly PermissionRecord ManageMonitors = new() { Name = "Client appliaction. Allow to manage monitors", Category = "Client", SystemName = nameof(ManageMonitors) };
   public static readonly PermissionRecord ManageHMIWidgets = new() { Name = "Client appliaction. Allow to manage HMI widgets", Category = "Client", SystemName = nameof(ManageHMIWidgets) };
   public static readonly PermissionRecord ManageReports = new() { Name = "Client appliaction. Allow to manage reports", Category = "Client", SystemName = nameof(ManageReports) };
   public static readonly PermissionRecord ShareDevices = new() { Name = "Client appliaction. Allow share device to users", Category = "Client", SystemName = nameof(ShareDevices) };
   public static readonly PermissionRecord ShareMonitors = new() { Name = "Client appliaction. Allow share monitors to users", Category = "Client", SystemName = nameof(ShareMonitors) };
   public static readonly PermissionRecord ShareWidgets = new() { Name = "Client appliaction. Allow share widgets to users", Category = "Client", SystemName = nameof(ShareWidgets) };
   public static readonly PermissionRecord ShareReports = new() { Name = "Client appliaction. Allow share reports to users", Category = "Client", SystemName = nameof(ShareReports) };
   public static readonly PermissionRecord ViewData = new() { Name = "Client appliaction. Allow users to view reports", Category = "Client", SystemName = nameof(ViewData) };
   public static readonly PermissionRecord ViewReports = new() { Name = "Client appliaction. Allow users to get reports", Category = "Client", SystemName = nameof(ViewReports) };


   private static readonly IEnumerable<PermissionRecord> _permissions = new[]
   {
      AccessAdminPanel,
      AllowUserImpersonation,
      ManageUsers,
      AllowAdminsDevices,
      ManageAffiliates,
      ManageCampaigns,
      ManageNewsletterSubscribers,
      ManagePolls,
      ManageNews,
      ManageBlog,
      ManageWidgets,
      ManageTopics,
      ManageForums,
      ManageMessageTemplates,
      ManageCountries,
      ManageLanguages,
      ManageSettings,
      ManageExternalAuthenticationMethods,
      ManageMultifactorAuthenticationMethods,
      ManageMeasureSettings,
      ManageCurrencies,
      ManageActivityLog,
      ManageAcl,
      ManageEmailAccounts,
      ManagePlugins,
      ManageSystemLog,
      ManageMessageQueue,
      ManageMaintenance,
      HtmlEditorManagePictures,
      ManageScheduleTasks,
      ManageAppSettings,
      AllowManageDevices,
      AllowManageMonitors,
      AllowManageWidgets,
      AllowManageReports,
      AllowShareDevices,
      AllowShareMonitors,
      AllowShareWidgets,
      AllowShareReports,
      AllowGetData,
      AllowGetReports,

      EnableWishlist,
      PublicPagesAllowNavigation,
      AccessClosedService,
      AccessProfiling,
      AccessToClient,
      AccessBlog,
      AccessForum,
      AccessNews,
      AccessDocumentation,
      AccessPolls,

      GetDeviceConfig,
      SaveSensorData,
      SaveVideo,
      SetupP2PChannel,

      AccessToClient,
      ManageDevices,
      ManageMonitors,
      ManageHMIWidgets,
      ManageReports,
      ShareDevices,
      ShareMonitors,
      ShareWidgets,
      ShareReports,
      ViewData,
      ViewReports
   };

   public static void PrepareAuthorizationPolicies(AuthorizationOptions options)
   {
      foreach (var permission in _permissions)
      {
         options.AddPolicy(permission.SystemName, policy =>
         {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", permission.SystemName);
         });
      }
   }

   /// <summary>
   /// Get permissions
   /// </summary>
   /// <returns>Permissions</returns>
   public virtual IEnumerable<PermissionRecord> GetPermissions() => _permissions;


   // TODO add telemetry role permission
   /// <summary>
   /// Get default permissions
   /// </summary>
   /// <returns>Permissions</returns>
   public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
   {
      return new HashSet<(string, PermissionRecord[])>
      {
         (UserDefaults.AdministratorsRoleName,
         new[]
         {
            AccessAdminPanel,
            AllowUserImpersonation,
            ManageUsers,
            AllowAdminsDevices,
            ManageAffiliates,
            ManageCampaigns,
            ManageNewsletterSubscribers,
            ManagePolls,
            ManageNews,
            ManageBlog,
            ManageWidgets,
            ManageTopics,
            ManageForums,
            ManageMessageTemplates,
            ManageCountries,
            ManageLanguages,
            ManageSettings,
            ManageExternalAuthenticationMethods,
            ManageMultifactorAuthenticationMethods,
            ManageMeasureSettings,
            ManageCurrencies,
            ManageActivityLog,
            ManageAcl,
            ManageEmailAccounts,
            ManagePlugins,
            ManageSystemLog,
            ManageMessageQueue,
            ManageMaintenance,
            HtmlEditorManagePictures,
            ManageScheduleTasks,
            ManageAppSettings,

            AllowManageDevices,
            AllowManageMonitors,
            AllowManageWidgets,
            AllowManageReports,
            AllowShareDevices,
            AllowShareMonitors,
            AllowShareWidgets,
            AllowShareReports,
            AllowGetData,
            AllowGetReports,

            EnableWishlist,
            PublicPagesAllowNavigation,
            AccessClosedService,
            AccessProfiling,
            AccessToClient,
            AccessBlog,
            AccessForum,
            AccessNews,
            AccessDocumentation,
            AccessPolls,

            AccessToClient,
            ManageDevices,
            ManageMonitors,
            ManageHMIWidgets,
            ManageReports,
            ShareDevices,
            ShareMonitors,
            ShareWidgets,
            ShareReports,
            ViewData,
            ViewReports
         }),
         (UserDefaults.ForumModeratorsRoleName,
         new[]
         {
            EnableWishlist,
            PublicPagesAllowNavigation
         }),
         (UserDefaults.GuestsRoleName,
         new[]
         {
            EnableWishlist,
            PublicPagesAllowNavigation
         }),
         (UserDefaults.RegisteredRoleName,
         new[]
         {
            EnableWishlist,
            PublicPagesAllowNavigation,
            AccessBlog,
            AccessForum,
            AccessNews,
            AccessDocumentation,
            AccessPolls,
         }),
         (UserDefaults.OwnersRoleName,
         new[]
         {
            AllowManageDevices,
            AllowManageMonitors,
            AllowManageWidgets,
            AllowManageReports,
            AllowShareDevices,
            AllowShareMonitors,
            AllowShareWidgets,
            AllowShareReports,
            AllowGetData,
            AllowGetReports,

            AccessToClient,
            ManageDevices,
            ManageMonitors,
            ManageHMIWidgets,
            ManageReports,
            ShareDevices,
            ShareMonitors,
            ShareWidgets,
            ShareReports,
            ViewData,
            ViewReports
         }),
         (UserDefaults.OperatorsRoleName,
         new[]
         {
            AllowGetData,
            AllowGetReports,

            AccessToClient,
            ViewData,
            ViewReports
         }),
         (UserDefaults.DevicesRoleName, new[]
         {
            GetDeviceConfig,
            SaveSensorData,
            SaveVideo,
            SetupP2PChannel
         })
      };
   }



#pragma warning restore CS1591
}