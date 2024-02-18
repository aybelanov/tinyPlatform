using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users
{
   /// <summary>
   /// Represents an online user list model
   /// </summary>
   public partial record OnlineUserListModel : BasePagedListModel<OnlineUserModel>
   {
   }
}