using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record CurrencySelectorModel : BaseAppModel
{
   public CurrencySelectorModel()
   {
      AvailableCurrencies = new List<CurrencyModel>();
   }

   public IList<CurrencyModel> AvailableCurrencies { get; set; }

   public long CurrentCurrencyId { get; set; }
}