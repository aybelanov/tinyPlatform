using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Directory
{
   /// <summary>
   /// Represents a currency list model
   /// </summary>
   public partial record CurrencyListModel : BasePagedListModel<CurrencyModel>
   {
   }
}