using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Reports
{
   /// <summary>
   /// Represents a registered users report list model
   /// </summary>
   public partial record RegisteredUsersReportListModel : BasePagedListModel<RegisteredUsersReportModel>
   {
   }
}