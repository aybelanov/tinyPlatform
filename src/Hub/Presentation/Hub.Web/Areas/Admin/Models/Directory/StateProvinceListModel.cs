using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Directory
{
   /// <summary>
   /// Represents a state and province list model
   /// </summary>
   public record StateProvinceListModel : BasePagedListModel<StateProvinceModel>
   {
   }
}