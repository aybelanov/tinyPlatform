using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a GDPR consent localized model
   /// </summary>
   public partial record GdprConsentLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.Message")]
      public string Message { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage")]
      public string RequiredMessage { get; set; }
   }
}
