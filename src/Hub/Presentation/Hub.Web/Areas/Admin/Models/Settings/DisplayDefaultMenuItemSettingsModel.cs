using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a display default menu item settings model
   /// </summary>
   public partial record DisplayDefaultMenuItemSettingsModel : BaseAppModel, ISettingsModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem")]
      public bool DisplayHomepageMenuItem { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplaySearchMenuItem")]
      public bool DisplaySearchMenuItem { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem")]
      public bool DisplayUserInfoMenuItem { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem")]
      public bool DisplayBlogMenuItem { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem")]
      public bool DisplayForumsMenuItem { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
      public bool DisplayContactUsMenuItem { get; set; }

      #endregion
   }
}