using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.ComponentModel;
using Hub.Core.Configuration;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Data.Configuration;
using Hub.Data.Migrations;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Cms;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Directory;
using Hub.Services.Events;
using Hub.Services.ExportImport;
using Hub.Services.Helpers;
using Hub.Services.Html;
using Hub.Services.Installation;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Messages;
using Hub.Services.Plugins;
using Hub.Services.ScheduleTasks;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Services.Tests.ScheduleTasks;
using Hub.Services.Themes;
using Hub.Services.Users;
using Hub.Web.Factories;
using Hub.Web.Framework;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Infrastructure.Extensions;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Themes;
using Hub.Web.Framework.UI;
using Hub.Web.Infrastructure.Installation;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Moq;
using Shared.Common;
using SkiaSharp;
using IAuthenticationService = Hub.Services.Authentication.IAuthenticationService;
using Task = System.Threading.Tasks.Task;

namespace Hub.Tests;

public partial class BaseAppTest
{
   private static readonly ServiceProvider _serviceProvider;
   private static readonly ResourceManager _resourceManager;

   protected BaseAppTest()
   {
      SetDataProviderType(DataProviderType.Unknown);
   }

   private static void Init()
   {
      //var dataProvider = _serviceProvider.GetService<IDataProviderManager>().DataProvider;

      //dataProvider.CreateDatabase(null);
      //dataProvider.InitializeDatabase();
      Singleton<DataConfig>.Instance = new()
      {
         ConnectionString = "Data Source=applicationTest.sqlite;Mode=Memory;Cache=Shared",
         DataProvider = DataProviderType.SQLite
      };
      var dataProvider = _serviceProvider.GetService<AppDbContext>();
      dataProvider.Database.OpenConnection();
      dataProvider.Database.EnsureDeleted();
      dataProvider.Database.EnsureCreated();

      //var languagePackInfo = (DownloadUrl: string.Empty, Progress: 0);

      //_serviceProvider.GetService<IInstallationService>()
      //    .InstallRequiredDataAsync(AppTestsDefaults.AdminEmail, AppTestsDefaults.AdminPassword, languagePackInfo, null, null).Wait();
      _serviceProvider.GetService<IInstallationService>()
          .InstallRequiredDataAsync(AppTestsDefaults.AdminEmail, AppTestsDefaults.AdminPassword, null, null).Wait();
      _serviceProvider.GetService<IInstallationService>().InstallSampleDataAsync(AppTestsDefaults.AdminEmail).Wait();

      var provider = (IPermissionProvider)Activator.CreateInstance(typeof(StandardPermissionProvider));
      EngineContext.Current.Resolve<IPermissionService>().InstallPermissionsAsync(provider).Wait();
   }

   protected static T PropertiesShouldEqual<T, Tm>(T entity, Tm model, params string[] filter) where T : BaseEntity
   where Tm : BaseAppModel
   {
      var objectProperties = typeof(T).GetProperties();
      var modelProperties = typeof(Tm).GetProperties();

      foreach (var objectProperty in objectProperties)
      {
         var name = objectProperty.Name;

         if (filter.Contains(name))
            continue;

         var modelProperty = Array.Find(modelProperties, p => p.Name == name);

         if (modelProperty == null)
            continue;

         var objectPropertyValue = objectProperty.GetValue(entity);
         var modelPropertyValue = modelProperty.GetValue(model);

         objectPropertyValue.Should().Be(modelPropertyValue, $"The property \"{typeof(T).Name}.{objectProperty.Name}\" of these objects is not equal");
      }

      return entity;
   }

