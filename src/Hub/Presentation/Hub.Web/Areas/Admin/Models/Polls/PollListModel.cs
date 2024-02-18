using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Polls
{
   /// <summary>
   /// Represents a poll list model
   /// </summary>
   public partial record PollListModel : BasePagedListModel<PollModel>
   {
   }
}