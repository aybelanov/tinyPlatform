using System.ComponentModel.DataAnnotations;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a common configuration model
/// </summary>
public partial record CommonConfigModel : BaseAppModel, IConfigModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.DisplayFullErrorStack")]
   public bool DisplayFullErrorStack { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.UserAgentStringsPath")]
   public string UserAgentStringsPath { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath")]
   public string CrawlerOnlyUserAgentStringsPath { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider")]
   public bool UseSessionStateTempDataProvider { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.MiniProfilerEnabled")]
   public bool MiniProfilerEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.ScheduleTaskRunTimeout")]
   [UIHint("Int32Nullable")]
   public int? ScheduleTaskRunTimeout { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.StaticFilesCacheControl")]
   public string StaticFilesCacheControl { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.SupportPreviousTinyPlatformVersions")]
   public bool SupportPreviousTinyPlatformVersions { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.PluginStaticFileExtensionsBlacklist")]
   public string PluginStaticFileExtensionsBlacklist { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Common.ServeUnknownFileTypes")]
   public bool ServeUnknownFileTypes { get; set; }

   #endregion
}