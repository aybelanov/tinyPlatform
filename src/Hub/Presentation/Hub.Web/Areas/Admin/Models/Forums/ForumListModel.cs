using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Forums
{
   /// <summary>
   /// Represents a forum list model
   /// </summary>
   public partial record ForumListModel : BasePagedListModel<ForumModel>
   {
   }
}