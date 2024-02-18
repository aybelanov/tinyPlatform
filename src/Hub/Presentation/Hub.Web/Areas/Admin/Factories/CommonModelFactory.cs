using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Configuration;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Security;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Data.Extensions;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Clients;
using Hub.Services.Cms;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Directory;
using Hub.Services.Events;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Plugins;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Areas.Admin.Models.Localization;
using Hub.Web.Framework.Models.Extensions;
using Hub.Web.Framework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Shared.Clients.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents common models factory implementation
/// </summary>
public partial class CommonModelFactory : ICommonModelFactory
{
   #region Fields

   private readonly AdminAreaSettings _adminAreaSettings;
   private readonly AppSettings _appSettings;
   private readonly CurrencySettings _currencySettings;
   private readonly IActionContextAccessor _actionContextAccessor;
   private readonly IAuthenticationPluginManager _authenticationPluginManager;
   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly ICurrencyService _currencyService;
   private readonly IUserService _userService;
   private readonly IHubDeviceService _deviceService;
   private readonly IHubSensorService _sensorService;
   private readonly IHubSensorRecordService _sensorDataService;
   private readonly IEventPublisher _eventPublisher;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IExchangeRatePluginManager _exchangeRatePluginManager;
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly IMaintenanceService _maintenanceService;
   private readonly IMeasureService _measureService;
   private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
   private readonly IAppFileProvider _fileProvider;
   private readonly IPluginService _pluginService;
   private readonly ISearchTermService _searchTermService;
   private readonly IServiceCollection _serviceCollection;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IUrlHelperFactory _urlHelperFactory;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWebHelper _webHelper;
   private readonly IWidgetPluginManager _widgetPluginManager;
   private readonly IWorkContext _workContext;
   private readonly MeasureSettings _measureSettings;
   private readonly CommonSettings _commonSettings;
   private readonly AppHttpClient _appHttpClient;
   private readonly ProxySettings _proxySettings;
   private readonly ICommunicator _communicator;

   #endregion

   #region Ctor

