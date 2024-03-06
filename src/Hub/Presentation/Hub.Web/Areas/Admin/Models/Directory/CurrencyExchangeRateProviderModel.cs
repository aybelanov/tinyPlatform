using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency exchange rate provider model
/// </summary>
public partial record CurrencyExchangeRateProviderModel : BaseAppModel
{
   #region Ctor

   public CurrencyExchangeRateProviderModel()
   {
      ExchangeRates = new List<CurrencyExchangeRateModel>();
      ExchangeRateProviders = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyRateAutoUpdateEnabled")]
   public bool AutoUpdateEnabled { get; set; }

   public IList<CurrencyExchangeRateModel> ExchangeRates { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.ExchangeRateProvider")]
   public string ExchangeRateProvider { get; set; }
   public IList<SelectListItem> ExchangeRateProviders { get; set; }

   #endregion
}