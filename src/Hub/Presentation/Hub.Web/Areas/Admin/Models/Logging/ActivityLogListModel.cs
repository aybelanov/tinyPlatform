using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Logging
{
   /// <summary>
   /// Represents an activity log list model
   /// </summary>
   public partial record ActivityLogListModel : BasePagedListModel<ActivityLogModel>
   {
   }
}