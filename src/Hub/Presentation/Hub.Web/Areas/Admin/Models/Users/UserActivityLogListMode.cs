using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users
{
   /// <summary>
   /// Represents a user activity log list model
   /// </summary>
   public partial record UserActivityLogListModel : BasePagedListModel<UserActivityLogModel>
   {
   }
}