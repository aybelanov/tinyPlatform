using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Configuration;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Data.Mapping;
using Hub.Data.Migrations;
using Hub.Services.Authentication;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Blogs;
using Hub.Services.Clients.Devices;
using Hub.Services.Clients.Monitors;
using Hub.Services.Clients.Records;
using Hub.Services.Clients.Reports;
using Hub.Services.Clients.Sensors;
using Hub.Services.Clients.Widgets;
using Hub.Services.Cms;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Devices;
using Hub.Services.Directory;
using Hub.Services.Events;
using Hub.Services.ExportImport;
using Hub.Services.Forums;
using Hub.Services.Gdpr;
using Hub.Services.Helpers;
using Hub.Services.Hosted;
using Hub.Services.Html;
using Hub.Services.Installation;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Media.RoxyFileman;
using Hub.Services.Messages;
using Hub.Services.News;
using Hub.Services.Plugins;
using Hub.Services.Plugins.Marketplace;
using Hub.Services.Polls;
using Hub.Services.ScheduleTasks;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Services.Themes;
using Hub.Services.Topics;
using Hub.Services.Users;
using Hub.Web.Framework.Menu;
using Hub.Web.Framework.Mvc.Routing;
using Hub.Web.Framework.Themes;
using Hub.Web.Framework.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Hub.Web.Framework.Infrastructure;

/// <summary>
/// Represents the registering services on application startup
/// </summary>
public class AppliedServicesStartup : IAppStartup
{
   /// <summary>
   /// Add and configure any of the middleware
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   /// <param name="configuration">Configuration of the application</param>
   public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
   {
      //file provider
      services.AddScoped<IAppFileProvider, AppFileProvider>();

      //web helper
      services.AddScoped<IWebHelper, WebHelper>();

      //user agent helper
      services.AddScoped<IUserAgentHelper, UserAgentHelper>();

      //data layer
      //services.AddTransient<IDataProviderManager, DataProviderManager>();
      //services.AddTransient(sp => sp.GetRequiredService<IDataProviderManager>().DataProvider);
      services.AddTransient<IMigrationManager,  MigrationManager>();
      services.AddTransient<IMappingEntityAccessor, AppDbContext>();

      //repositories
      services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

      //plugins
      services.AddScoped<IPluginService, PluginService>();
      services.AddScoped<OfficialFeedManager>();

      //static cache manager
      var appSettings = Singleton<AppSettings>.Instance;
      if (appSettings.Get<DistributedCacheConfig>().Enabled)
      {
         services.AddScoped<ILocker, DistributedCacheManager>();
         services.AddScoped<IStaticCacheManager, DistributedCacheManager>();
      }
      else
      {
         services.AddSingleton<ILocker, MemoryCacheManager>();
         services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
      }

      //work context
      services.AddScoped<IWorkContext, WebWorkContext>();

      // hosted services
      if (DataSettingsManager.IsDatabaseInstalled())
      {
         services.AddHostedService<ExportDataHostedService>();
         services.AddTransient<ExportDataFileService>();
      }

      //services
      services.AddScoped<ITopicTemplateService, TopicTemplateService>();
      services.AddScoped<IAddressAttributeFormatter, AddressAttributeFormatter>();
      services.AddScoped<IAddressAttributeParser, AddressAttributeParser>();
      services.AddScoped<IAddressAttributeService, AddressAttributeService>();
      services.AddScoped<IAddressService, AddressService>();
      services.AddScoped<ISearchTermService, SearchTermService>();
      services.AddScoped<IGenericAttributeService, GenericAttributeService>();
      services.AddScoped<IMaintenanceService, MaintenanceService>();
      services.AddScoped<IUserAttributeFormatter, UserAttributeFormatter>();
      services.AddScoped<IUserAttributeParser, UserAttributeParser>();
      services.AddScoped<IUserAttributeService, UserAttributeService>();
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<IUserRegistrationService, UserRegistrationService>();
      services.AddScoped<IUserReportService, UserReportService>();
      services.AddScoped<IPermissionService, PermissionService>();
      services.AddScoped<IAclService, AclService>();
      services.AddScoped<IGeoLookupService, GeoLookupService>();
      services.AddScoped<ICountryService, CountryService>();
      services.AddScoped<ICurrencyService, CurrencyService>();
      services.AddScoped<IMeasureService, MeasureService>();
      services.AddScoped<IStateProvinceService, StateProvinceService>();
      services.AddScoped<ILocalizationService, LocalizationService>();
      services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
      services.AddScoped<ILanguageService, LanguageService>();
      services.AddScoped<IDownloadService, DownloadService>();
      services.AddScoped<IMessageTemplateService, MessageTemplateService>();
      services.AddScoped<IQueuedEmailService, QueuedEmailService>();
      services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
      services.AddScoped<INotificationService, NotificationService>();
      services.AddScoped<ICampaignService, CampaignService>();
      services.AddScoped<IEmailAccountService, EmailAccountService>();
      services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
      services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
      services.AddScoped<ITokenizer, Tokenizer>();
      services.AddScoped<ISmtpBuilder, SmtpBuilder>();
      services.AddScoped<IEmailSender, EmailSender>();
      services.AddScoped<IEncryptionService, EncryptionService>();
      services.AddScoped<IAuthenticationService, MixedAuthenticationService>();
      services.AddScoped<IUrlRecordService, UrlRecordService>();
      services.AddScoped<ILogger, DefaultLogger>();
      services.AddScoped<IUserActivityService, UserActivityService>();
      services.AddTransient<IForumService, ForumService>();
      services.AddScoped<IGdprService, GdprService>();
      services.AddScoped<IPollService, PollService>();
      services.AddScoped<IBlogService, BlogService>();
      services.AddScoped<ITopicService, TopicService>();
      services.AddScoped<INewsService, NewsService>();
      services.AddScoped<IDateTimeHelper, DateTimeHelper>();
      services.AddScoped<ISitemapGenerator, SitemapGenerator>();
      services.AddScoped<IAppHtmlHelper, AppHtmlHelper>();
      services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
      services.AddScoped<IExportManager, ExportManager>();
      services.AddScoped<IImportManager, ImportManager>();
      services.AddScoped<IPdfService, PdfService>();
      services.AddScoped<IUploadService, UploadService>();
      services.AddScoped<IThemeProvider, ThemeProvider>();
      services.AddScoped<IThemeContext, ThemeContext>();
      services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
      services.AddSingleton<IRoutePublisher, RoutePublisher>();
      services.AddSingleton<IEventPublisher, EventPublisher>();
      services.AddScoped<ISettingService, SettingService>();
      services.AddScoped<IBBCodeHelper, BBCodeHelper>();
      services.AddScoped<IHtmlFormatter, HtmlFormatter>();

      services.AddScoped<IDeviceActivityService, DeviceActivityService>();
      services.AddScoped<IDeviceRegistrationService, DeviceRegistrationService>();
      services.AddScoped<IHubDeviceService, HubDeviceService>();
      services.AddScoped<ILocalizer, Localizer>(); 
      services.AddScoped<IHubSensorService, HubSensorService>();
      services.AddScoped<IHubSensorRecordService, HubSensorRecordService>();
      services.AddScoped<IVideoStreamService, VideoStreamService>();

      // client services
      services.AddScoped<IMonitorService, MonitorService>();
      services.AddScoped<IDeviceService, DeviceService>();
      services.AddScoped<ISensorService, SensorService>();
      services.AddScoped<IWidgetService, WidgetService>();
      services.AddScoped<ISensorRecordService, SensorRecordService>();
      services.AddScoped<IDownloadTaskService, DownloadTaskService>();
      services.AddScoped<IPresentationService, PresentationService>();
      services.AddScoped<IDispatcherService, DispatcherService>();

      //plugin managers
      services.AddScoped(typeof(IPluginManager<>), typeof(PluginManager<>));
      services.AddScoped<IAuthenticationPluginManager, AuthenticationPluginManager>();
      services.AddScoped<IMultiFactorAuthenticationPluginManager, MultiFactorAuthenticationPluginManager>();
      services.AddScoped<IWidgetPluginManager, WidgetPluginManager>();
      services.AddScoped<IExchangeRatePluginManager, ExchangeRatePluginManager>();

      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

      //register all settings
      var typeFinder = Singleton<ITypeFinder>.Instance;
      var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
      foreach (var setting in settings)
         services.AddScoped(setting, serviceProvider => serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting).Result);

