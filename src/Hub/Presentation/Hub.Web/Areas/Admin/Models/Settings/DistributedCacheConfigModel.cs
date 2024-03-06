using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents distributed cache configuration model
   /// </summary>
   public partial record DistributedCacheConfigModel : BaseAppModel, IConfigModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.DistributedCacheType")]
      public SelectList DistributedCacheTypeValues { get; set; }
      public int DistributedCacheType { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.Enabled")]
      public bool Enabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.ConnectionString")]
      public string ConnectionString { get; set; }

      [AppResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.SchemaName")]
      public string SchemaName { get; set; } = "dbo";

      [AppResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.TableName")]
      public string TableName { get; set; } = "DistributedCache";

      #endregion
   }
}