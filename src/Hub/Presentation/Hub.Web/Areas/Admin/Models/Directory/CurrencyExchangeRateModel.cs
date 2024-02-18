using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency exchange rate model
/// </summary>
public partial record CurrencyExchangeRateModel : BaseAppModel
{
   #region Properties

   public string CurrencyCode { get; set; }

   public decimal Rate { get; set; }

   #endregion
}