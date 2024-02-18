using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Tasks
{
   /// <summary>
   /// Represents a schedule task list model
   /// </summary>
   public partial record ScheduleTaskListModel : BasePagedListModel<ScheduleTaskModel>
   {
   }
}