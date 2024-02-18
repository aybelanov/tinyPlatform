using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents an email account list model
   /// </summary>
   public partial record EmailAccountListModel : BasePagedListModel<EmailAccountModel>
   {
   }
}