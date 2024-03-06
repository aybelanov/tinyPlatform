using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Tasks;

/// <summary>
/// Represents a schedule task model
/// </summary>
public partial record ScheduleTaskModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.System.ScheduleTasks.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.System.ScheduleTasks.Seconds")]
   public int Seconds { get; set; }

   [AppResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
   public bool Enabled { get; set; }

   [AppResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
   public bool StopOnError { get; set; }

   [AppResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
   public string LastStartUtc { get; set; }

   [AppResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
   public string LastEndUtc { get; set; }

   [AppResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
   public string LastSuccessUtc { get; set; }

   #endregion
}