using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users
{
   /// <summary>
   /// Represents a user list model
   /// </summary>
   public partial record UserListModel : BasePagedListModel<UserModel>
   {
   }
}