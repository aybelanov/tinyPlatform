using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Gdpr;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Seo;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Data.Configuration;
using Hub.Data.Extensions;
using Hub.Services;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Directory;
using Hub.Services.Gdpr;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.Themes;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Settings;
using Hub.Web.Framework.Configuration;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the setting model factory implementation
/// </summary>
public partial class SettingModelFactory : ISettingModelFactory
{
   #region Fields

   private readonly AppSettings _appSettings;
   private readonly CurrencySettings _currencySettings;
   private readonly IAddressModelFactory _addressModelFactory;
   private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
   private readonly IAddressService _addressService;
   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly ICurrencyService _currencyService;
   private readonly IUserAttributeModelFactory _userAttributeModelFactory;
   private readonly AppDbContext _dataProvider;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly ILocalizedModelFactory _localizedModelFactory;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly ILocalizationService _localizationService;
   private readonly IPictureService _pictureService;
   private readonly ISettingService _settingService;
   private readonly IThemeProvider _themeProvider;
   private readonly IWorkContext _workContext;
   private readonly IGdprService _gdprService;

   #endregion

   #region Ctor

   public SettingModelFactory(AppSettings appSettings,
       CurrencySettings currencySettings,
       IAddressModelFactory addressModelFactory,
       IAddressAttributeModelFactory addressAttributeModelFactory,
       IAddressService addressService,
       IBaseAdminModelFactory baseAdminModelFactory,
       ICurrencyService currencyService,
       IUserAttributeModelFactory userAttributeModelFactory,
       AppDbContext dataProvider,
       IDateTimeHelper dateTimeHelper,
      IGdprService gdprService,
      ILocalizedModelFactory localizedModelFactory,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       IPictureService pictureService,
       ISettingService settingService,
       IThemeProvider themeProvider,
       IWorkContext workContext)
   {
      _appSettings = appSettings;
      _currencySettings = currencySettings;
      _addressModelFactory = addressModelFactory;
      _addressAttributeModelFactory = addressAttributeModelFactory;
      _addressService = addressService;
      _baseAdminModelFactory = baseAdminModelFactory;
      _currencyService = currencyService;
      _userAttributeModelFactory = userAttributeModelFactory;
      _dataProvider = dataProvider;
      _dateTimeHelper = dateTimeHelper;
      _localizedModelFactory = localizedModelFactory;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _pictureService = pictureService;
      _settingService = settingService;
      _themeProvider = themeProvider;
      _workContext = workContext;
      _gdprService = gdprService;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare platform theme models
   /// </summary>
   /// <param name="models">List of platform theme models</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareHubThemeModelsAsync(IList<AppInformationSettingsModel.ThemeModel> models)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //load settings
      var appInformationSettings = await _settingService.LoadSettingAsync<AppInfoSettings>();

      //get available themes
      var availableThemes = await _themeProvider.GetThemesAsync();
      foreach (var theme in availableThemes)
         models.Add(new AppInformationSettingsModel.ThemeModel
         {
            FriendlyName = theme.FriendlyName,
            SystemName = theme.SystemName,
            PreviewImageUrl = theme.PreviewImageUrl,
            PreviewText = theme.PreviewText,
            SupportRtl = theme.SupportRtl,
            Selected = theme.SystemName.Equals(appInformationSettings.DefaultAppTheme, StringComparison.InvariantCultureIgnoreCase)
         });
   }

