using Hub.Core.Infrastructure;
using Hub.Web.Framework.Factories;
using Hub.Web.Infrastructure.Installation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Helpers;
using Hub.Web.Areas.OpenId.Factories;
using Hub.Core;
using Hub.Web.Services;
using Hub.Services.Clients;

namespace Hub.Web.Infrastructure;

public class AppliedServicesStartup : IAppStartup
{
   /// <summary>
   /// Add and configure any of the middleware
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   /// <param name="configuration">Configuration of the application</param>
   public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
   {
      //installation localization service
      services.AddScoped<IInstallationLocalizationService, InstallationLocalizationService>();

      //common factories
      services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
      services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();

      //admin factories
      services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
      services.AddScoped<IActivityLogModelFactory, ActivityLogModelFactory>();
      services.AddScoped<IAddressModelFactory, AddressModelFactory>();
      services.AddScoped<IAddressAttributeModelFactory, AddressAttributeModelFactory>();
      services.AddScoped<IBlogModelFactory, BlogModelFactory>();
      services.AddScoped<ICampaignModelFactory, CampaignModelFactory>();
      services.AddScoped<ICommonModelFactory, CommonModelFactory>();
      services.AddScoped<ICountryModelFactory, CountryModelFactory>();
      services.AddScoped<ICurrencyModelFactory, CurrencyModelFactory>();
      services.AddScoped<IUserAttributeModelFactory, UserAttributeModelFactory>();
      services.AddScoped<IUserModelFactory, UserModelFactory>();
      services.AddScoped<IUserRoleModelFactory, UserRoleModelFactory>();
      services.AddScoped<IEmailAccountModelFactory, EmailAccountModelFactory>();
      services.AddScoped<IExternalAuthenticationMethodModelFactory, ExternalAuthenticationMethodModelFactory>();
      services.AddScoped<IForumModelFactory, ForumModelFactory>();
      services.AddScoped<IHomeModelFactory, HomeModelFactory>();
      services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
      services.AddScoped<ILogModelFactory, LogModelFactory>();
      services.AddScoped<IMeasureModelFactory, MeasureModelFactory>();
      services.AddScoped<IMessageTemplateModelFactory, MessageTemplateModelFactory>();
      services.AddScoped<IMultiFactorAuthenticationMethodModelFactory, MultiFactorAuthenticationMethodModelFactory>();
      services.AddScoped<INewsletterSubscriptionModelFactory, NewsletterSubscriptionModelFactory>();
      services.AddScoped<INewsModelFactory, NewsModelFactory>();
      services.AddScoped<IPluginModelFactory, PluginModelFactory>();
      services.AddScoped<IPollModelFactory, PollModelFactory>();
      services.AddScoped<IReportModelFactory, ReportModelFactory>();
      services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
      services.AddScoped<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
      services.AddScoped<ISecurityModelFactory, SecurityModelFactory>();
      services.AddScoped<ISettingModelFactory, SettingModelFactory>();
      services.AddScoped<ITemplateModelFactory, TemplateModelFactory>();
      services.AddScoped<ITopicModelFactory, TopicModelFactory>();
      services.AddScoped<IWidgetModelFactory, WidgetModelFactory>();
      services.AddScoped<IDeviceModelFactory, DeviceModelFactory>();

      //openid factories
      services.AddScoped<IOpenIdModelFactory, OpenIdModelFactory>(); 

      //factories
      services.AddScoped<Factories.IAddressModelFactory, Factories.AddressModelFactory>();
      services.AddScoped<Factories.IBlogModelFactory, Factories.BlogModelFactory>();
      services.AddScoped<Factories.ICommonModelFactory, Factories.CommonModelFactory>();
      services.AddScoped<Factories.ICountryModelFactory, Factories.CountryModelFactory>();
      services.AddScoped<Factories.IUserModelFactory, Factories.UserModelFactory>();
      services.AddScoped<Factories.IForumModelFactory, Factories.ForumModelFactory>();
      services.AddScoped<Factories.IExternalAuthenticationModelFactory, Factories.ExternalAuthenticationModelFactory>();
      services.AddScoped<Factories.INewsModelFactory, Factories.NewsModelFactory>();
      services.AddScoped<Factories.INewsletterModelFactory, Factories.NewsletterModelFactory>();
      services.AddScoped<Factories.IPollModelFactory, Factories.PollModelFactory>();
      services.AddScoped<Factories.IPrivateMessagesModelFactory, Factories.PrivateMessagesModelFactory>();
      services.AddScoped<Factories.IProfileModelFactory, Factories.ProfileModelFactory>();
      services.AddScoped<Factories.ITopicModelFactory, Factories.TopicModelFactory>();
      services.AddScoped<Factories.IWidgetModelFactory, Factories.WidgetModelFactory>();

      //helpers classes
      services.AddScoped<ITinyMceHelper, TinyMceHelper>();

      services.AddSingleton<ICommunicator, HubCommunicator>();
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
   public int Order => 2001;
}