   static BaseAppTest()
   {
      _resourceManager = Connections.ResourceManager;
      SetDataProviderType(DataProviderType.Unknown);

      TypeDescriptor.AddAttributes(typeof(List<int>),
          new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
      TypeDescriptor.AddAttributes(typeof(List<string>),
          new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

      var services = new ServiceCollection();

      services.AddHttpClient();

      var memoryCache = new MemoryCache(new MemoryCacheOptions());
      var typeFinder = new AppDomainTypeFinder();
      Singleton<ITypeFinder>.Instance = typeFinder;

      //var mAssemblies = typeFinder.FindClassesOfType<AutoReversingMigration>()
      //    .Select(t => t.Assembly)
      //    .Distinct()
      //    .ToArray();

      //create app settings
      var configurations = typeFinder
          .FindClassesOfType<IConfig>()
          .Select(configType => (IConfig)Activator.CreateInstance(configType))
          .ToList();

      var appSettings = new AppSettings(configurations);
      appSettings.Update(new List<IConfig> { Singleton<DataConfig>.Instance });
      Singleton<AppSettings>.Instance = appSettings;
      services.AddSingleton(appSettings);

      var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
      services.AddSingleton(hostApplicationLifetime.Object);

      var rootPath =
          new DirectoryInfo(
                  $"{Directory.GetCurrentDirectory().Split("bin")[0]}{Path.Combine(@"\..\..\Hub\Presentation\Hub.Web".Split('\\', '/').ToArray())}")
              .FullName;

      //Presentation\Hub.Web\wwwroot
      var webHostEnvironment = new Mock<IWebHostEnvironment>();
      webHostEnvironment.Setup(p => p.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));
      webHostEnvironment.Setup(p => p.ContentRootPath).Returns(rootPath);
      webHostEnvironment.Setup(p => p.EnvironmentName).Returns("test");
      webHostEnvironment.Setup(p => p.ApplicationName).Returns("application");
      services.AddSingleton(webHostEnvironment.Object);

      services.AddWebEncoders();

#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
      var httpContext = new DefaultHttpContext
      {
         Request = { Headers = { { HeaderNames.Host, AppTestsDefaults.HostIpAddress } } }
      };
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer

      var httpContextAccessor = new Mock<IHttpContextAccessor>();
      httpContextAccessor.Setup(p => p.HttpContext).Returns(httpContext);

      services.AddSingleton(httpContextAccessor.Object);

      var actionContextAccessor = new Mock<IActionContextAccessor>();
      actionContextAccessor.Setup(x => x.ActionContext)
          .Returns(new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor()));

      services.AddSingleton(actionContextAccessor.Object);

      var urlHelperFactory = new Mock<IUrlHelperFactory>();
      var urlHelper = new AppTestUrlHelper(actionContextAccessor.Object.ActionContext);

      urlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
          .Returns(urlHelper);

      services.AddTransient(_ => actionContextAccessor.Object);

      services.AddSingleton(urlHelperFactory.Object);

      var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
      var dataDictionary = new TempDataDictionary(httpContextAccessor.Object.HttpContext,
          new Mock<ITempDataProvider>().Object);
      tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(dataDictionary);
      services.AddSingleton(tempDataDictionaryFactory.Object);

      services.AddSingleton<ITypeFinder>(typeFinder);
      Singleton<ITypeFinder>.Instance = typeFinder;

      //file provider
      services.AddTransient<IAppFileProvider, AppFileProvider>();
      CommonHelper.DefaultFileProvider = new AppFileProvider(webHostEnvironment.Object);

      //web helper
      services.AddTransient<IWebHelper, WebHelper>();

      //user agent helper
      services.AddTransient<IUserAgentHelper, UserAgentHelper>();

      //data layer
      //services.AddTransient<IDataProviderManager, TestDataProviderManager>();
      //services.AddTransient(serviceProvider =>
      //    serviceProvider.GetRequiredService<IDataProviderManager>().DataProvider);
      //services.AddTransient<IMappingEntityAccessor>(x => x.GetRequiredService<IAppDataProvider>());

      //services.AddAppDbContext<AppDbContext>();
      LinqToDBForEFTools.Initialize();
      services.AddDbContext<AppDbContext>(options =>
      {
         //options.UseSqlite("Data Source = appTestDb.sqlite;").UseLinqToDb();
         options.UseSqlite("Data Source=applicationTest.sqlite;Mode=Memory;Cache=Shared").UseLinqToDB();

      }, ServiceLifetime.Scoped);

      //repositories
      services.AddTransient(typeof(IRepository<>), typeof(EntityRepository<>));

      //plugins
      services.AddTransient<IPluginService, PluginService>();

      services.AddSingleton<IMemoryCache>(memoryCache);
      services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
      services.AddSingleton<ILocker, MemoryCacheManager>();

      //services
      services.AddTransient<IAddressAttributeFormatter, AddressAttributeFormatter>();
      services.AddTransient<IAddressAttributeParser, AddressAttributeParser>();
      services.AddTransient<IAddressAttributeService, AddressAttributeService>();
      services.AddTransient<IAddressService, AddressService>();
      services.AddTransient<ISearchTermService, SearchTermService>();
      services.AddTransient<IGenericAttributeService, GenericAttributeService>();
      services.AddTransient<IMaintenanceService, MaintenanceService>();
      services.AddTransient<IUserAttributeFormatter, UserAttributeFormatter>();
      services.AddTransient<IUserAttributeParser, UserAttributeParser>();
      services.AddTransient<IUserAttributeService, UserAttributeService>();
      services.AddTransient<IUserService, UserService>();
      services.AddTransient<IUserRegistrationService, UserRegistrationService>();
      services.AddTransient<IUserReportService, UserReportService>();
      services.AddTransient<IPermissionService, PermissionService>();
      services.AddTransient<IAclService, AclService>();
      services.AddTransient<IGeoLookupService, GeoLookupService>();
      services.AddTransient<ICountryService, CountryService>();
      services.AddTransient<ICurrencyService, CurrencyService>();
      services.AddTransient<IMeasureService, MeasureService>();
      services.AddTransient<IStateProvinceService, StateProvinceService>();
      services.AddTransient<ILocalizationService, LocalizationService>();
      services.AddTransient<ILocalizedEntityService, LocalizedEntityService>();
      services.AddTransient<ILanguageService, LanguageService>();
      services.AddTransient<IInstallationLocalizationService, InstallationLocalizationService>();
      services.AddTransient<IDownloadService, DownloadService>();
      services.AddTransient<IMessageTemplateService, MessageTemplateService>();
      services.AddTransient<IQueuedEmailService, QueuedEmailService>();
      services.AddTransient<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
      services.AddTransient<INotificationService, NotificationService>();
      services.AddTransient<ICampaignService, CampaignService>();
      services.AddTransient<IEmailAccountService, EmailAccountService>();
      services.AddTransient<IWorkflowMessageService, WorkflowMessageService>();
      services.AddTransient<IMessageTokenProvider, MessageTokenProvider>();
      services.AddTransient<ITokenizer, Tokenizer>();
      services.AddTransient<ISmtpBuilder, TestSmtpBuilder>();
      services.AddTransient<IEmailSender, EmailSender>();
      services.AddTransient<IEncryptionService, EncryptionService>();
      services.AddTransient<IAuthenticationService, TestAuthenticationService>();
      services.AddTransient<IUrlRecordService, UrlRecordService>();
      services.AddTransient<ILogger, DefaultLogger>();
      services.AddTransient<IUserActivityService, UserActivityService>();
      services.AddTransient<IDateTimeHelper, DateTimeHelper>();
      services.AddTransient<ISitemapGenerator, SitemapGenerator>();
      services.AddTransient<IScheduleTaskService, ScheduleTaskService>();
      services.AddTransient<IExportManager, ExportManager>();
      services.AddTransient<IImportManager, ImportManager>();
      services.AddTransient<IPdfService, PdfService>();
      services.AddTransient<IUploadService, UploadService>();
      services.AddTransient<IThemeProvider, ThemeProvider>();
      services.AddTransient<IExternalAuthenticationService, ExternalAuthenticationService>();
      services.AddScoped<IBBCodeHelper, BBCodeHelper>();
      services.AddScoped<IHtmlFormatter, HtmlFormatter>();

      //slug route transformer
      services.AddSingleton<IEventPublisher, EventPublisher>();
      services.AddTransient<ISettingService, SettingService>();

      //plugin managers
      services.AddTransient(typeof(IPluginManager<>), typeof(PluginManager<>));
      services.AddTransient<IAuthenticationPluginManager, AuthenticationPluginManager>();
      services.AddTransient<IMultiFactorAuthenticationPluginManager, MultiFactorAuthenticationPluginManager>();
      services.AddTransient<IWidgetPluginManager, WidgetPluginManager>();
      services.AddTransient<IExchangeRatePluginManager, ExchangeRatePluginManager>();

      services.AddTransient<IPictureService, TestPictureService>();

      //register all settings
      var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
      foreach (var setting in settings)
         services.AddTransient(setting, context => context.GetRequiredService<ISettingService>().LoadSettingAsync(setting).Result);


      //event consumers
      foreach (var consumer in typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList())
      {
         var interfaces = consumer.FindInterfaces((type, criteria) => type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition()), typeof(IConsumer<>));
         foreach (var findInterface in interfaces)
         {
            services.AddTransient(findInterface, consumer);
         }
      }

