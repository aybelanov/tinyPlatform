using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Logging
{
   /// <summary>
   /// Represents a log list model
   /// </summary>
   public partial record LogListModel : BasePagedListModel<LogModel>
   {
   }
}