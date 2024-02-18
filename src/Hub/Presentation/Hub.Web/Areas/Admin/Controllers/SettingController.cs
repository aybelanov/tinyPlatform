using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Gdpr;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Seo;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Data.Configuration;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Gdpr;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Media.RoxyFileman;
using Hub.Services.Messages;
using Hub.Services.Plugins;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Settings;
using Hub.Web.Framework;
using Hub.Web.Framework.Configuration;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class SettingController : BaseAdminController
{
   #region Fields

   private readonly AppSettings _appSettings;
   private readonly IAddressService _addressService;
   private readonly IUserActivityService _userActivityService;
   private readonly IUserService _userService;
   private readonly AppDbContext _dataProvider;
   private readonly IEncryptionService _encryptionService;
   private readonly IEventPublisher _eventPublisher;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IGdprService _gdprService;
   private readonly ILocalizedEntityService _localizedEntityService;
   private readonly ILocalizationService _localizationService;
   private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
   private readonly IAppFileProvider _fileProvider;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IPictureService _pictureService;
   private readonly IRoxyFilemanService _roxyFilemanService;
   private readonly IServiceScopeFactory _serviceScopeFactory;
   private readonly ISettingModelFactory _settingModelFactory;
   private readonly ISettingService _settingService;
   private readonly IWorkContext _workContext;
   private readonly IUploadService _uploadService;

   #endregion

   #region Ctor

   public SettingController(AppSettings appSettings,
       IAddressService addressService,
       IUserActivityService userActivityService,
       IUserService userService,
       AppDbContext dataProvider,
       IEncryptionService encryptionService,
       IEventPublisher eventPublisher,
       IGenericAttributeService genericAttributeService,
       IGdprService gdprService,
       ILocalizedEntityService localizedEntityService,
       ILocalizationService localizationService,
       IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
       IAppFileProvider fileProvider,
       INotificationService notificationService,
       IPermissionService permissionService,
       IPictureService pictureService,
       IRoxyFilemanService roxyFilemanService,
       IServiceScopeFactory serviceScopeFactory,
       ISettingModelFactory settingModelFactory,
       ISettingService settingService,
       IWorkContext workContext,
       IUploadService uploadService)
   {
      _appSettings = appSettings;
      _addressService = addressService;
      _userActivityService = userActivityService;
      _userService = userService;
      _dataProvider = dataProvider;
      _encryptionService = encryptionService;
      _eventPublisher = eventPublisher;
      _genericAttributeService = genericAttributeService;
      _gdprService = gdprService;
      _localizedEntityService = localizedEntityService;
      _localizationService = localizationService;
      _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
      _fileProvider = fileProvider;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _pictureService = pictureService;
      _roxyFilemanService = roxyFilemanService;
      _serviceScopeFactory = serviceScopeFactory;
      _settingModelFactory = settingModelFactory;
      _settingService = settingService;
      _workContext = workContext;
      _uploadService = uploadService;
   }

   #endregion

   #region Utilities

   protected virtual async Task UpdateGdprConsentLocalesAsync(GdprConsent gdprConsent, GdprConsentModel model)
   {
      foreach (var localized in model.Locales)
      {
         await _localizedEntityService.SaveLocalizedValueAsync(gdprConsent,
             x => x.Message,
             localized.Message,
             localized.LanguageId);

         await _localizedEntityService.SaveLocalizedValueAsync(gdprConsent,
             x => x.RequiredMessage,
             localized.RequiredMessage,
             localized.LanguageId);
      }
   }

   #endregion

   #region Methods

   #region User settings

   public virtual async Task<IActionResult> UserUser(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareAllUserSettingsModelAsync();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> UserUser(UserUserSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var userSettings = await _settingService.LoadSettingAsync<UserSettings>();

         var lastUsernameValidationRule = userSettings.UsernameValidationRule;
         var lastUsernameValidationEnabledValue = userSettings.UsernameValidationEnabled;
         var lastUsernameValidationUseRegexValue = userSettings.UsernameValidationUseRegex;

         //Phone number validation settings
         var lastPhoneNumberValidationRule = userSettings.PhoneNumberValidationRule;
         var lastPhoneNumberValidationEnabledValue = userSettings.PhoneNumberValidationEnabled;
         var lastPhoneNumberValidationUseRegexValue = userSettings.PhoneNumberValidationUseRegex;

         var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>();
         var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>();
         var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>();
         var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>();

         userSettings = model.UserSettings.ToSettings(userSettings);

         if (userSettings.UsernameValidationEnabled && userSettings.UsernameValidationUseRegex)
         {
            try
            {
               //validate regex rule
               var unused = Regex.IsMatch("test_user_name", userSettings.UsernameValidationRule);
            }
            catch (ArgumentException)
            {
               //restoring previous settings
               userSettings.UsernameValidationRule = lastUsernameValidationRule;
               userSettings.UsernameValidationEnabled = lastUsernameValidationEnabledValue;
               userSettings.UsernameValidationUseRegex = lastUsernameValidationUseRegexValue;

               _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerSettings.RegexValidationRule.Error"));
            }
         }

         if (userSettings.PhoneNumberValidationEnabled && userSettings.PhoneNumberValidationUseRegex)
         {
            try
            {
               //validate regex rule
               var unused = Regex.IsMatch("123456789", userSettings.PhoneNumberValidationRule);
            }
            catch (ArgumentException)
            {
               //restoring previous settings
               userSettings.PhoneNumberValidationRule = lastPhoneNumberValidationRule;
               userSettings.PhoneNumberValidationEnabled = lastPhoneNumberValidationEnabledValue;
               userSettings.PhoneNumberValidationUseRegex = lastPhoneNumberValidationUseRegexValue;

               _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerSettings.PhoneNumberRegexValidationRule.Error"));
            }
         }

         await _settingService.SaveSettingAsync(userSettings);

         addressSettings = model.AddressSettings.ToSettings(addressSettings);
         await _settingService.SaveSettingAsync(addressSettings);

         dateTimeSettings.DefaultPlatformTimeZoneId = model.DateTimeSettings.DefaultPlatformTimeZoneId;
         dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
         await _settingService.SaveSettingAsync(dateTimeSettings);

         externalAuthenticationSettings.AllowUsersToRemoveAssociations = model.ExternalAuthenticationSettings.AllowUsersToRemoveAssociations;
         await _settingService.SaveSettingAsync(externalAuthenticationSettings);

         multiFactorAuthenticationSettings = model.MultiFactorAuthenticationSettings.ToSettings(multiFactorAuthenticationSettings);
         await _settingService.SaveSettingAsync(multiFactorAuthenticationSettings);

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("UserUser");
      }

      //prepare model
      model = await _settingModelFactory.PrepareAllUserSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Device settings

   public virtual async Task<IActionResult> Device(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareDeviceSettingsModelAsync();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      //if we got this far, something failed, redisplay form
      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> Device(DeviceSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var deviceSettings = await _settingService.LoadSettingAsync<DeviceSettings>();

         deviceSettings = model.ToSettings(deviceSettings);

         await _settingService.SaveSettingAsync(deviceSettings);

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("Device");
      }

      //prepare model
      model = await _settingModelFactory.PrepareDeviceSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Application settings

   public virtual async Task<IActionResult> AppSettings(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAppSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareAppSettingsModel();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> AppSettings(AppSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAppSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var configurations = new List<IConfig>
         {
            model.CacheConfigModel.ToConfig(_appSettings.Get<CacheConfig>()),
            model.HostingConfigModel.ToConfig(_appSettings.Get<HostingConfig>()),
            model.HostingConfigModel.ToConfig(_appSettings.Get<ServerConfig>()),
            model.SecurityConfigModel.ToConfig(_appSettings.Get<SecurityConfig>()),
            model.DistributedCacheConfigModel.ToConfig(_appSettings.Get<DistributedCacheConfig>()),
            model.AzureBlobConfigModel.ToConfig(_appSettings.Get<AzureBlobConfig>()),
            model.InstallationConfigModel.ToConfig(_appSettings.Get<InstallationConfig>()),
            model.PluginConfigModel.ToConfig(_appSettings.Get<PluginConfig>()),
            model.CommonConfigModel.ToConfig(_appSettings.Get<CommonConfig>()),
            model.DataConfigModel.ToConfig(_appSettings.Get<DataConfig>()),
            model.WebOptimizerConfigModel.ToConfig(_appSettings.Get<WebOptimizerConfig>())
         };

         await _eventPublisher.PublishAsync(new AppSettingsSavingEvent(configurations));

         AppSettingsHelper.SaveAppSettings(configurations, _fileProvider);

         await _userActivityService.InsertActivityAsync("EditSettings",
             await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(
             await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         var returnUrl = Url.Action("AppSettings", "Setting", new { area = AreaNames.Admin });
         return View("RestartApplication", returnUrl);
      }

      //prepare model
      model = await _settingModelFactory.PrepareAppSettingsModel(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Blog

   public virtual async Task<IActionResult> Blog(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareBlogSettingsModelAsync();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Blog(BlogSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>();
         blogSettings = model.ToSettings(blogSettings);

         await _settingService.SaveSettingAsync(blogSettings);

         //now clear settings cache
         await _settingService.ClearCacheAsync();

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("Blog");
      }

      //prepare model
      model = await _settingModelFactory.PrepareBlogSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Forum

   public virtual async Task<IActionResult> Forum(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareForumSettingsModelAsync();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Forum(ForumSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         //load settings for a chosen platform scope
         var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>();
         forumSettings = model.ToSettings(forumSettings);

         await _settingService.SaveSettingAsync(forumSettings);

         //now clear settings cache
         await _settingService.ClearCacheAsync();

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("Forum");
      }

      //prepare model
      model = await _settingModelFactory.PrepareForumSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region News

   public virtual async Task<IActionResult> News(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareNewsSettingsModelAsync();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> News(NewsSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>();
         newsSettings = model.ToSettings(newsSettings);

         await _settingService.SaveSettingAsync(newsSettings);
         
         //now clear settings cache
         await _settingService.ClearCacheAsync();

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("News");
      }

      //prepare model
      model = await _settingModelFactory.PrepareNewsSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Sort options

   // TODO implement sort option settings
   [HttpPost]
   public virtual async Task<IActionResult> SortOptionsList(SortOptionSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return await AccessDeniedDataTablesJson();

      //prepare model
      //var model = await _settingModelFactory.PrepareSortOptionListModelAsync(searchModel);

      return Json(new SortOptionListModel());
   }

   #endregion

   #region Media

   public virtual async Task<IActionResult> Media()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareMediaSettingsModelAsync();

      return View(model);
   }

   [HttpPost]
   [FormValueRequired("save")]
   public virtual async Task<IActionResult> Media(MediaSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>();
         mediaSettings = model.ToSettings(mediaSettings);

         await _settingService.SaveSettingAsync(mediaSettings);
         
         //now clear settings cache
         await _settingService.ClearCacheAsync();

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("Media");
      }

      //prepare model
      model = await _settingModelFactory.PrepareMediaSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost, ActionName("Media")]
   [FormValueRequired("change-picture-storage")]
   public virtual async Task<IActionResult> ChangePictureStorage()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      await _pictureService.SetIsStoreInDbAsync(!await _pictureService.IsStoreInDbAsync());
     
      //activity log
      await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

      return RedirectToAction("Media");
   }

   #endregion

   #region GDPR

   public virtual async Task<IActionResult> Gdpr()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareGdprSettingsModelAsync();

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Gdpr(GdprSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         //load settings for a chosen platform scope
         var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>();
         gdprSettings = model.ToSettings(gdprSettings);

         await _settingService.SaveSettingAsync(gdprSettings);

         //now clear settings cache
         await _settingService.ClearCacheAsync();

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("Gdpr");
      }

      //prepare model
      model = await _settingModelFactory.PrepareGdprSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> GdprConsentList(GdprConsentSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _settingModelFactory.PrepareGdprConsentListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> CreateGdprConsent()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareGdprConsentModelAsync(new GdprConsentModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> CreateGdprConsent(GdprConsentModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var gdprConsent = model.ToEntity<GdprConsent>();
         await _gdprService.InsertConsentAsync(gdprConsent);

         //locales                
         await UpdateGdprConsentLocalesAsync(gdprConsent, model);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Added"));

         return continueEditing ? RedirectToAction("EditGdprConsent", new { gdprConsent.Id }) : RedirectToAction("Gdpr");
      }

      //prepare model
      model = await _settingModelFactory.PrepareGdprConsentModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> EditGdprConsent(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //try to get a consent with the specified id
      var gdprConsent = await _gdprService.GetConsentByIdAsync(id);
      if (gdprConsent == null)
         return RedirectToAction("Gdpr");

      //prepare model
      var model = await _settingModelFactory.PrepareGdprConsentModelAsync(null, gdprConsent);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> EditGdprConsent(GdprConsentModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //try to get a GDPR consent with the specified id
      var gdprConsent = await _gdprService.GetConsentByIdAsync(model.Id);
      if (gdprConsent == null)
         return RedirectToAction("Gdpr");

      if (ModelState.IsValid)
      {
         gdprConsent = model.ToEntity(gdprConsent);
         await _gdprService.UpdateConsentAsync(gdprConsent);

         //locales                
         await UpdateGdprConsentLocalesAsync(gdprConsent, model);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Updated"));

         return continueEditing ? RedirectToAction("EditGdprConsent", gdprConsent.Id) : RedirectToAction("Gdpr");
      }

      //prepare model
      model = await _settingModelFactory.PrepareGdprConsentModelAsync(model, gdprConsent, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> DeleteGdprConsent(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //try to get a GDPR consent with the specified id
      var gdprConsent = await _gdprService.GetConsentByIdAsync(id);
      if (gdprConsent == null)
         return RedirectToAction("Gdpr");

      await _gdprService.DeleteConsentAsync(gdprConsent);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Deleted"));

      return RedirectToAction("Gdpr");
   }

   #endregion

   #region General common settings

   public virtual async Task<IActionResult> GeneralCommon(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareGeneralCommonSettingsModelAsync();

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   [FormValueRequired("save")]
   public virtual async Task<IActionResult> GeneralCommon(GeneralCommonSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         //store information settings
         var appInformationSettings = await _settingService.LoadSettingAsync<AppInfoSettings>();
         var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>();
         var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>();

         appInformationSettings.PlatformClosed = model.PlatformInformationSettings.PlatformClosed;
         appInformationSettings.DefaultAppTheme = model.PlatformInformationSettings.DefaultPlatformTheme;
         appInformationSettings.AllowUserToSelectTheme = model.PlatformInformationSettings.AllowUserToSelectTheme;
         appInformationSettings.LogoPictureId = model.PlatformInformationSettings.LogoPictureId;
         //EU Cookie law
         appInformationSettings.DisplayEuCookieLawWarning = model.PlatformInformationSettings.DisplayEuCookieLawWarning;
         //social pages
         appInformationSettings.FacebookLink = model.PlatformInformationSettings.FacebookLink;
         appInformationSettings.TwitterLink = model.PlatformInformationSettings.TwitterLink;
         appInformationSettings.YoutubeLink = model.PlatformInformationSettings.YoutubeLink;
         //contact us
         commonSettings.SubjectFieldOnContactUsForm = model.PlatformInformationSettings.SubjectFieldOnContactUsForm;
         commonSettings.UseSystemEmailForContactUsForm = model.PlatformInformationSettings.UseSystemEmailForContactUsForm;
         //terms of service
         commonSettings.PopupForTermsOfServiceLinks = model.PlatformInformationSettings.PopupForTermsOfServiceLinks;
         //sitemap
         sitemapSettings.SitemapEnabled = model.SitemapSettings.SitemapEnabled;
         sitemapSettings.SitemapPageSize = model.SitemapSettings.SitemapPageSize;
         sitemapSettings.SitemapIncludeBlogPosts = model.SitemapSettings.SitemapIncludeBlogPosts;
         sitemapSettings.SitemapIncludeNews = model.SitemapSettings.SitemapIncludeNews;
         sitemapSettings.SitemapIncludeTopics = model.SitemapSettings.SitemapIncludeTopics;

         //minification
         commonSettings.EnableHtmlMinification = model.MinificationSettings.EnableHtmlMinification;
         //use response compression
         commonSettings.UseResponseCompression = model.MinificationSettings.UseResponseCompression;

         await _settingService.SaveSettingAsync(appInformationSettings);
         await _settingService.SaveSettingAsync(commonSettings);
         await _settingService.SaveSettingAsync(sitemapSettings);

         //seo settings
         var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>();
         seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
         seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
         seoSettings.HomepageTitle = model.SeoSettings.HomepageTitle;
         seoSettings.HomepageDescription = model.SeoSettings.HomepageDescription;
         seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
         seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
         seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
         seoSettings.GenerateMetaDescription = model.SeoSettings.GenerateMetaDescription;
         seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
         seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
         seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
         seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
         seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
         seoSettings.MicrodataEnabled = model.SeoSettings.MicrodataEnabled;
         seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

         await _settingService.SaveSettingAsync(seoSettings);
         
         //security settings
         var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>();
         if (securitySettings.AdminAreaAllowedIpAddresses == null)
            securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
         securitySettings.AdminAreaAllowedIpAddresses.Clear();
         if (!string.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
            foreach (var s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
               if (!string.IsNullOrWhiteSpace(s))
                  securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
         securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
         await _settingService.SaveSettingAsync(securitySettings);

         //captcha settings
         var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>();
         captchaSettings.Enabled = model.CaptchaSettings.Enabled;
         captchaSettings.ShowOnLoginPage = model.CaptchaSettings.ShowOnLoginPage;
         captchaSettings.ShowOnRegistrationPage = model.CaptchaSettings.ShowOnRegistrationPage;
         captchaSettings.ShowOnContactUsPage = model.CaptchaSettings.ShowOnContactUsPage;
         captchaSettings.ShowOnEmailWishlistToFriendPage = model.CaptchaSettings.ShowOnEmailWishlistToFriendPage;
         captchaSettings.ShowOnEmailToFriendPage = model.CaptchaSettings.ShowOnEmailToFriendPage;
         captchaSettings.ShowOnBlogCommentPage = model.CaptchaSettings.ShowOnBlogCommentPage;
         captchaSettings.ShowOnNewsCommentPage = model.CaptchaSettings.ShowOnNewsCommentPage;
         captchaSettings.ShowOnForgotPasswordPage = model.CaptchaSettings.ShowOnForgotPasswordPage;
         captchaSettings.ShowOnForum = model.CaptchaSettings.ShowOnForum;
         captchaSettings.ReCaptchaPublicKey = model.CaptchaSettings.ReCaptchaPublicKey;
         captchaSettings.ReCaptchaPrivateKey = model.CaptchaSettings.ReCaptchaPrivateKey;
         captchaSettings.CaptchaType = (CaptchaType)model.CaptchaSettings.CaptchaType;
         captchaSettings.ReCaptchaV3ScoreThreshold = model.CaptchaSettings.ReCaptchaV3ScoreThreshold;

         await _settingService.SaveSettingAsync(captchaSettings);

         if (captchaSettings.Enabled &&
             (string.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || string.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            //captcha is enabled but the keys are not entered
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError"));

         //PDF settings
         var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>();
         pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
         pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
         pdfSettings.InvoiceFooterTextColumn1 = model.PdfSettings.InvoiceFooterTextColumn1;
         pdfSettings.InvoiceFooterTextColumn2 = model.PdfSettings.InvoiceFooterTextColumn2;

         await _settingService.SaveSettingAsync(pdfSettings);
         
         //localization settings
         var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>();
         localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
         if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;

         localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
         localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
         localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
         localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
         await _settingService.SaveSettingAsync(localizationSettings);

         //display default menu item
         var displayDefaultMenuItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultMenuItemSettings>();

         //we do not clear cache after each setting update.
         //this behavior can increase performance because cached settings will not be cleared 
         //and loaded from database after each update
         displayDefaultMenuItemSettings.DisplayHomepageMenuItem = model.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem;
         displayDefaultMenuItemSettings.DisplaySearchMenuItem = model.DisplayDefaultMenuItemSettings.DisplaySearchMenuItem;
         displayDefaultMenuItemSettings.DisplayUserInfoMenuItem = model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem;
         displayDefaultMenuItemSettings.DisplayBlogMenuItem = model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem;
         displayDefaultMenuItemSettings.DisplayForumsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem;
         displayDefaultMenuItemSettings.DisplayContactUsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem;

         await _settingService.SaveSettingAsync(displayDefaultMenuItemSettings);

         //display default footer item
         var displayDefaultFooterItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultFooterItemSettings>();

         //we do not clear cache after each setting update.
         //this behavior can increase performance because cached settings will not be cleared 
         //and loaded from database after each update
         displayDefaultFooterItemSettings.DisplaySitemapFooterItem = model.DisplayDefaultFooterItemSettings.DisplaySitemapFooterItem;
         displayDefaultFooterItemSettings.DisplayContactUsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayContactUsFooterItem;
         displayDefaultFooterItemSettings.DisplayUserInfoFooterItem = model.DisplayDefaultFooterItemSettings.DisplayUserInfoFooterItem;
         displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem = model.DisplayDefaultFooterItemSettings.DisplayUserAddressesFooterItem;
         displayDefaultFooterItemSettings.DisplayNewsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayNewsFooterItem;
         displayDefaultFooterItemSettings.DisplayBlogFooterItem = model.DisplayDefaultFooterItemSettings.DisplayBlogFooterItem;
         displayDefaultFooterItemSettings.DisplayForumsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayForumsFooterItem;

         await _settingService.SaveSettingAsync(displayDefaultFooterItemSettings);
        
         //admin area
         var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>();

         //we do not clear cache after each setting update.
         //this behavior can increase performance because cached settings will not be cleared 
         //and loaded from database after each update
         adminAreaSettings.UseRichEditorInMessageTemplates = model.AdminAreaSettings.UseRichEditorInMessageTemplates;

         await _settingService.SaveSettingAsync(adminAreaSettings);

         //activity log
         await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

         return RedirectToAction("GeneralCommon");
      }

      //prepare model
      model = await _settingModelFactory.PrepareGeneralCommonSettingsModelAsync(model);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost, ActionName("GeneralCommon")]
   [FormValueRequired("changeencryptionkey")]
   public virtual async Task<IActionResult> ChangeEncryptionKey(GeneralCommonSettingsModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>();

      try
      {
         if (model.SecuritySettings.EncryptionKey == null)
            model.SecuritySettings.EncryptionKey = string.Empty;

         model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

         var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
         if (string.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
            throw new AppException(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

         var oldEncryptionPrivateKey = securitySettings.EncryptionKey;
         if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
            throw new AppException(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

         //update password information
         //optimization - load only passwords with PasswordFormat.Encrypted
         var userPasswords = await _userService.GetUserPasswordsAsync(passwordFormat: PasswordFormat.Encrypted);
         foreach (var userPassword in userPasswords)
         {
            var decryptedPassword = _encryptionService.DecryptText(userPassword.Password, oldEncryptionPrivateKey);
            var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

            userPassword.Password = encryptedPassword;
            await _userService.UpdateUserPasswordAsync(userPassword);
         }

         securitySettings.EncryptionKey = newEncryptionPrivateKey;
         await _settingService.SaveSettingAsync(securitySettings);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("GeneralCommon");
   }

   [HttpPost]
   public virtual async Task<IActionResult> UploadLocalePattern()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      try
      {
         _uploadService.UploadLocalePattern();
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.LocalePattern.SuccessUpload"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("GeneralCommon");
   }

   [HttpPost]
   public virtual async Task<IActionResult> UploadIcons(IFormFile iconsFile)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      try
      {
         if (iconsFile == null || iconsFile.Length == 0)
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
            return RedirectToAction("GeneralCommon");
         }

         //load settings for a chosen platform scope
         var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>();

         switch (_fileProvider.GetFileExtension(iconsFile.FileName))
         {
            case ".ico":
               _uploadService.UploadFavicon(iconsFile);
               commonSettings.FaviconAndAppIconsHeadCode = string.Format(HubCommonDefaults.SingleFaviconHeadLink, iconsFile.FileName);

               break;

            case ".zip":
               _uploadService.UploadIconsArchive(iconsFile);

               var headCodePath = _fileProvider.GetAbsolutePath(HubCommonDefaults.FaviconAndAppIconsPath, HubCommonDefaults.HeadCodeFileName);
               if (!_fileProvider.FileExists(headCodePath))
                  throw new Exception(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.MissingFile"), HubCommonDefaults.HeadCodeFileName));

               using (var sr = new StreamReader(headCodePath))
                  commonSettings.FaviconAndAppIconsHeadCode = await sr.ReadToEndAsync();

               break;

            default:
               throw new InvalidOperationException("File is not supported.");
         }

         await _settingService.SaveSettingAsync(commonSettings, x => x.FaviconAndAppIconsHeadCode, true);

         //delete old favicon icon if exist
         var oldFaviconIconPath = _fileProvider.GetAbsolutePath(string.Format(HubCommonDefaults.OldFaviconIconName));
         if (_fileProvider.FileExists(oldFaviconIconPath))
            _fileProvider.DeleteFile(oldFaviconIconPath);

         //activity log
         await _userActivityService.InsertActivityAsync("UploadIcons", string.Format(await _localizationService.GetResourceAsync("ActivityLog.UploadNewIcons")));
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.FaviconAndAppIcons.Uploaded"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("GeneralCommon");
   }

   #endregion

   #region All settings

   public virtual async Task<IActionResult> AllSettings(string settingName)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _settingModelFactory.PrepareSettingSearchModelAsync(new SettingSearchModel { SearchSettingName = WebUtility.HtmlEncode(settingName) });

      return View(model);
   }


   [HttpPost]
   public virtual async Task<IActionResult> AllSettings(SettingSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _settingModelFactory.PrepareSettingListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> SettingUpdate(SettingModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (model.Name != null)
         model.Name = model.Name.Trim();

      if (model.Value != null)
         model.Value = model.Value.Trim();

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      //try to get a setting with the specified id
      var setting = await _settingService.GetSettingByIdAsync(model.Id)
          ?? throw new ArgumentException("No setting found with the specified id");

      if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
         //setting name has been changed
         await _settingService.DeleteSettingAsync(setting);

      await _settingService.SetSettingAsync(model.Name, model.Value);

      //activity log
      await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"), setting);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> SettingAdd(SettingModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      if (model.Name != null)
         model.Name = model.Name.Trim();

      if (model.Value != null)
         model.Value = model.Value.Trim();

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      await _settingService.SetSettingAsync(model.Name, model.Value);

      //activity log
      await _userActivityService.InsertActivityAsync("AddNewSetting",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSetting"), model.Name),
          await _settingService.GetSettingAsync(model.Name));

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> SettingDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
         return AccessDeniedView();

      //try to get a setting with the specified id
      var setting = await _settingService.GetSettingByIdAsync(id)
          ?? throw new ArgumentException("No setting found with the specified id", nameof(id));

      await _settingService.DeleteSettingAsync(setting);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteSetting",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSetting"), setting.Name), setting);

      return new NullJsonResult();
   }

   #endregion

   #region Other settings

   //action displaying notification (warning) to a platform owner about a lot of traffic 
   //between the distributed cache server and the application when LoadAllLocaleRecordsOnStartup setting is set
   public async Task<IActionResult> DistributedCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
   {
      //LoadAllLocaleRecordsOnStartup is set and distributed cache is used, so display warning
      if (_appSettings.Get<DistributedCacheConfig>().Enabled && _appSettings.Get<DistributedCacheConfig>().DistributedCacheType != DistributedCacheType.Memory && loadAllLocaleRecordsOnStartup)
         return Json(new
         {
            Result = await _localizationService
                 .GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning")
         });

      return Json(new { Result = string.Empty });
   }

   //Action that displays a notification (warning) to the platform owner about the absence of active authentication providers
   public async Task<IActionResult> ForceMultifactorAuthenticationWarning(bool forceMultifactorAuthentication)
   {
      //ForceMultifactorAuthentication is set and the platform haven't active Authentication provider , so display warning
      if (forceMultifactorAuthentication && !await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
         return Json(new
         {
            Result = await _localizationService
                 .GetResourceAsync("Admin.Configuration.Settings.UserUser.ForceMultifactorAuthentication.Warning")
         });

      return Json(new { Result = string.Empty });
   }

   //Action that displays a notification (warning) to the platform owner about the need to restart the application after changing the setting
   public async Task<IActionResult> SeoFriendlyUrlsForLanguagesEnabledWarning(bool seoFriendlyUrlsForLanguagesEnabled)
   {
      //load settings for a chosen platform scope
      var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>();

      if (seoFriendlyUrlsForLanguagesEnabled != localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
         return Json(new
         {
            Result = await _localizationService
                 .GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled.Warning")
         });

      return Json(new { Result = string.Empty });
   }

   #endregion

   #endregion
}