      services.AddSingleton<IInstallationService, InstallationService>();
      services.AddScoped<IMigrationManager, MigrationManager>();

      //services.AddTransient(p => new Lazy<IVersionLoader>(p.GetRequiredService<IVersionLoader>()));

      //services
      //    // add common FluentMigrator services
      //    .AddFluentMigratorCore()
      //    .AddScoped<IProcessorAccessor, TestProcessorAccessor>()
      //    // set accessor for the connection string
      //    .AddScoped<IConnectionStringAccessor>(_ => DataSettingsManager.LoadSettings())
      //    .AddScoped<IMigrationManager, TestMigrationManager>()
      //    .AddSingleton<IConventionSet, AppTestConventionSet>()
      //    .ConfigureRunner(rb =>
      //        rb.WithVersionTable(new MigrationVersionInfo()).AddSqlServer().AddMySql5().AddPostgres().AddSQLite()
      //            // define the assembly containing the migrations
      //            .ScanIn(mAssemblies).For.Migrations());

      services.AddTransient<IWorkContext, WebWorkContext>();
      services.AddTransient<IThemeContext, ThemeContext>();

      services.AddTransient<IAppHtmlHelper, AppHtmlHelper>();

      //schedule tasks
      services.AddSingleton<ITaskScheduler, TestTaskScheduler>();
      services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