   /// <summary>
   /// Prepare sort option search model
   /// </summary>
   /// <param name="searchModel">Sort option search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the sort option search model
   /// </returns>
   protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare GDPR consent search model
   /// </summary>
   /// <param name="searchModel">GDPR consent search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR consent search model
   /// </returns>
   protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModelAsync(GdprConsentSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare address settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the address settings model
   /// </returns>
   protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
   {
      //load settings 
      var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>();

      //fill in model values from the entity
      var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

      return model;
   }

   /// <summary>
   /// Prepare user settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user settings model
   /// </returns>
   protected virtual async Task<UserSettingsModel> PrepareUserSettingsModelAsync()
   {
      //load settings 
      var userSettings = await _settingService.LoadSettingAsync<UserSettings>();

      //fill in model values from the entity
      var model = userSettings.ToSettingsModel<UserSettingsModel>();

      return model;
   }

   /// <summary>
   /// Prepare multi-factor authentication settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the multiFactorAuthenticationSettingsModel
   /// </returns>
   protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
   {
      //load settings 
      var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>();

      //fill in model values from the entity
      var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();

      return model;

   }

   /// <summary>
   /// Prepare date time settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the date time settings model
   /// </returns>
   protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
   {
      //load settings 
      var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>();

      //fill in model values from the entity
      var model = new DateTimeSettingsModel
      {
         AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone
      };

      //fill in additional values (not existing in the entity)
      model.DefaultPlatformTimeZoneId = _dateTimeHelper.DefaultPlatformTimeZone.Id;

      //prepare available time zones
      await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

      return model;
   }

   /// <summary>
   /// Prepare external authentication settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the external authentication settings model
   /// </returns>
   protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModelAsync()
   {
      //load settings 
      var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>();

      //fill in model values from the entity
      var model = new ExternalAuthenticationSettingsModel
      {
         AllowUsersToRemoveAssociations = externalAuthenticationSettings.AllowUsersToRemoveAssociations
      };

      return model;
   }

   /// <summary>
   /// Prepare this IoT-service information settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the platfrom information settings model
   /// </returns>
   protected virtual async Task<AppInformationSettingsModel> PrepareServiceInformationSettingsModelAsync()
   {
      //load settings 
      var appInformationSettings = await _settingService.LoadSettingAsync<AppInfoSettings>();
      var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>();

      //fill in model values from the entity
      var model = new AppInformationSettingsModel
      {
         PlatformClosed = appInformationSettings.PlatformClosed,
         DefaultPlatformTheme = appInformationSettings.DefaultAppTheme,
         AllowUserToSelectTheme = appInformationSettings.AllowUserToSelectTheme,
         LogoPictureId = appInformationSettings.LogoPictureId,
         DisplayEuCookieLawWarning = appInformationSettings.DisplayEuCookieLawWarning,
         FacebookLink = appInformationSettings.FacebookLink,
         TwitterLink = appInformationSettings.TwitterLink,
         YoutubeLink = appInformationSettings.YoutubeLink,
         SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
         UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
         PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
      };

      //prepare available themes
      await PrepareHubThemeModelsAsync(model.AvailableApplicationThemes);

      return model;
   }

   /// <summary>
   /// Prepare Sitemap settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the sitemap settings model
   /// </returns>
   protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
   {
      //load settings 
      var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>();

      //fill in model values from the entity
      var model = new SitemapSettingsModel
      {
         SitemapEnabled = sitemapSettings.SitemapEnabled,
         SitemapPageSize = sitemapSettings.SitemapPageSize,
         SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
         SitemapIncludeNews = sitemapSettings.SitemapIncludeNews,
         SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
      };

      return model;
   }

   /// <summary>
   /// Prepare minification settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the minification settings model
   /// </returns>
   protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
   {
      //load settings 
      var minificationSettings = await _settingService.LoadSettingAsync<CommonSettings>();

      //fill in model values from the entity
      var model = new MinificationSettingsModel
      {
         EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
         UseResponseCompression = minificationSettings.UseResponseCompression
      };

      return model;
   }

   /// <summary>
   /// Prepare SEO settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the sEO settings model
   /// </returns>
   protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
   {
      //load settings 
      var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>();

      //fill in model values from the entity
      var model = new SeoSettingsModel
      {
         PageTitleSeparator = seoSettings.PageTitleSeparator,
         PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
         PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
         HomepageTitle = seoSettings.HomepageTitle,
         HomepageDescription = seoSettings.HomepageDescription,
         DefaultTitle = seoSettings.DefaultTitle,
         DefaultMetaKeywords = seoSettings.DefaultMetaKeywords,
         DefaultMetaDescription = seoSettings.DefaultMetaDescription,
         GenerateMetaDescription = seoSettings.GenerateMetaDescription,
         ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
         CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
         WwwRequirement = (int)seoSettings.WwwRequirement,
         WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

         TwitterMetaTags = seoSettings.TwitterMetaTags,
         OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
         CustomHeadTags = seoSettings.CustomHeadTags,
         MicrodataEnabled = seoSettings.MicrodataEnabled
      };

      return model;
   }

   /// <summary>
   /// Prepare security settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the security settings model
   /// </returns>
   protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
   {
      //load settings 
      var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>();

      //fill in model values from the entity
      var model = new SecuritySettingsModel
      {
         EncryptionKey = securitySettings.EncryptionKey,
         HoneypotEnabled = securitySettings.HoneypotEnabled
      };

      //fill in additional values (not existing in the entity)
      if (securitySettings.AdminAreaAllowedIpAddresses != null)
         model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

      return model;
   }

   /// <summary>
   /// Prepare captcha settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the captcha settings model
   /// </returns>
   protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
   {
      //load settings 
      var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>();

      //fill in model values from the entity
      var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

      model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

      return model;
   }

   /// <summary>
   /// Prepare PDF settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the pDF settings model
   /// </returns>
   protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
   {
      //load settings 
      var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>();

      //fill in model values from the entity
      var model = new PdfSettingsModel
      {
         LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
         LogoPictureId = pdfSettings.LogoPictureId,
         InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
         InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
      };

      return model;
   }

   /// <summary>
   /// Prepare localization settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the localization settings model
   /// </returns>
   protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
   {
      //load settings 
      var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>();

      //fill in model values from the entity
      var model = new LocalizationSettingsModel
      {
         UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
         SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
         AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
         LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
         LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
         LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
      };

      return model;
   }

   /// <summary>
   /// Prepare admin area settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the admin area settings model
   /// </returns>
   protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
   {
      //load settings 
      var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>();

      //fill in model values from the entity
      var model = new AdminAreaSettingsModel
      {
         UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates
      };

      return model;
   }

   /// <summary>
   /// Prepare display default menu item settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the display default menu item settings model
   /// </returns>
   protected virtual async Task<DisplayDefaultMenuItemSettingsModel> PrepareDisplayDefaultMenuItemSettingsModelAsync()
   {
      //load settings 
      var displayDefaultMenuItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultMenuItemSettings>();

      //fill in model values from the entity
      var model = new DisplayDefaultMenuItemSettingsModel
      {
         DisplayHomepageMenuItem = displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
         DisplaySearchMenuItem = displayDefaultMenuItemSettings.DisplaySearchMenuItem,
         DisplayUserInfoMenuItem = displayDefaultMenuItemSettings.DisplayUserInfoMenuItem,
         DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem,
         DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem,
         DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem
      };

      return model;
   }

   /// <summary>
   /// Prepare display default footer item settings model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the display default footer item settings model
   /// </returns>
   protected virtual async Task<DisplayDefaultFooterItemSettingsModel> PrepareDisplayDefaultFooterItemSettingsModelAsync()
   {
      //load settings 
      var displayDefaultFooterItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultFooterItemSettings>();

      //fill in model values from the entity
      var model = new DisplayDefaultFooterItemSettingsModel
      {
         DisplaySitemapFooterItem = displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
         DisplayContactUsFooterItem = displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
         DisplayUserInfoFooterItem = displayDefaultFooterItemSettings.DisplayUserInfoFooterItem,
         DisplayUserAddressesFooterItem = displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem,
         DisplayNewsFooterItem = displayDefaultFooterItemSettings.DisplayNewsFooterItem,
         DisplayBlogFooterItem = displayDefaultFooterItemSettings.DisplayBlogFooterItem,
         DisplayForumsFooterItem = displayDefaultFooterItemSettings.DisplayForumsFooterItem,
      };

      return model;
   }

   /// <summary>
   /// Prepare setting model to add
   /// </summary>
   /// <param name="model">Setting model to add</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual Task PrepareAddSettingModelAsync(SettingModel model)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      // TODO refactor
      return Task.CompletedTask;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare app settings model
   /// </summary>
   /// <param name="model">AppSettings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the app settings model
   /// </returns>
   public virtual async Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null)
   {
      model ??= new AppSettingsModel
      {
         CacheConfigModel = _appSettings.Get<CacheConfig>().ToConfigModel<CacheConfigModel>(),
         SecurityConfigModel = _appSettings.Get<SecurityConfig>().ToConfigModel<SecurityConfigModel>(),
         HostingConfigModel = _appSettings.Get<HostingConfig>().ToConfigModel<HostingConfigModel>(),
         DistributedCacheConfigModel = _appSettings.Get<DistributedCacheConfig>().ToConfigModel<DistributedCacheConfigModel>(),
         AzureBlobConfigModel = _appSettings.Get<AzureBlobConfig>().ToConfigModel<AzureBlobConfigModel>(),
         InstallationConfigModel = _appSettings.Get<InstallationConfig>().ToConfigModel<InstallationConfigModel>(),
         PluginConfigModel = _appSettings.Get<PluginConfig>().ToConfigModel<PluginConfigModel>(),
         CommonConfigModel = _appSettings.Get<CommonConfig>().ToConfigModel<CommonConfigModel>(),
         DataConfigModel = _appSettings.Get<DataConfig>().ToConfigModel<DataConfigModel>(),
         WebOptimizerConfigModel = _appSettings.Get<WebOptimizerConfig>().ToConfigModel<WebOptimizerConfigModel>(),
      };

      // it's a toplevel (doesn't have an own section) config
      model.HostingConfigModel = _appSettings.Get<ServerConfig>().ToConfigModel(model.HostingConfigModel);

      model.DistributedCacheConfigModel.DistributedCacheTypeValues = await _appSettings.Get<DistributedCacheConfig>().DistributedCacheType.ToSelectListAsync();

      model.DataConfigModel.DataProviderTypeValues = await _appSettings.Get<DataConfig>().DataProvider.ToSelectListAsync();

      //Since we decided to use the naming of the DB connections section as in the .net core - "ConnectionStrings",
      //we are forced to adjust our internal model naming to this convention in this check.
      model.EnvironmentVariables.AddRange(from property in model.GetType().GetProperties()
                                          where property.Name != nameof(AppSettingsModel.EnvironmentVariables)
                                          from pp in property.PropertyType.GetProperties()
                                          where Environment.GetEnvironmentVariables().Contains($"{property.Name.Replace("Model", "").Replace("DataConfig", "ConnectionStrings")}__{pp.Name}")
                                          select $"{property.Name}_{pp.Name}");
      return model;
   }

