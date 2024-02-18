using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users
{
   /// <summary>
   /// Represents a user attribute value list model
   /// </summary>
   public partial record UserAttributeValueListModel : BasePagedListModel<UserAttributeValueModel>
   {
   }
}