      //WebOptimizer
      services.AddWebOptimizer();

      //common factories
      services.AddTransient<ICommonModelFactory, CommonModelFactory>();
      services.AddTransient<IAddressModelFactory, AddressModelFactory>();
      services.AddTransient<IAclSupportedModelFactory, AclSupportedModelFactory>();
      services.AddTransient<ILocalizedModelFactory, LocalizedModelFactory>();
      services.AddTransient<IUserModelFactory, UserModelFactory>();
      services.AddTransient<INewsletterModelFactory, NewsletterModelFactory>();
      services.AddTransient<IProfileModelFactory, ProfileModelFactory>();
      services.AddTransient<IWidgetModelFactory, WidgetModelFactory>();

      _serviceProvider = services.BuildServiceProvider();

      EngineContext.Replace(new AppTestEngine(_serviceProvider));

      Init();
   }

   public static T GetService<T>()
   {
      try
      {
         return _serviceProvider.GetRequiredService<T>();
      }
      catch (InvalidOperationException)
      {
         return (T)EngineContext.Current.ResolveUnregistered(typeof(T));
      }
   }

   public async Task TestCrud<TEntity>(TEntity baseEntity, TEntity updateEntity, Func<TEntity, TEntity, bool> equals) where TEntity : BaseEntity
   {
      baseEntity.Id = 0;

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var repository = scope.ServiceProvider.GetRequiredService<IRepository<TEntity>>();
         await repository.InsertAsync(baseEntity);
         baseEntity.Id.Should().BeGreaterThan(0);
      }

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var repository = scope.ServiceProvider.GetRequiredService<IRepository<TEntity>>();
         updateEntity.Id = baseEntity.Id;
         await repository.UpdateAsync(updateEntity);
      }

      TEntity item;
      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var repository = scope.ServiceProvider.GetRequiredService<IRepository<TEntity>>();
         item = await repository.GetByIdAsync(baseEntity.Id);
         item.Should().NotBeNull();
         equals(updateEntity, item).Should().BeTrue();
      }

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var repository = scope.ServiceProvider.GetRequiredService<IRepository<TEntity>>();
         await repository.DeleteAsync(baseEntity);
         item = await repository.GetByIdAsync(baseEntity.Id);
         item.Should().BeNull();
      }
   }


   public static bool SetDataProviderType(DataProviderType type)
   {
      var dataConfig = Singleton<DataConfig>.Instance ?? new DataConfig();

      dataConfig.DataProvider = type;
      dataConfig.ConnectionString = string.Empty;

      try
      {
         switch (type)
         {
            case DataProviderType.SqlServer:
               dataConfig.ConnectionString = _resourceManager.GetString("sql server connection string");
               break;
            case DataProviderType.MySql:
               dataConfig.ConnectionString = _resourceManager.GetString("MySql server connection string");
               break;
            case DataProviderType.PostgreSQL:
               dataConfig.ConnectionString = _resourceManager.GetString("PostgreSql server connection string");
               break;
            case DataProviderType.Unknown:
               dataConfig.ConnectionString = "Data Source=applicationTest.sqlite;Mode=Memory;Cache=Shared";
               break;
         }
      }
      catch (MissingManifestResourceException)
      {
         //ignore
      }

      Singleton<DataConfig>.Instance = dataConfig;
      var flag = !string.IsNullOrEmpty(dataConfig.ConnectionString);

      if (Singleton<AppSettings>.Instance == null)
         return flag;

      Singleton<AppSettings>.Instance.Update(new List<IConfig> { Singleton<DataConfig>.Instance });

      return flag;
   }

   #region Nested classes

   protected class AppTestUrlHelper : UrlHelperBase
   {
      public AppTestUrlHelper(ActionContext actionContext) : base(actionContext)
      {
      }

      public override string Action(UrlActionContext actionContext)
      {
         return string.Empty;
      }

      public override string RouteUrl(UrlRouteContext routeContext)
      {
         return string.Empty;
      }
   }

   //protected class AppTestConventionSet : AppConventionSet
   //{
   //   public AppTestConventionSet(IAppDataProvider dataProvider) : base(dataProvider)
   //   {
   //   }
   //}

   public partial class AppTestEngine : AppEngine
   {
      protected readonly IServiceProvider _internalServiceProvider;

      public AppTestEngine(IServiceProvider serviceProvider)
      {
         _internalServiceProvider = serviceProvider;
      }

      public override IServiceProvider ServiceProvider => _internalServiceProvider;
   }

   public class TestAuthenticationService : IAuthenticationService
   {
      public Task SignInAsync(User user, bool isPersistent)
      {
         return Task.CompletedTask;
      }

      public Task SignOutAsync()
      {
         return Task.CompletedTask;
      }

      public async Task<User> GetAuthenticatedUserAsync()
      {
         return await _serviceProvider.GetService<IUserService>().GetUserByEmailAsync(AppTestsDefaults.AdminEmail);
      }

      public Task<Device> GetAuthenticatedDeviceAsync()
      {
         throw new NotImplementedException();
      }
   }

   protected class TestPictureService : PictureService
   {
      public TestPictureService(IDownloadService downloadService,
          IHttpContextAccessor httpContextAccessor, IAppFileProvider fileProvider,
          IRepository<Picture> pictureRepository, IRepository<PictureBinary> pictureBinaryRepository,
          ISettingService settingService, IUrlRecordService urlRecordService, IWebHelper webHelper, MediaSettings mediaSettings) :

      base(downloadService, httpContextAccessor, fileProvider, pictureRepository, pictureBinaryRepository, settingService, urlRecordService,
          webHelper, mediaSettings)
      {
      }

      // Travis doesn't support named semaphore, that's why we use implementation without it 
      public override async Task<(string Url, Picture Picture)> GetPictureUrlAsync(Picture picture,
          int targetSize = 0,
          bool showDefaultPicture = true,
          string storeLocation = null,
          PictureType defaultPictureType = PictureType.Entity)
      {
         if (picture == null)
         {
            return showDefaultPicture
                ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), null)
                : (string.Empty, (Picture)null);
         }

         byte[] pictureBinary = null;
         if (picture.IsNew)
         {
            await DeletePictureThumbsAsync(picture);
            pictureBinary = await LoadPictureBinaryAsync(picture);

            if ((pictureBinary?.Length ?? 0) == 0)
            {
               return showDefaultPicture
                   ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), picture)
                   : (string.Empty, picture);
            }

            //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
            picture = await UpdatePictureAsync(picture.Id,
                pictureBinary,
                picture.MimeType,
                picture.SeoFilename,
                picture.AltAttribute,
                picture.TitleAttribute,
                false,
                false);
         }

         var seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure

         var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);
         string thumbFileName;
         if (targetSize == 0)
         {
            thumbFileName = !string.IsNullOrEmpty(seoFileName)
                ? $"{picture.Id:0000000}_{seoFileName}.{lastPart}"
                : $"{picture.Id:0000000}.{lastPart}";

            var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
            if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
               return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            pictureBinary ??= await LoadPictureBinaryAsync(picture);

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();
            try
            {
               SaveThumbAsync(thumbFilePath, thumbFileName, string.Empty, pictureBinary).Wait();
            }
            finally
            {
               mutex.ReleaseMutex();
            }
         }
         else
         {
            thumbFileName = !string.IsNullOrEmpty(seoFileName)
                ? $"{picture.Id:0000000}_{seoFileName}_{targetSize}.{lastPart}"
                : $"{picture.Id:0000000}_{targetSize}.{lastPart}";

            var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
            if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
               return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            pictureBinary ??= await LoadPictureBinaryAsync(picture);

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();
            try
            {
               if (pictureBinary != null)
               {
                  try
                  {
                     using var image = SKBitmap.Decode(pictureBinary);
                     var format = GetImageFormatByMimeType(picture.MimeType);
                     pictureBinary = ImageResize(image, format, targetSize);
                  }
                  catch
                  {
                  }
               }

               SaveThumbAsync(thumbFilePath, thumbFileName, string.Empty, pictureBinary).Wait();
            }
            finally
            {
               mutex.ReleaseMutex();
            }
         }

         return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);
      }
   }

   #endregion
}