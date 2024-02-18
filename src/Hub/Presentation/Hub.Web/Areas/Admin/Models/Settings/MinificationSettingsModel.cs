using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a minification settings model
/// </summary>
public partial record MinificationSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification")]
   public bool EnableHtmlMinification { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseResponseCompression")]
   public bool UseResponseCompression { get; set; }

   #endregion
}