   /// <summary>
   /// Prepare blog settings model
   /// </summary>
   /// <param name="model">Blog settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the blog settings model
   /// </returns>
   public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null)
   {
      //load settings 
      var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>();

      //fill in model values from the entity
      model ??= blogSettings.ToSettingsModel<BlogSettingsModel>();

      return model;
   }

   /// <summary>
   /// Prepare forum settings model
   /// </summary>
   /// <param name="model">Forum settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum settings model
   /// </returns>
   public virtual async Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null)
   {
      //load settings 
      var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>();

      //fill in model values from the entity
      model ??= forumSettings.ToSettingsModel<ForumSettingsModel>();

      //fill in additional values (not existing in the entity)
      model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

      return model;
   }

   /// <summary>
   /// Prepare news settings model
   /// </summary>
   /// <param name="model">News settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news settings model
   /// </returns>
   public virtual async Task<NewsSettingsModel> PrepareNewsSettingsModelAsync(NewsSettingsModel model = null)
   {
      //load settings 
      var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>();

      //fill in model values from the entity
      model ??= newsSettings.ToSettingsModel<NewsSettingsModel>();

      return model;
   }

   /// <summary>
   /// Prepare media settings model
   /// </summary>
   /// <param name="model">Media settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the media settings model
   /// </returns>
   public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null)
   {
      //load settings 
      var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>();

      //fill in model values from the entity
      model ??= mediaSettings.ToSettingsModel<MediaSettingsModel>();

      //fill in additional values (not existing in the entity)
      model.PicturesStoredIntoDatabase = await _pictureService.IsStoreInDbAsync();

      return model;
   }

   /// <summary>
   /// Prepare user's user settings model
   /// </summary>
   /// <param name="model">User user settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user's user settings model
   /// </returns>
   public virtual async Task<UserUserSettingsModel> PrepareAllUserSettingsModelAsync(UserUserSettingsModel model = null)
   {
      model ??= new UserUserSettingsModel();

      //prepare user settings model
      model.UserSettings = await PrepareUserSettingsModelAsync();

      //prepare multi-factor authentication settings model
      model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

      //prepare address settings model
      model.AddressSettings = await PrepareAddressSettingsModelAsync();

      //prepare date time settings model
      model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

      //prepare external authentication settings model
      model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModelAsync();

      //prepare nested search models
      await _userAttributeModelFactory.PrepareUserAttributeSearchModelAsync(model.UserAttributeSearchModel);
      await _addressAttributeModelFactory.PrepareAddressAttributeSearchModelAsync(model.AddressAttributeSearchModel);

      return model;
   }

   /// <summary>
   /// Prepare device settings model
   /// </summary>
   /// <param name="model">Device settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device settings model
   /// </returns>
   public async Task<DeviceSettingsModel> PrepareDeviceSettingsModelAsync(DeviceSettingsModel model = null)
   {
      var deviceSettings = await _settingService.LoadSettingAsync<DeviceSettings>();

      model ??= deviceSettings.ToSettingsModel<DeviceSettingsModel>();

      return model;
   }

   /// <summary>
   /// Prepare GDPR settings model
   /// </summary>
   /// <param name="model">Gdpr settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR settings model
   /// </returns>
   public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null)
   {
      //load settings 
      var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>();

      //fill in model values from the entity
      model ??= gdprSettings.ToSettingsModel<GdprSettingsModel>();

      //prepare nested search model
      await PrepareGdprConsentSearchModelAsync(model.GdprConsentSearchModel);

      return model;
   }

   /// <summary>
   /// Prepare paged GDPR consent list model
   /// </summary>
   /// <param name="searchModel">GDPR search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR consent list model
   /// </returns>
   public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get sort options
      var consentList = (await _gdprService.GetAllConsentsAsync()).ToPagedList(searchModel);

      //prepare list model
      var model = await new GdprConsentListModel().PrepareToGridAsync(searchModel, consentList, () =>
      {
         return consentList.SelectAwait(async consent =>
            {
               var gdprConsentModel = consent.ToModel<GdprConsentModel>();

               var gdprConsent = await _gdprService.GetConsentByIdAsync(gdprConsentModel.Id);
               gdprConsentModel.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message);
               gdprConsentModel.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage);

               return gdprConsentModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare GDPR consent model
   /// </summary>
   /// <param name="model">GDPR consent model</param>
   /// <param name="gdprConsent">GDPR consent</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR consent model
   /// </returns>
   public virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
   {
      Func<GdprConsentLocalizedModel, long, Task> localizedModelConfiguration = null;

      //fill in model values from the entity
      if (gdprConsent != null)
      {
         model ??= gdprConsent.ToModel<GdprConsentModel>();

         //define localized model configuration action
         localizedModelConfiguration = async (locale, languageId) =>
         {
            locale.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message, languageId, false, false);
            locale.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
         };
      }

      //set default values for the new model
      if (gdprConsent == null)
         model.DisplayOrder = 1;

      //prepare localized models
      if (!excludeProperties)
         model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

      return model;
   }

   /// <summary>
   /// Prepare general and common settings model
   /// </summary>
   /// <param name="model">General common settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the general and common settings model
   /// </returns>
   public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null)
   {
      model ??= new GeneralCommonSettingsModel();

      //prepare platfrom information settings model
      model.PlatformInformationSettings = await PrepareServiceInformationSettingsModelAsync();

      //prepare Sitemap settings model
      model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

      //prepare Minification settings model
      model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

      //prepare SEO settings model
      model.SeoSettings = await PrepareSeoSettingsModelAsync();

      //prepare security settings model
      model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

      //prepare captcha settings model
      model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

      //prepare PDF settings model
      model.PdfSettings = await PreparePdfSettingsModelAsync();

      //prepare PDF settings model
      model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

      //prepare admin area settings model
      model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

      //prepare display default menu item settings model
      model.DisplayDefaultMenuItemSettings = await PrepareDisplayDefaultMenuItemSettingsModelAsync();

      //prepare display default footer item settings model
      model.DisplayDefaultFooterItemSettings = await PrepareDisplayDefaultFooterItemSettingsModelAsync();

      return model;
   }

   /// <summary>
   /// Prepare setting search model
   /// </summary>
   /// <param name="searchModel">Setting search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting search model
   /// </returns>
   public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare model to add
      //await PrepareAddSettingModelAsync(searchModel.AddSetting);

      //prepare page parameters
      searchModel.SetGridPageSize();

      return await Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged setting list model
   /// </summary>
   /// <param name="searchModel">Setting search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting list model
   /// </returns>
   public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get settings
      var settings = (await _settingService.GetAllSettingsAsync()).AsQueryable();

      //filter settings
      if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
         settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
      if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
         settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

      var pagedSettings = settings.ToList().ToPagedList(searchModel);

      //prepare list model
      var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
      {
         return pagedSettings.SelectAwait(async setting =>
            {
               //fill in model values from the entity
               var settingModel = setting.ToModel<SettingModel>();

               // TODO some logic

               return await Task.FromResult(settingModel);
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare setting mode model
   /// </summary>
   /// <param name="modeName">Mode name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting mode model
   /// </returns>
   public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
   {
      var model = new SettingModeModel
      {
         ModeName = modeName,
         Enabled = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), modeName, defaultValue: true)
      };

      return model;
   }

   #endregion
}