using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents a queued email list model
   /// </summary>
   public partial record QueuedEmailListModel : BasePagedListModel<QueuedEmailModel>
   {
   }
}