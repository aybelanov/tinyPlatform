using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

public partial record DataConfigModel : BaseAppModel, IConfigModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Data.ConnectionString")]
   public string ConnectionString { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Data.DataProvider")]
   public int DataProvider { get; set; }
   public SelectList DataProviderTypeValues { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Data.SQLCommandTimeout")]
   [UIHint("Int32Nullable")]
   public int? SQLCommandTimeout { get; set; }

   #endregion
}
