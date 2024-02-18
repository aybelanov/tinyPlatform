using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a display default footer item settings model
/// </summary>
public partial record DisplayDefaultFooterItemSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem")]
   public bool DisplaySitemapFooterItem { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem")]
   public bool DisplayContactUsFooterItem { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem")]
   public bool DisplayNewsFooterItem { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem")]
   public bool DisplayBlogFooterItem { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem")]
   public bool DisplayForumsFooterItem { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserInfoFooterItem")]
   public bool DisplayUserInfoFooterItem { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserAddressesFooterItem")]
   public bool DisplayUserAddressesFooterItem { get; set; }

   #endregion
}