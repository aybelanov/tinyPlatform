using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a security settings model
   /// </summary>
   public partial record SecuritySettingsModel : BaseAppModel, ISettingsModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
      public string EncryptionKey { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
      public string AdminAreaAllowedIpAddresses { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
      public bool HoneypotEnabled { get; set; }

      #endregion
   }
}