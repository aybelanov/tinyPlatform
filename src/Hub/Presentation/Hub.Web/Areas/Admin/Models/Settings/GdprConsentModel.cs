using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a GDPR consent model
/// </summary>
public partial record GdprConsentModel : BaseAppEntityModel, ILocalizedModel<GdprConsentLocalizedModel>
{
   #region Ctor

   public GdprConsentModel()
   {
      Locales = new List<GdprConsentLocalizedModel>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.Message")]
   public string Message { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.IsRequired")]
   public bool IsRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage")]
   public string RequiredMessage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayDuringRegistration")]
   public bool DisplayDuringRegistration { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayOnUserInfoPage")]
   public bool DisplayOnUserInfoPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayOrder")]
   public int DisplayOrder { get; set; }

   public IList<GdprConsentLocalizedModel> Locales { get; set; }

   #endregion
}