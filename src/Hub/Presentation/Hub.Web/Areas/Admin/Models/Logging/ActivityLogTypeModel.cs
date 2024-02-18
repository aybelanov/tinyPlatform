using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents an activity log type model
/// </summary>
public partial record ActivityLogTypeModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Users.ActivityLogType.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLogType.Fields.Enabled")]
   public bool Enabled { get; set; }

   #endregion
}