using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a Sitemap settings model
/// </summary>
public partial record SitemapSettingsModel : BaseAppModel, ISettingsModel
{

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapEnabled")]
   public bool SitemapEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeBlogPosts")]
   public bool SitemapIncludeBlogPosts { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews")]
   public bool SitemapIncludeNews { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTopics")]
   public bool SitemapIncludeTopics { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapPageSize")]
   public int SitemapPageSize { get; set; }
}
