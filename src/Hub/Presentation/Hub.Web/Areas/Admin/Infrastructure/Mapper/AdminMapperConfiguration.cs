using AutoMapper;
using AutoMapper.Internal;
using Hub.Core.Configuration;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Configuration;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Gdpr;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Polls;
using Hub.Core.Domain.ScheduleTasks;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Seo;
using Hub.Core.Domain.Topics;
using Hub.Core.Domain.Users;
using Hub.Core.Infrastructure.Mapper;
using Hub.Data.Configuration;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Cms;
using Hub.Services.Plugins;
using Hub.Web.Areas.Admin.Models.Blogs;
using Hub.Web.Areas.Admin.Models.Cms;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Areas.Admin.Models.ExternalAuthentication;
using Hub.Web.Areas.Admin.Models.Forums;
using Hub.Web.Areas.Admin.Models.Gdpr;
using Hub.Web.Areas.Admin.Models.Localization;
using Hub.Web.Areas.Admin.Models.Logging;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Areas.Admin.Models.MultiFactorAuthentication;
using Hub.Web.Areas.Admin.Models.News;
using Hub.Web.Areas.Admin.Models.Plugins;
using Hub.Web.Areas.Admin.Models.Polls;
using Hub.Web.Areas.Admin.Models.Settings;
using Hub.Web.Areas.Admin.Models.Tasks;
using Hub.Web.Areas.Admin.Models.Templates;
using Hub.Web.Areas.Admin.Models.Topics;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Configuration;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Infrastructure.Mapper
{
   /// <summary>
   /// AutoMapper configuration for admin area models
   /// </summary>
   public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
   {
      #region Ctor

      public AdminMapperConfiguration()
      {
         //create specific maps
         CreateConfigMaps();
         CreateAuthenticationMaps();
         CreateMultiFactorAuthenticationMaps();
         CreateBlogsMaps();
         CreateCmsMaps();
         CreateCommonMaps();
         CreateUsersMaps();
         CreateDevicesMap();
         CreateDirectoryMaps();
         CreateForumsMaps();
         CreateGdprMaps();
         CreateLocalizationMaps();
         CreateLoggingMaps();
         CreateMediaMaps();
         CreateMessagesMaps();
         CreateNewsMaps();
         CreatePluginsMaps();
         CreatePollsMaps();
         CreateSecurityMaps();
         CreateSeoMaps();
         CreateTasksMaps();
         CreateTopicsMaps();

         //add some generic mapping rules
         this.Internal().ForAllMaps((mapConfiguration, map) =>
         {
            //exclude Form and CustomProperties from mapping BaseAppModel
            if (typeof(BaseAppModel).IsAssignableFrom(mapConfiguration.DestinationType))
               //map.ForMember(nameof(BaseAppModel.Form), options => options.Ignore());
               map.ForMember(nameof(BaseAppModel.CustomProperties), options => options.Ignore());

            //exclude some properties from mapping configuration and models
            if (typeof(IConfig).IsAssignableFrom(mapConfiguration.DestinationType))
               map.ForMember(nameof(IConfig.Name), options => options.Ignore());

            //exclude Locales from mapping ILocalizedModel
            if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
               map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());


            //exclude some properties from mapping ACL supported entities and models
            if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
               map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
            if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
            {
               map.ForMember(nameof(IAclSupportedModel.AvailableUserRoles), options => options.Ignore());
               map.ForMember(nameof(IAclSupportedModel.SelectedUserRoleIds), options => options.Ignore());
            }

            //exclude some properties from mapping discount supported entities and models
            if (typeof(IDiscountSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
            {
               map.ForMember(nameof(IDiscountSupportedModel.AvailableDiscounts), options => options.Ignore());
               map.ForMember(nameof(IDiscountSupportedModel.SelectedDiscountIds), options => options.Ignore());
            }

            if (typeof(IPluginModel).IsAssignableFrom(mapConfiguration.DestinationType))
            {
               //exclude some properties from mapping plugin models
               map.ForMember(nameof(IPluginModel.ConfigurationUrl), options => options.Ignore());
               map.ForMember(nameof(IPluginModel.IsActive), options => options.Ignore());
               map.ForMember(nameof(IPluginModel.LogoUrl), options => options.Ignore());

               //define specific rules for mapping plugin models
               if (typeof(IPlugin).IsAssignableFrom(mapConfiguration.SourceType))
               {
                  map.ForMember(nameof(IPluginModel.DisplayOrder), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.DisplayOrder));
                  map.ForMember(nameof(IPluginModel.FriendlyName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.FriendlyName));
                  map.ForMember(nameof(IPluginModel.SystemName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.SystemName));
               }
            }
         });
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Create configuration maps 
      /// </summary>
      protected virtual void CreateConfigMaps()
      {
         CreateMap<CacheConfig, CacheConfigModel>();
         CreateMap<CacheConfigModel, CacheConfig>();

         CreateMap<HostingConfig, HostingConfigModel>();
         CreateMap<HostingConfigModel, HostingConfig>();

         CreateMap<ServerConfig, HostingConfigModel>();
         CreateMap<HostingConfigModel, ServerConfig>();

         CreateMap<SecurityConfig, SecurityConfigModel>();
         CreateMap<SecurityConfigModel, SecurityConfig>();

         CreateMap<DistributedCacheConfig, DistributedCacheConfigModel>()
             .ForMember(model => model.DistributedCacheTypeValues, options => options.Ignore());
         CreateMap<DistributedCacheConfigModel, DistributedCacheConfig>();

         CreateMap<AzureBlobConfig, AzureBlobConfigModel>();
         CreateMap<AzureBlobConfigModel, AzureBlobConfig>()
             .ForMember(entity => entity.Enabled, options => options.Ignore())
             .ForMember(entity => entity.DataProtectionKeysEncryptWithVault, options => options.Ignore());

         CreateMap<InstallationConfig, InstallationConfigModel>();
         CreateMap<InstallationConfigModel, InstallationConfig>();

         CreateMap<PluginConfig, PluginConfigModel>();
         CreateMap<PluginConfigModel, PluginConfig>();

         CreateMap<CommonConfig, CommonConfigModel>();
         CreateMap<CommonConfigModel, CommonConfig>();

         CreateMap<DataConfig, DataConfigModel>()
             .ForMember(model => model.DataProviderTypeValues, options => options.Ignore());
         CreateMap<DataConfigModel, DataConfig>();

         CreateMap<WebOptimizerConfig, WebOptimizerConfigModel>();
         CreateMap<WebOptimizerConfigModel, WebOptimizerConfig>()
             .ForMember(entity => entity.CdnUrl, options => options.Ignore())
             .ForMember(entity => entity.AllowEmptyBundle, options => options.Ignore())
             .ForMember(entity => entity.HttpsCompression, options => options.Ignore())
             .ForMember(entity => entity.EnableTagHelperBundling, options => options.Ignore())
             .ForMember(entity => entity.EnableCaching, options => options.Ignore())
             .ForMember(entity => entity.EnableMemoryCache, options => options.Ignore());
      }


      /// <summary>
      /// Create authentication maps 
      /// </summary>
      protected virtual void CreateAuthenticationMaps()
      {
         CreateMap<IExternalAuthenticationMethod, ExternalAuthenticationMethodModel>();
      }

      /// <summary>
      /// Create multi-factor authentication maps 
      /// </summary>
      protected virtual void CreateMultiFactorAuthenticationMaps()
      {
         CreateMap<IMultiFactorAuthenticationMethod, MultiFactorAuthenticationMethodModel>();
      }

      /// <summary>
      /// Create blogs maps 
      /// </summary>new
      protected virtual void CreateBlogsMaps()
      {
         CreateMap<BlogComment, BlogCommentModel>()
             .ForMember(model => model.BlogPostTitle, options => options.Ignore())
             .ForMember(model => model.Comment, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.UserInfo, options => options.Ignore());

         CreateMap<BlogCommentModel, BlogComment>()
             .ForMember(entity => entity.CommentText, options => options.Ignore())
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.BlogPostId, options => options.Ignore())
             .ForMember(entity => entity.UserId, options => options.Ignore());

         CreateMap<BlogPost, BlogPostModel>()
             .ForMember(model => model.ApprovedComments, options => options.Ignore())
             .ForMember(model => model.AvailableLanguages, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.LanguageName, options => options.Ignore())
             .ForMember(model => model.NotApprovedComments, options => options.Ignore())
             .ForMember(model => model.SeName, options => options.Ignore())
             .ForMember(model => model.InitialBlogTags, options => options.Ignore());
         CreateMap<BlogPostModel, BlogPost>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

         CreateMap<BlogSettings, BlogSettingsModel>();
         CreateMap<BlogSettingsModel, BlogSettings>();
      }

      /// <summary>
      /// Create CMS maps 
      /// </summary>
      protected virtual void CreateCmsMaps()
      {
         CreateMap<IWidgetPlugin, WidgetModel>()
             .ForMember(model => model.WidgetViewComponentArguments, options => options.Ignore())
             .ForMember(model => model.WidgetViewComponentName, options => options.Ignore());
      }

      /// <summary>
      /// Create GDPR maps 
      /// </summary>
      protected virtual void CreateGdprMaps()
      {
         CreateMap<GdprSettings, GdprSettingsModel>()
             .ForMember(model => model.GdprConsentSearchModel, options => options.Ignore());
         CreateMap<GdprSettingsModel, GdprSettings>();

         CreateMap<GdprConsent, GdprConsentModel>();
         CreateMap<GdprConsentModel, GdprConsent>();

         CreateMap<GdprLog, GdprLogModel>()
             .ForMember(model => model.UserInfo, options => options.Ignore())
             .ForMember(model => model.RequestType, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore());
      }

      /// <summary>
      /// Create common maps 
      /// </summary>
      protected virtual void CreateCommonMaps()
      {
         CreateMap<Address, AddressModel>()
             .ForMember(model => model.AddressHtml, options => options.Ignore())
             .ForMember(model => model.AvailableCountries, options => options.Ignore())
             .ForMember(model => model.AvailableStates, options => options.Ignore())
             .ForMember(model => model.CountryName, options => options.Ignore())
             .ForMember(model => model.CustomAddressAttributes, options => options.Ignore())
             .ForMember(model => model.FormattedCustomAddressAttributes, options => options.Ignore())
             .ForMember(model => model.StateProvinceName, options => options.Ignore())
             .ForMember(model => model.CityRequired, options => options.Ignore())
             .ForMember(model => model.CompanyRequired, options => options.Ignore())
             .ForMember(model => model.CountryRequired, options => options.Ignore())
             .ForMember(model => model.CountyRequired, options => options.Ignore())
             .ForMember(model => model.EmailRequired, options => options.Ignore())
             .ForMember(model => model.FaxRequired, options => options.Ignore())
             .ForMember(model => model.FirstNameRequired, options => options.Ignore())
             .ForMember(model => model.LastNameRequired, options => options.Ignore())
             .ForMember(model => model.PhoneRequired, options => options.Ignore())
             .ForMember(model => model.StateProvinceName, options => options.Ignore())
             .ForMember(model => model.StreetAddress2Required, options => options.Ignore())
             .ForMember(model => model.StreetAddressRequired, options => options.Ignore())
             .ForMember(model => model.ZipPostalCodeRequired, options => options.Ignore());
         CreateMap<AddressModel, Address>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.CustomAttributes, options => options.Ignore());

         CreateMap<AddressAttribute, AddressAttributeModel>()
             .ForMember(model => model.AddressAttributeValueSearchModel, options => options.Ignore())
             .ForMember(model => model.AttributeControlTypeName, options => options.Ignore());
         CreateMap<AddressAttributeModel, AddressAttribute>()
             .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

         CreateMap<AddressAttributeValue, AddressAttributeValueModel>();
         CreateMap<AddressAttributeValueModel, AddressAttributeValue>();

         CreateMap<AddressSettings, AddressSettingsModel>();
         CreateMap<AddressSettingsModel, AddressSettings>()
             .ForMember(settings => settings.PreselectCountryIfOnlyOne, options => options.Ignore());

         CreateMap<Setting, SettingModel>();
      }

      /// <summary>
      /// Create users maps 
      /// </summary>
      protected virtual void CreateUsersMaps()
      {
         CreateMap<UserAttribute, UserAttributeModel>()
             .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
             .ForMember(model => model.UserAttributeValueSearchModel, options => options.Ignore());
         CreateMap<UserAttributeModel, UserAttribute>()
             .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

         CreateMap<UserAttributeValue, UserAttributeValueModel>();
         CreateMap<UserAttributeValueModel, UserAttributeValue>();

         CreateMap<UserRole, UserRoleModel>();
         CreateMap<UserRoleModel, UserRole>();

         CreateMap<UserSettings, UserSettingsModel>();
         CreateMap<UserSettingsModel, UserSettings>()
             .ForMember(settings => settings.AvatarMaximumSizeBytes, options => options.Ignore())
             .ForMember(settings => settings.DeleteGuestTaskOlderThanMinutes, options => options.Ignore())
             .ForMember(settings => settings.HashedPasswordFormat, options => options.Ignore())
             .ForMember(settings => settings.BeenRecentlyMinutes, options => options.Ignore())
             .ForMember(settings => settings.SuffixDeletedUsers, options => options.Ignore())
             .ForMember(settings => settings.LastActivityMinutes, options => options.Ignore());

         CreateMap<MultiFactorAuthenticationSettings, MultiFactorAuthenticationSettingsModel>();
         CreateMap<MultiFactorAuthenticationSettingsModel, MultiFactorAuthenticationSettings>()
             .ForMember(settings => settings.ActiveAuthenticationMethodSystemNames, option => option.Ignore());

         CreateMap<ActivityLog, UserActivityLogModel>()
            .ForMember(model => model.CreatedOn, options => options.Ignore())
            .ForMember(model => model.ActivityLogTypeName, options => options.Ignore());

         CreateMap<User, UserModel>()
             .ForMember(model => model.Email, options => options.Ignore())
             .ForMember(model => model.FullName, options => options.Ignore())
             .ForMember(model => model.Company, options => options.Ignore())
             .ForMember(model => model.Phone, options => options.Ignore())
             .ForMember(model => model.ZipPostalCode, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.LastActivityDate, options => options.Ignore())
             .ForMember(model => model.UserRoleNames, options => options.Ignore())
             .ForMember(model => model.AvatarUrl, options => options.Ignore())
             .ForMember(model => model.UsernamesEnabled, options => options.Ignore())
             .ForMember(model => model.Password, options => options.Ignore())
             .ForMember(model => model.GenderEnabled, options => options.Ignore())
             .ForMember(model => model.Gender, options => options.Ignore())
             .ForMember(model => model.FirstNameEnabled, options => options.Ignore())
             .ForMember(model => model.FirstName, options => options.Ignore())
             .ForMember(model => model.LastNameEnabled, options => options.Ignore())
             .ForMember(model => model.LastName, options => options.Ignore())
             .ForMember(model => model.DateOfBirthEnabled, options => options.Ignore())
             .ForMember(model => model.DateOfBirth, options => options.Ignore())
             .ForMember(model => model.CompanyEnabled, options => options.Ignore())
             .ForMember(model => model.StreetAddressEnabled, options => options.Ignore())
             .ForMember(model => model.StreetAddress, options => options.Ignore())
             .ForMember(model => model.StreetAddress2Enabled, options => options.Ignore())
             .ForMember(model => model.StreetAddress2, options => options.Ignore())
             .ForMember(model => model.ZipPostalCodeEnabled, options => options.Ignore())
             .ForMember(model => model.CityEnabled, options => options.Ignore())
             .ForMember(model => model.City, options => options.Ignore())
             .ForMember(model => model.CountyEnabled, options => options.Ignore())
             .ForMember(model => model.County, options => options.Ignore())
             .ForMember(model => model.CountryEnabled, options => options.Ignore())
             .ForMember(model => model.CountryId, options => options.Ignore())
             .ForMember(model => model.AvailableCountries, options => options.Ignore())
             .ForMember(model => model.StateProvinceEnabled, options => options.Ignore())
             .ForMember(model => model.StateProvinceId, options => options.Ignore())
             .ForMember(model => model.AvailableStates, options => options.Ignore())
             .ForMember(model => model.PhoneEnabled, options => options.Ignore())
             .ForMember(model => model.FaxEnabled, options => options.Ignore())
             .ForMember(model => model.Fax, options => options.Ignore())
             .ForMember(model => model.UserAttributes, options => options.Ignore())
             .ForMember(model => model.AffiliateName, options => options.Ignore())
             .ForMember(model => model.TimeZoneId, options => options.Ignore())
             .ForMember(model => model.AllowUsersToSetTimeZone, options => options.Ignore())
             .ForMember(model => model.AvailableTimeZones, options => options.Ignore())
             .ForMember(model => model.LastVisitedPage, options => options.Ignore())
             .ForMember(model => model.SendEmail, options => options.Ignore())
             .ForMember(model => model.SendPm, options => options.Ignore())
             .ForMember(model => model.AllowSendingOfPrivateMessage, options => options.Ignore())
             .ForMember(model => model.AllowSendingOfWelcomeMessage, options => options.Ignore())
             .ForMember(model => model.AllowReSendingOfActivationMessage, options => options.Ignore())
             .ForMember(model => model.GdprEnabled, options => options.Ignore())
             .ForMember(model => model.MultiFactorAuthenticationProvider, options => options.Ignore())
             .ForMember(model => model.UserAssociatedExternalAuthRecordsSearchModel, options => options.Ignore())
             .ForMember(model => model.UserAddressSearchModel, options => options.Ignore())
             .ForMember(model => model.UserActivityLogSearchModel, options => options.Ignore());

         CreateMap<UserModel, User>()
             .ForMember(entity => entity.UserGuid, options => options.Ignore())
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.LastActivityUtc, options => options.Ignore())
             .ForMember(entity => entity.EmailToRevalidate, options => options.Ignore())
             .ForMember(entity => entity.RequireReLogin, options => options.Ignore())
             .ForMember(entity => entity.FailedLoginAttempts, options => options.Ignore())
             .ForMember(entity => entity.CannotLoginUntilDateUtc, options => options.Ignore())
             .ForMember(entity => entity.IsDeleted, options => options.Ignore())
             .ForMember(entity => entity.IsSystemAccount, options => options.Ignore())
             .ForMember(entity => entity.SystemName, options => options.Ignore())
             .ForMember(entity => entity.LastLoginUtc, options => options.Ignore())
             .ForMember(entity => entity.BillingAddressId, options => options.Ignore())
             .ForMember(entity => entity.ShippingAddressId, options => options.Ignore());

         CreateMap<User, OnlineUserModel>()
             .ForMember(model => model.LastActivityDate, options => options.Ignore())
             .ForMember(model => model.UserInfo, options => options.Ignore())
             .ForMember(model => model.LastIpAddress, options => options.Ignore())
             .ForMember(model => model.Location, options => options.Ignore())
             .ForMember(model => model.LastVisitedPage, options => options.Ignore());
      }

      /// <summary>
      /// Create device maps
      /// </summary>
      protected virtual void CreateDevicesMap()
      {
         CreateMap<Device, DeviceModel>();
         CreateMap<DeviceModel, Device>()
            .ForMember(dest => dest.CreatedOnUtc, options => options.Ignore())
            .ForMember(dest => dest.CannotLoginUntilDateUtc, options => options.Ignore())
            .ForMember(dest => dest.UpdatedOnUtc, options => options.Ignore())
            .ForMember(dest => dest.IsDeleted, options => options.Ignore())
            .ForMember(dest => dest.Guid, options => options.Ignore())
            .ForMember(dest => dest.Name, options => options.Ignore())
            .ForMember(dest => dest.Description, options => options.Ignore());

         CreateMap<DeviceSettings, DeviceSettingsModel>();
         CreateMap<DeviceSettingsModel, DeviceSettings>();

         CreateMap<ActivityLog, DeviceActivityLogModel>()
            .ForMember(model => model.CreatedOn, options => options.Ignore())
            .ForMember(model => model.ActivityLogTypeName, options => options.Ignore());
         ;
      }

      /// <summary>
      /// Create directory maps 
      /// </summary>
      protected virtual void CreateDirectoryMaps()
      {
         CreateMap<Country, CountryModel>()
             .ForMember(model => model.NumberOfStates, options => options.Ignore())
             .ForMember(model => model.StateProvinceSearchModel, options => options.Ignore());
         CreateMap<CountryModel, Country>();

         CreateMap<Currency, CurrencyModel>()
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.IsPrimaryExchangeRateCurrency, options => options.Ignore())
             .ForMember(model => model.IsPrimaryPlatformCurrency, options => options.Ignore());
         CreateMap<CurrencyModel, Currency>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.RoundingType, options => options.Ignore())
             .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

         CreateMap<MeasureDimension, MeasureDimensionModel>()
             .ForMember(model => model.IsPrimaryDimension, options => options.Ignore());
         CreateMap<MeasureDimensionModel, MeasureDimension>();

         CreateMap<MeasureWeight, MeasureWeightModel>()
             .ForMember(model => model.IsPrimaryWeight, options => options.Ignore());
         CreateMap<MeasureWeightModel, MeasureWeight>();

         CreateMap<StateProvince, StateProvinceModel>();
         CreateMap<StateProvinceModel, StateProvince>();
      }

      /// <summary>
      /// Create forums maps 
      /// </summary>
      protected virtual void CreateForumsMaps()
      {
         CreateMap<Forum, ForumModel>()
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.ForumGroups, options => options.Ignore());
         CreateMap<ForumModel, Forum>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.LastPostUserId, options => options.Ignore())
             .ForMember(entity => entity.LastPostId, options => options.Ignore())
             .ForMember(entity => entity.LastPostTime, options => options.Ignore())
             .ForMember(entity => entity.LastTopicId, options => options.Ignore())
             .ForMember(entity => entity.NumPosts, options => options.Ignore())
             .ForMember(entity => entity.NumTopics, options => options.Ignore())
             .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

         CreateMap<ForumGroup, ForumGroupModel>()
             .ForMember(model => model.CreatedOn, options => options.Ignore());
         CreateMap<ForumGroupModel, ForumGroup>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

         CreateMap<ForumSettings, ForumSettingsModel>()
             .ForMember(model => model.ForumEditorValues, options => options.Ignore());

         CreateMap<ForumSettingsModel, ForumSettings>()
             .ForMember(settings => settings.ForumSearchTermMinimumLength, options => options.Ignore())
             .ForMember(settings => settings.ForumSubscriptionsPageSize, options => options.Ignore())
             .ForMember(settings => settings.HomepageActiveDiscussionsTopicCount, options => options.Ignore())
             .ForMember(settings => settings.LatestUserPostsPageSize, options => options.Ignore())
             .ForMember(settings => settings.PMSubjectMaxLength, options => options.Ignore())
             .ForMember(settings => settings.PMTextMaxLength, options => options.Ignore())
             .ForMember(settings => settings.PostMaxLength, options => options.Ignore())
             .ForMember(settings => settings.PrivateMessagesPageSize, options => options.Ignore())
             .ForMember(settings => settings.StrippedTopicMaxLength, options => options.Ignore())
             .ForMember(settings => settings.TopicSubjectMaxLength, options => options.Ignore());
      }

      /// <summary>
      /// Create localization maps 
      /// </summary>
      protected virtual void CreateLocalizationMaps()
      {
         CreateMap<Language, LanguageModel>()
             .ForMember(model => model.AvailableCurrencies, options => options.Ignore())
             .ForMember(model => model.LocaleResourceSearchModel, options => options.Ignore());
         CreateMap<LanguageModel, Language>();

         CreateMap<LocaleResourceModel, LocaleStringResource>()
             .ForMember(entity => entity.LanguageId, options => options.Ignore());
      }

      /// <summary>
      /// Create logging maps 
      /// </summary>
      protected virtual void CreateLoggingMaps()
      {
         CreateMap<ActivityLog, ActivityLogModel>()
             .ForMember(model => model.ActivityLogTypeName, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.Subject, options => options.Ignore());
         CreateMap<ActivityLogModel, ActivityLog>()
             .ForMember(entity => entity.ActivityLogTypeId, options => options.Ignore())
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.EntityId, options => options.Ignore())
             .ForMember(entity => entity.EntityName, options => options.Ignore());

         CreateMap<ActivityLogType, ActivityLogTypeModel>();
         CreateMap<ActivityLogTypeModel, ActivityLogType>()
             .ForMember(entity => entity.SystemKeyword, options => options.Ignore());

         CreateMap<Log, LogModel>()
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.FullMessage, options => options.Ignore())
             .ForMember(model => model.Subject, options => options.Ignore());
         CreateMap<LogModel, Log>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.LogLevelId, options => options.Ignore());
      }

      /// <summary>
      /// Create media maps 
      /// </summary>
      protected virtual void CreateMediaMaps()
      {
         CreateMap<MediaSettings, MediaSettingsModel>()
             .ForMember(model => model.PicturesStoredIntoDatabase, options => options.Ignore());
         CreateMap<MediaSettingsModel, MediaSettings>()
             .ForMember(settings => settings.AutoCompleteSearchThumbPictureSize, options => options.Ignore())
             .ForMember(settings => settings.AzureCacheControlHeader, options => options.Ignore())
             .ForMember(settings => settings.UseAbsoluteImagePath, options => options.Ignore())
             .ForMember(settings => settings.ImageSquarePictureSize, options => options.Ignore());
      }

      /// <summary>
      /// Create messages maps 
      /// </summary>
      protected virtual void CreateMessagesMaps()
      {
         CreateMap<Campaign, CampaignModel>()
             .ForMember(model => model.AllowedTokens, options => options.Ignore())
             .ForMember(model => model.AvailableUserRoles, options => options.Ignore())
             .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
             .ForMember(model => model.EmailAccountId, options => options.Ignore())
             .ForMember(model => model.TestEmail, options => options.Ignore());
         CreateMap<CampaignModel, Campaign>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore());

         CreateMap<EmailAccount, EmailAccountModel>()
             .ForMember(model => model.IsDefaultEmailAccount, options => options.Ignore())
             .ForMember(model => model.Password, options => options.Ignore())
             .ForMember(model => model.SendTestEmailTo, options => options.Ignore());
         CreateMap<EmailAccountModel, EmailAccount>()
             .ForMember(entity => entity.Password, options => options.Ignore());

         CreateMap<MessageTemplate, MessageTemplateModel>()
             .ForMember(model => model.AllowedTokens, options => options.Ignore())
             .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
             .ForMember(model => model.HasAttachedDownload, options => options.Ignore())
             .ForMember(model => model.SendImmediately, options => options.Ignore());
         CreateMap<MessageTemplateModel, MessageTemplate>()
             .ForMember(entity => entity.DelayPeriod, options => options.Ignore());

         CreateMap<NewsLetterSubscription, NewsletterSubscriptionModel>()
             .ForMember(model => model.CreatedOn, options => options.Ignore());
         CreateMap<NewsletterSubscriptionModel, NewsLetterSubscription>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.NewsLetterSubscriptionGuid, options => options.Ignore());

         CreateMap<QueuedEmail, QueuedEmailModel>()
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
             .ForMember(model => model.EmailAccountName, options => options.Ignore())
             .ForMember(model => model.PriorityName, options => options.Ignore())
             .ForMember(model => model.SendImmediately, options => options.Ignore())
             .ForMember(model => model.SentOn, options => options.Ignore());
         CreateMap<QueuedEmailModel, QueuedEmail>()
             .ForMember(entity => entity.AttachmentFileName, options => options.Ignore())
             .ForMember(entity => entity.AttachmentFilePath, options => options.Ignore())
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore())
             .ForMember(entity => entity.EmailAccountId, options => options.Ignore())
             .ForMember(entity => entity.Priority, options => options.Ignore())
             .ForMember(entity => entity.PriorityId, options => options.Ignore())
             .ForMember(entity => entity.SentOnUtc, options => options.Ignore());
      }

      /// <summary>
      /// Create news maps 
      /// </summary>
      protected virtual void CreateNewsMaps()
      {
         CreateMap<NewsComment, NewsCommentModel>()
             .ForMember(model => model.UserInfo, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.CommentText, options => options.Ignore())
             .ForMember(model => model.NewsItemTitle, options => options.Ignore());
         CreateMap<NewsCommentModel, NewsComment>()
             .ForMember(entity => entity.CommentTitle, options => options.Ignore())
             .ForMember(entity => entity.CommentText, options => options.Ignore())
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
             .ForMember(entity => entity.NewsItemId, options => options.Ignore())
             .ForMember(entity => entity.UserId, options => options.Ignore());

         CreateMap<NewsItem, NewsItemModel>()
             .ForMember(model => model.ApprovedComments, options => options.Ignore())
             .ForMember(model => model.AvailableLanguages, options => options.Ignore())
             .ForMember(model => model.CreatedOn, options => options.Ignore())
             .ForMember(model => model.LanguageName, options => options.Ignore())
             .ForMember(model => model.NotApprovedComments, options => options.Ignore())
             .ForMember(model => model.SeName, options => options.Ignore());
         CreateMap<NewsItemModel, NewsItem>()
             .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

         CreateMap<NewsSettings, NewsSettingsModel>();
         CreateMap<NewsSettingsModel, NewsSettings>();
      }

      /// <summary>
      /// Create plugins maps 
      /// </summary>
      protected virtual void CreatePluginsMaps()
      {
         CreateMap<PluginDescriptor, PluginModel>()
             .ForMember(model => model.CanChangeEnabled, options => options.Ignore())
             .ForMember(model => model.IsEnabled, options => options.Ignore());
      }

      /// <summary>
      /// Create polls maps 
      /// </summary>
      protected virtual void CreatePollsMaps()
      {
         CreateMap<PollAnswer, PollAnswerModel>();
         CreateMap<PollAnswerModel, PollAnswer>();

         CreateMap<Poll, PollModel>()
             .ForMember(model => model.AvailableLanguages, options => options.Ignore())
             .ForMember(model => model.PollAnswerSearchModel, options => options.Ignore())
             .ForMember(model => model.LanguageName, options => options.Ignore());
         CreateMap<PollModel, Poll>();
      }

      /// <summary>
      /// Create security maps 
      /// </summary>
      protected virtual void CreateSecurityMaps()
      {
         CreateMap<CaptchaSettings, CaptchaSettingsModel>()
             .ForMember(model => model.CaptchaTypeValues, options => options.Ignore());
         CreateMap<CaptchaSettingsModel, CaptchaSettings>()
             .ForMember(settings => settings.AutomaticallyChooseLanguage, options => options.Ignore())
             .ForMember(settings => settings.ReCaptchaDefaultLanguage, options => options.Ignore())
             .ForMember(settings => settings.ReCaptchaRequestTimeout, options => options.Ignore())
             .ForMember(settings => settings.ReCaptchaTheme, options => options.Ignore())
             .ForMember(settings => settings.ReCaptchaApiUrl, options => options.Ignore());
      }

      /// <summary>
      /// Create SEO maps 
      /// </summary>
      protected virtual void CreateSeoMaps()
      {
         CreateMap<UrlRecord, UrlRecordModel>()
             .ForMember(model => model.DetailsUrl, options => options.Ignore())
             .ForMember(model => model.Language, options => options.Ignore())
             .ForMember(model => model.Name, options => options.Ignore());
         CreateMap<UrlRecordModel, UrlRecord>()
             .ForMember(entity => entity.LanguageId, options => options.Ignore())
             .ForMember(entity => entity.Slug, options => options.Ignore());
      }

      /// <summary>
      /// Create tasks maps 
      /// </summary>
      protected virtual void CreateTasksMaps()
      {
         CreateMap<ScheduleTask, ScheduleTaskModel>();
         CreateMap<ScheduleTaskModel, ScheduleTask>()
             .ForMember(entity => entity.Type, options => options.Ignore())
             .ForMember(entity => entity.LastStartUtc, options => options.Ignore())
             .ForMember(entity => entity.LastEndUtc, options => options.Ignore())
             .ForMember(entity => entity.LastSuccessUtc, options => options.Ignore())
             .ForMember(entity => entity.LastEnabledUtc, options => options.Ignore());
      }

      /// <summary>
      /// Create topics maps 
      /// </summary>
      protected virtual void CreateTopicsMaps()
      {
         CreateMap<Topic, TopicModel>()
             .ForMember(model => model.AvailableTopicTemplates, options => options.Ignore())
             .ForMember(model => model.SeName, options => options.Ignore())
             .ForMember(model => model.TopicName, options => options.Ignore())
             .ForMember(model => model.Url, options => options.Ignore());
         CreateMap<TopicModel, Topic>();

         CreateMap<TopicTemplate, TopicTemplateModel>();
         CreateMap<TopicTemplateModel, TopicTemplate>();
      }

      #endregion

      #region Properties

      /// <summary>
      /// Order of this mapper implementation
      /// </summary>
      public int Order => 0;

      #endregion
   }
}