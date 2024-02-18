using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a multi-factor authentication settings model
/// </summary>
public partial record MultiFactorAuthenticationSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.ForceMultifactorAuthentication")]
   public bool ForceMultifactorAuthentication { get; set; }

   #endregion
}