   public CommonModelFactory(AdminAreaSettings adminAreaSettings,
       AppSettings appSettings,
       CurrencySettings currencySettings,
       IActionContextAccessor actionContextAccessor,
       IAuthenticationPluginManager authenticationPluginManager,
       IBaseAdminModelFactory baseAdminModelFactory,
       ICurrencyService currencyService,
       IUserService userService,
       IHubDeviceService deviceService,
       IHubSensorService sensorService,
       IHubSensorRecordService sensorDataService,
       ICommunicator communicator,
       IEventPublisher eventPublisher,
       IDateTimeHelper dateTimeHelper,
       IAppFileProvider fileProvider,
       IExchangeRatePluginManager exchangeRatePluginManager,
       IHttpContextAccessor httpContextAccessor,
       ILanguageService languageService,
       ILocalizationService localizationService,
       IMaintenanceService maintenanceService,
       IMeasureService measureService,
       IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
       IPluginService pluginService,
       ISearchTermService searchTermService,
       IServiceCollection serviceCollection,
       IStaticCacheManager staticCacheManager,
       IUrlHelperFactory urlHelperFactory,
       IUrlRecordService urlRecordService,
       IWebHelper webHelper,
       IWidgetPluginManager widgetPluginManager,
       IWorkContext workContext,
       CommonSettings commonSettings,
       MeasureSettings measureSettings,
       AppHttpClient appHttpClient,
       ProxySettings proxySettings)
   {
      _adminAreaSettings = adminAreaSettings;
      _appSettings = appSettings;
      _currencySettings = currencySettings;
      _actionContextAccessor = actionContextAccessor;
      _authenticationPluginManager = authenticationPluginManager;
      _baseAdminModelFactory = baseAdminModelFactory;
      _currencyService = currencyService;
      _userService = userService;
      _deviceService = deviceService;
      _communicator = communicator;
      _sensorDataService = sensorDataService;
      _sensorService = sensorService;
      _eventPublisher = eventPublisher;
      _dateTimeHelper = dateTimeHelper;
      _exchangeRatePluginManager = exchangeRatePluginManager;
      _httpContextAccessor = httpContextAccessor;
      _languageService = languageService;
      _localizationService = localizationService;
      _maintenanceService = maintenanceService;
      _measureService = measureService;
      _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
      _fileProvider = fileProvider;
      _pluginService = pluginService;
      _searchTermService = searchTermService;
      _serviceCollection = serviceCollection;
      _staticCacheManager = staticCacheManager;
      _urlHelperFactory = urlHelperFactory;
      _urlRecordService = urlRecordService;
      _webHelper = webHelper;
      _widgetPluginManager = widgetPluginManager;
      _workContext = workContext;
      _measureSettings = measureSettings;
      _appHttpClient = appHttpClient;
      _proxySettings = proxySettings;
      _commonSettings = commonSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare platfrom URL warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PreparePlatformUrlWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //check whether current platform URL matches the platform configured URL

      var currentAppUrl = Singleton<AppSettings>.Instance.Get<HostingConfig>().HubHostUrl.Trim('/') + "/";
      if (!string.IsNullOrEmpty(currentAppUrl) && (currentAppUrl.Equals(_webHelper.GetAppLocation(false), StringComparison.InvariantCultureIgnoreCase) ||
          currentAppUrl.Equals(_webHelper.GetAppLocation(true), StringComparison.InvariantCultureIgnoreCase)))
      {
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.Match")
         });
         return;
      }

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Fail,
         Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.NoMatch"),
              currentAppUrl, _webHelper.GetAppLocation(false))
      });
   }

   /// <summary>
   /// Prepare copyright removal key warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareRemovalKeyWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      if (!_adminAreaSettings.CheckCopyrightRemovalKey)
         return;

      //try to get a warning
      var warning = string.Empty; //nameof(CommonModelFactory.PrepareRemovalKeyWarningModelAsync);
      try
      {
         // TODO copyleft
         //warning = await _appHttpClient.GetCopyrightWarningAsync();
         await Task.CompletedTask;
      }
      catch
      {
         // ignored
      }

      if (string.IsNullOrEmpty(warning))
         return;

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.CopyleftRemovalKey,
         Text = warning,
         DontEncode = true //this text could contain links, so don't encode it
      });
   }

   /// <summary>
   /// Prepare primary exchange rate currency warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareExchangeRateCurrencyWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //check whether primary exchange rate currency set
      var primaryExchangeRateCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId);
      if (primaryExchangeRateCurrency == null)
      {
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.NotSet")
         });
         return;
      }

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Pass,
         Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.Set")
      });

      //check whether primary exchange rate currency rate configured
      if (primaryExchangeRateCurrency.Rate != 1)
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.Rate1")
         });
   }

   /// <summary>
   /// Prepare primary platform currency warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PreparePrimaryApplicationCurrencyWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //check whether primary platform currency set
      var primaryPlatformCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryPlatformCurrencyId);
      if (primaryPlatformCurrency == null)
      {
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.PrimaryCurrency.NotSet")
         });
         return;
      }

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Pass,
         Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.PrimaryCurrency.Set")
      });
   }

   /// <summary>
   /// Prepare base weight warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareBaseWeightWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //check whether base measure weight set
      var baseWeight = await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
      if (baseWeight == null)
      {
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.NotSet")
         });
         return;
      }

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Pass,
         Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Set")
      });

      //check whether base measure weight ratio configured
      if (baseWeight.Ratio != 1)
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Ratio1")
         });
   }

   /// <summary>
   /// Prepare base dimension warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareBaseDimensionWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //check whether base measure dimension set
      var baseDimension = await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
      if (baseDimension == null)
      {
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.NotSet")
         });
         return;
      }

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Pass,
         Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Set")
      });

      //check whether base measure dimension ratio configured
      if (baseDimension.Ratio != 1)
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Ratio1")
         });
   }


   /// <summary>
   /// Prepare plugins warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PreparePluginsWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //check whether there are incompatible plugins
      foreach (var pluginName in _pluginService.GetIncompatiblePlugins())
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Warning,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotLoaded"), pluginName)
         });

      //check whether there are any collision of loaded assembly
      foreach (var assembly in _pluginService.GetAssemblyCollisions())
      {
         //get plugin references message
         var message = (await assembly.Collisions
             .SelectAwait(async item => string.Format(await _localizationService
                 .GetResourceAsync("Admin.System.Warnings.PluginRequiredAssembly"), item.PluginName, item.AssemblyName))
             .AggregateAsync("", (curent, all) => all + ", " + curent)).TrimEnd(',', ' ');

         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Warning,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.AssemblyHasCollision"),
                 assembly.ShortName, assembly.AssemblyFullNameInMemory, message)
         });
      }

      //check whether there are different plugins which try to override the same interface
      var baseLibraries = new[] { "Hub.Core", "Hub.Data", "Hub.Services", "Hub.Web", "Hub.Web.Framework" };
      var overridenServices = _serviceCollection.Where(p =>
              p.ServiceType.FullName != null &&
              p.ServiceType.FullName.StartsWith("Hub.", StringComparison.InvariantCulture) &&
              !p.ServiceType.FullName.StartsWith(
                  typeof(IConsumer<>).FullName?.Replace("~1", string.Empty) ?? string.Empty,
                  StringComparison.InvariantCulture)).Select(p =>
              KeyValuePair.Create(p.ServiceType.FullName, p.ImplementationType?.Assembly.GetName().Name))
          .Where(p => baseLibraries.All(library =>
              !p.Value?.StartsWith(library, StringComparison.InvariantCultureIgnoreCase) ?? false))
          .GroupBy(p => p.Key, p => p.Value)
          .Where(p => p.Count() > 1)
          .ToDictionary(p => p.Key, p => p.ToList());

      foreach (var overridenService in overridenServices)
      {
         var assemblies = overridenService.Value
             .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Warning,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginsOverrideSameService"), overridenService.Key, assemblies)
         });
      }
   }

   /// <summary>
   /// Prepare performance settings warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PreparePerformanceSettingsWarningModelAsync(IList<SystemWarningModel> models)
   {
      ArgumentNullException.ThrowIfNull(models);

      //check whether "IgnoreAcl" setting disabled
      if (_commonSettings.IgnoreAcl)
      {
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.Performance.IgnoreAcl")
         });

         return;
      }

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Recommendation,
         Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.Performance.IgnoreAcl")
      });
   }

   /// <summary>
   /// Prepare file permissions warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareFilePermissionsWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      var dirPermissionsOk = true;
      var dirsToCheck = _fileProvider.GetDirectoriesWrite();
      foreach (var dir in dirsToCheck)
      {
         if (_fileProvider.CheckPermissions(dir, false, true, true, false))
            continue;

         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Warning,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.Wrong"),
                 CurrentOSUser.FullName, dir)
         });
         dirPermissionsOk = false;
      }

      if (dirPermissionsOk)
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.OK")
         });

      var filePermissionsOk = true;
      var filesToCheck = _fileProvider.GetFilesWrite();
      foreach (var file in filesToCheck)
      {
         if (_fileProvider.CheckPermissions(file, false, true, true, true))
            continue;

         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Warning,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.Wrong"),
                 CurrentOSUser.FullName, file)
         });
         filePermissionsOk = false;
      }

      if (filePermissionsOk)
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.OK")
         });
   }

   /// <summary>
   /// Prepare backup file search model
   /// </summary>
   /// <param name="searchModel">Backup file search model</param>
   /// <returns>Backup file search model</returns>
   protected virtual BackupFileSearchModel PrepareBackupFileSearchModel(BackupFileSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare plugins installed warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PreparePluginsInstalledWarningModelAsync(List<SystemWarningModel> models)
   {
      var plugins = await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.NotInstalledOnly);

      var notInstalled = plugins.Select(p => p.FriendlyName).ToList();

      if (!notInstalled.Any())
         return;

      models.Add(new SystemWarningModel
      {
         Level = SystemWarningLevel.Warning,
         DontEncode = true,
         Text = $"{await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotInstalled")}: {string.Join(", ", notInstalled)}. {await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotInstalled.HelpText")}"
      });
   }

   /// <summary>
   /// Prepare plugins enabled warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PreparePluginsEnabledWarningModelAsync(List<SystemWarningModel> models)
   {
      var plugins = await _pluginService.GetPluginsAsync<IPlugin>();

      var notEnabled = new List<string>();
      var notEnabledSystemNames = new List<string>();

      foreach (var plugin in plugins)
      {
         var isEnabled = true;

         switch (plugin)
         {
            case IExternalAuthenticationMethod externalAuthenticationMethod:
               isEnabled = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
               break;

            case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
               isEnabled = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
               break;

            case IWidgetPlugin widgetPlugin:
               isEnabled = _widgetPluginManager.IsPluginActive(widgetPlugin);
               break;

            case IExchangeRateProvider exchangeRateProvider:
               isEnabled = _exchangeRatePluginManager.IsPluginActive(exchangeRateProvider);
               break;
         }

         if (isEnabled)
            continue;

         notEnabled.Add(plugin.PluginDescriptor.FriendlyName);
         notEnabledSystemNames.Add(plugin.PluginDescriptor.SystemName);
      }

      if (notEnabled.Any())
      {
         //get URL helper
         var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Warning,
            DontEncode = true,

            Text = $"{await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotEnabled")}: {string.Join(", ", notEnabled)} (<a href=\"{urlHelper.Action("UninstallAndDeleteUnusedPlugins", "Plugin", new { names = notEnabledSystemNames.ToArray() })}\">{await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotEnabled.AutoFixAndRestart")}</a>)"
         });
      }
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare system info model
   /// </summary>
   /// <param name="model">System info model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the system info model
   /// </returns>
   public virtual async Task<SystemInfoModel> PrepareSystemInfoModelAsync(SystemInfoModel model)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      model.AppVersion = AppVersion.FULL_VERSION;
      model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
      model.ServerLocalTime = DateTime.Now;
      model.UtcTime = DateTime.UtcNow;
      model.CurrentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
      model.HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];

      //ensure no exception is thrown
      try
      {
         model.OperatingSystem = Environment.OSVersion.VersionString;
         model.AspNetInfo = RuntimeInformation.FrameworkDescription;
         model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted;
      }
      catch
      {
         // ignored
      }

      foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
         model.Headers.Add(new SystemInfoModel.HeaderModel
         {
            Name = header.Key,
            Value = header.Value
         });

      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
         var loadedAssemblyModel = new SystemInfoModel.LoadedAssembly
         {
            FullName = assembly.FullName
         };

         //ensure no exception is thrown
         try
         {
            loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
            loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

            //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
            //we use a simple method because the more Jeff Atwood's solution doesn't work anymore 
            //more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/
            loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(_fileProvider.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

         }
         catch
         {
            // ignored
         }

         model.LoadedAssemblies.Add(loadedAssemblyModel);
      }


      var currentStaticCacheManagerName = _staticCacheManager.GetType().Name;

      if (_appSettings.Get<DistributedCacheConfig>().Enabled)
         currentStaticCacheManagerName +=
             $"({await _localizationService.GetLocalizedEnumAsync(_appSettings.Get<DistributedCacheConfig>().DistributedCacheType)})";

      model.CurrentStaticCacheManager = currentStaticCacheManagerName;

      model.AzureBlobStorageEnabled = _appSettings.Get<AzureBlobConfig>().Enabled;

      return model;
   }

   /// <summary>
   /// Prepare proxy connection warning model
   /// </summary>
   /// <param name="models">List of system warning models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareProxyConnectionWarningModelAsync(IList<SystemWarningModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //whether proxy is enabled
      if (!_proxySettings.Enabled)
         return;

      try
      {
         await _appHttpClient.PingAsync();

         //connection is OK
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.OK")
         });
      }
      catch
      {
         //connection failed
         models.Add(new SystemWarningModel
         {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.Failed")
         });
      }
   }

   /// <summary>
   /// Prepare system warning models
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of system warning models
   /// </returns>
   public virtual async Task<IList<SystemWarningModel>> PrepareSystemWarningModelsAsync()
   {
      var models = new List<SystemWarningModel>();

      //store URL
      await PreparePlatformUrlWarningModelAsync(models);

      //removal key
      await PrepareRemovalKeyWarningModelAsync(models);

      ////primary exchange rate currency
      //await PrepareExchangeRateCurrencyWarningModelAsync(models);

      ////primary platform currency
      //await PreparePrimaryApplicationCurrencyWarningModelAsync(models);

      //base measure weight
      await PrepareBaseWeightWarningModelAsync(models);

      //base dimension weight
      await PrepareBaseDimensionWarningModelAsync(models);

      //plugins
      await PreparePluginsWarningModelAsync(models);

      //performance settings
      await PreparePerformanceSettingsWarningModelAsync(models);

      //validate write permissions (the same procedure like during installation)
      await PrepareFilePermissionsWarningModelAsync(models);

      //not active plugins
      await PreparePluginsEnabledWarningModelAsync(models);

      //not install plugins
      await PreparePluginsInstalledWarningModelAsync(models);

      //proxy connection
      await PrepareProxyConnectionWarningModelAsync(models);

      //publish event
      var warningEvent = new SystemWarningCreatedEvent();
      await _eventPublisher.PublishAsync(warningEvent);
      //add another warnings (for example from plugins) 
      models.AddRange(warningEvent.SystemWarnings);

      return models;
   }

   /// <summary>
   /// Prepare maintenance model
   /// </summary>
   /// <param name="model">Maintenance model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the maintenance model
   /// </returns>
   public virtual Task<MaintenanceModel> PrepareMaintenanceModelAsync(MaintenanceModel model)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
      model.DeleteAbandonedDevices.OlderThan = DateTime.UtcNow.AddDays(-182);

      model.DeleteAlreadySentQueuedEmails.EndDate = DateTime.UtcNow.AddDays(-7);

      // TODO backup supported
      //model.BackupSupported = _dataProvider.BackupSupported;

      //prepare nested search model
      PrepareBackupFileSearchModel(model.BackupFileSearchModel);

      return Task.FromResult(model);
   }

   /// <summary>
   /// Prepare paged backup file list model
   /// </summary>
   /// <param name="searchModel">Backup file search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the backup file list model
   /// </returns>
   public virtual Task<BackupFileListModel> PrepareBackupFileListModelAsync(BackupFileSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get backup files
      var backupFiles = _maintenanceService.GetAllBackupFiles().ToPagedList(searchModel);

      //prepare list model
      var model = new BackupFileListModel().PrepareToGrid(searchModel, backupFiles, () =>
      {
         return backupFiles.Select(file => new BackupFileModel
         {
            Name = _fileProvider.GetFileName(file),

            //fill in additional values (not existing in the entity)
            Length = $"{_fileProvider.FileLength(file) / 1024f / 1024f:F2} Mb",

            Link = $"{_webHelper.GetAppLocation()}db_backups/{_fileProvider.GetFileName(file)}"
         });
      });

      return Task.FromResult(model);
   }

   /// <summary>
   /// Prepare URL record search model
   /// </summary>
   /// <param name="searchModel">URL record search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the uRL record search model
   /// </returns>
   public virtual async Task<UrlRecordSearchModel> PrepareUrlRecordSearchModelAsync(UrlRecordSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare available languages
      //we insert 0 as 'Standard' language.
      //let's insert -1 for 'All' language selection.
      await _baseAdminModelFactory.PrepareLanguagesAsync(searchModel.AvailableLanguages,
          defaultItemText: await _localizationService.GetResourceAsync("Admin.System.SeNames.List.Language.Standard"));
      searchModel.AvailableLanguages.Insert(0,
          new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "-1" });
      searchModel.LanguageId = -1;

      //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
      searchModel.AvailableActiveOptions.Add(new SelectListItem
      {
         Value = "0",
         Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.All")
      });
      searchModel.AvailableActiveOptions.Add(new SelectListItem
      {
         Value = "1",
         Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.ActiveOnly")
      });
      searchModel.AvailableActiveOptions.Add(new SelectListItem
      {
         Value = "2",
         Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.InactiveOnly")
      });

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged URL record list model
   /// </summary>
   /// <param name="searchModel">URL record search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the uRL record list model
   /// </returns>
   public virtual async Task<UrlRecordListModel> PrepareUrlRecordListModelAsync(UrlRecordSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);
      var languageId = searchModel.LanguageId < 0 ? null : (int?)searchModel.LanguageId;

      //get URL records
      var urlRecords = await _urlRecordService.GetAllUrlRecordsAsync(slug: searchModel.SeName,
          languageId: languageId, isActive: isActive,
          pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //get URL helper
      var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

      //prepare list model
      var model = await new UrlRecordListModel().PrepareToGridAsync(searchModel, urlRecords, () =>
      {
         return urlRecords.SelectAwait(async urlRecord =>
            {
               //fill in model values from the entity
               var urlRecordModel = urlRecord.ToModel<UrlRecordModel>();

               //fill in additional values (not existing in the entity)
               urlRecordModel.Name = urlRecord.Slug;
               urlRecordModel.Language = urlRecord.LanguageId == 0
                      ? await _localizationService.GetResourceAsync("Admin.System.SeNames.Language.Standard")
                      : (await _languageService.GetLanguageByIdAsync(urlRecord.LanguageId))?.Name ?? "Unknown";

               //details URL
               var detailsUrl = string.Empty;
               var entityName = urlRecord.EntityName?.ToLowerInvariant() ?? string.Empty;
               switch (entityName)
               {
                  case "blogpost":
                     detailsUrl = urlHelper.Action("BlogPostEdit", "Blog", new { id = urlRecord.EntityId });
                     break;
                  case "newsitem":
                     detailsUrl = urlHelper.Action("NewsItemEdit", "News", new { id = urlRecord.EntityId });
                     break;
                  case "topic":
                     detailsUrl = urlHelper.Action("Edit", "Topic", new { id = urlRecord.EntityId });
                     break;
               }

               urlRecordModel.DetailsUrl = detailsUrl;

               return urlRecordModel;
            });
      });
      return model;
   }

   /// <summary>
   /// Prepare language selector model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language selector model
   /// </returns>
   public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
   {
      var model = new LanguageSelectorModel
      {
         CurrentLanguage = (await _workContext.GetWorkingLanguageAsync()).ToModel<LanguageModel>(),
         AvailableLanguages = (await _languageService
              .GetAllLanguagesAsync())
              .Select(language => language.ToModel<LanguageModel>()).ToList()
      };

      return model;
   }

   /// <summary>
   /// Prepare popular search term search model
   /// </summary>
   /// <param name="searchModel">Popular search term search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the popular search term search model
   /// </returns>
   public virtual Task<PopularSearchTermSearchModel> PreparePopularSearchTermSearchModelAsync(PopularSearchTermSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize(5);

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged popular search term list model
   /// </summary>
   /// <param name="searchModel">Popular search term search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the popular search term list model
   /// </returns>
   public virtual async Task<PopularSearchTermListModel> PreparePopularSearchTermListModelAsync(PopularSearchTermSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get popular search terms
      var searchTermRecordLines = await _searchTermService.GetStatsAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //prepare list model
      var model = new PopularSearchTermListModel().PrepareToGrid(searchModel, searchTermRecordLines, () =>
      {
         return searchTermRecordLines.Select(searchTerm => new PopularSearchTermModel
         {
            Keyword = searchTerm.Keyword,
            Count = searchTerm.Count
         });
      });

      return model;
   }

   /// <summary>
   /// Prepare common statistics model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the common statistics model
   /// </returns>
   public virtual async Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync()
   {
      var model = new CommonStatisticsModel();

      var userRoleIds = new[] { (await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName)).Id };
      model.NumberOfUsers = (await _userService.GetAllUsersAsync(userRoleIds: userRoleIds, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

      model.NumberOfOnlineUsers = (await _communicator.GetOnlineUserIdsAsync()).Count;

      model.NumberOfDevices = (await _deviceService.GetAllPagedDevicesAsync(pageIndex: 0, pageSize: 0, getOnlyTotalCount: true)).TotalCount;

      model.NumberOfOnlineDevices = (await _communicator.GetOnlineDeviceIdsAsync()).Count;

      model.NumberOfSensors = (await _sensorService.GetAllPagedSensorsAsync(pageIndex: 0, pageSize: 0, getOnlyTotalCount: true)).TotalCount;

      model.NumberOfRecords = (await _sensorDataService.GetAllPagedSensorDataAsync(pageIndex: 0, pageSize: 0, getOnlyTotalCount: true)).TotalCount;

      return model;
   }

   #endregion
}