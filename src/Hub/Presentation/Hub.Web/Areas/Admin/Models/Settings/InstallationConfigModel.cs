using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents an installation configuration model
/// </summary>
public partial record InstallationConfigModel : BaseAppModel, IConfigModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Installation.DisableSampleData")]
   public bool DisableSampleData { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Installation.DisabledPlugins")]
   public string DisabledPlugins { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Installation.InstallRegionalResources")]
   public bool InstallRegionalResources { get; set; }

   #endregion
}