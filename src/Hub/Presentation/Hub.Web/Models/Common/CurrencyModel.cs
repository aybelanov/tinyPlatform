using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record CurrencyModel : BaseAppEntityModel
{
   public string Name { get; set; }

   public string CurrencySymbol { get; set; }
}