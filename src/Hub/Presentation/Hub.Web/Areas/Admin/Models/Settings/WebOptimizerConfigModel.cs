using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents WebOptimizer config model
   /// </summary>
   public partial record WebOptimizerConfigModel : BaseAppModel, IConfigModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableJavaScriptBundling")]
      public bool EnableJavaScriptBundling { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableCssBundling")]
      public bool EnableCssBundling { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableDiskCache")]
      public bool EnableDiskCache { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CacheDirectory")]
      public string CacheDirectory { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.JavaScriptBundleSuffix")]
      public string JavaScriptBundleSuffix { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CssBundleSuffix")]
      public string CssBundleSuffix { get; set; }

      #endregion
   }
}
