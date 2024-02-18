using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents an external authentication settings model
/// </summary>
public partial record ExternalAuthenticationSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToRemoveAssociations")]
   public bool AllowUsersToRemoveAssociations { get; set; }

   #endregion
}