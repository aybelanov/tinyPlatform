using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a cache configuration model
/// </summary>
public partial record CacheConfigModel : BaseAppModel, IConfigModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Cache.DefaultCacheTime")]
   public int DefaultCacheTime { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Cache.ShortTermCacheTime")]
   public int ShortTermCacheTime { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Cache.BundledFilesCacheTime")]
   public int BundledFilesCacheTime { get; set; }

   #endregion
}