         //picture service
      if (appSettings.Get<AzureBlobConfig>().Enabled)
         services.AddScoped<IPictureService, AzurePictureService>();
      else
         services.AddScoped<IPictureService, PictureService>();

      //roxy file manager
      services.AddScoped<IRoxyFilemanService, RoxyFilemanService>();
      services.AddScoped<IRoxyFilemanFileProvider, RoxyFilemanFileProvider>();

      //installation service
      if (!DataSettingsManager.IsDatabaseInstalled())
         services.AddScoped<IInstallationService, InstallationService>();

      //slug route transformer
      if (DataSettingsManager.IsDatabaseInstalled())
         services.AddScoped<SlugRouteTransformer>();

      //schedule tasks
      services.AddSingleton<ITaskScheduler, TaskScheduler>();
      services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

      //event consumers
      var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
      foreach (var consumer in consumers)
      {
         var interfaces = consumer.FindInterfaces((type, criteria) =>
         {
            var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
            return isMatch;
         }, typeof(IConsumer<>));

         foreach (var findInterface in interfaces)
            services.AddScoped(findInterface, consumer);
      }

      //XML sitemap
      services.AddScoped<IXmlSiteMap, XmlSiteMap>();
   }

   /// <summary>
   /// Configure the using of added middleware
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public void Configure(IApplicationBuilder application)
   {
   }

   /// <summary>
   /// Gets order of this startup configuration implementation
   /// </summary>
   public int Order => 2000;
}