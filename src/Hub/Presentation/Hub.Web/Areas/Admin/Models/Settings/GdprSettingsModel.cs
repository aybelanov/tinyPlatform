using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a GDPR settings model
/// </summary>
public partial record GdprSettingsModel : BaseAppModel, ISettingsModel
{
   #region Ctor

   public GdprSettingsModel()
   {
      GdprConsentSearchModel = new GdprConsentSearchModel();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.GdprEnabled")]
   public bool GdprEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogPrivacyPolicyConsent")]
   public bool LogPrivacyPolicyConsent { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogNewsletterConsent")]
   public bool LogNewsletterConsent { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogUserProfileChanges")]
   public bool LogUserProfileChanges { get; set; }

   public GdprConsentSearchModel GdprConsentSearchModel { get; set; }

   #endregion
}