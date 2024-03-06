using Hub.Core.Domain.Directory;
using Hub.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Directory
{
   /// <summary>
   /// Exchange rate provider interface
   /// </summary>
   public partial interface IExchangeRateProvider : IPlugin
   {
      /// <summary>
      /// Gets currency live rates
      /// </summary>
      /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the exchange rates
      /// </returns>
      Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode);
   }
}