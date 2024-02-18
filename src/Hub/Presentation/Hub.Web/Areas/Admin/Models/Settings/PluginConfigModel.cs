using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a plugin configuration model
   /// </summary>
   public partial record PluginConfigModel : BaseAppModel, IConfigModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup")]
      public bool ClearPluginShadowDirectoryOnStartup { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup")]
      public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.Plugin.UseUnsafeLoadAssembly")]
      public bool UseUnsafeLoadAssembly { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy")]
      public bool UsePluginsShadowCopy { get; set; }

      #endregion
   }
}