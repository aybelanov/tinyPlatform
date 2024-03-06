using Hub.Core.Domain.Gdpr;
using Hub.Web.Areas.Admin.Models.Settings;
using System.Threading.Tasks;


namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the setting model factory
/// </summary>
public partial interface ISettingModelFactory
{
   /// <summary>
   /// Prepare app settings model
   /// </summary>
   /// <param name="model">AppSettings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the app settings model
   /// </returns>
   Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null);

   /// <summary>
   /// Prepare blog settings model
   /// </summary>
   /// <param name="model">Blog settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the blog settings model
   /// </returns>
   Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null);

   /// <summary>
   /// Prepare forum settings model
   /// </summary>
   /// <param name="model">Forum settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the forum settings model
   /// </returns>
   Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null);

   /// <summary>
   /// Prepare news settings model
   /// </summary>
   /// <param name="model">News settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the news settings model
   /// </returns>
   Task<NewsSettingsModel> PrepareNewsSettingsModelAsync(NewsSettingsModel model = null);

   /// <summary>
   /// Prepare media settings model
   /// </summary>
   /// <param name="model">Media settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the media settings model
   /// </returns>
   Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null);

   /// <summary>
   /// Prepare user's user settings model
   /// </summary>
   /// <param name="model">User user settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user's user settings model
   /// </returns>
   Task<UserUserSettingsModel> PrepareAllUserSettingsModelAsync(UserUserSettingsModel model = null);

   /// <summary>
   /// Prepare device settings model
   /// </summary>
   /// <param name="model">Device settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device settings model
   /// </returns>
   Task<DeviceSettingsModel> PrepareDeviceSettingsModelAsync(DeviceSettingsModel model = null);

   /// <summary>
   /// Prepare GDPR settings model
   /// </summary>
   /// <param name="model">Gdpr settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR settings model
   /// </returns>
   Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null);

   /// <summary>
   /// Prepare paged GDPR consent list model
   /// </summary>
   /// <param name="searchModel">GDPR search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR consent list model
   /// </returns>
   Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel);

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
   Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false);

   /// <summary>
   /// Prepare general and common settings model
   /// </summary>
   /// <param name="model">General common settings model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the general and common settings model
   /// </returns>
   Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null);

   /// <summary>
   /// Prepare setting search model
   /// </summary>
   /// <param name="searchModel">Setting search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting search model
   /// </returns>
   Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel);

   /// <summary>
   /// Prepare paged setting list model
   /// </summary>
   /// <param name="searchModel">Setting search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting list model
   /// </returns>
   Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel);

   /// <summary>
   /// Prepare setting mode model
   /// </summary>
   /// <param name="modeName">Mode name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting mode model
   /// </returns>
   Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName);
}