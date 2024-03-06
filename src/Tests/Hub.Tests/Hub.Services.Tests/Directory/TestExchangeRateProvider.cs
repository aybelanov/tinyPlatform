using Hub.Core.Domain.Directory;
using Hub.Services.Directory;
using Hub.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Tests.Directory
{
   public class TestExchangeRateProvider : BasePlugin, IExchangeRateProvider
   {
      /// <summary>
      /// Gets currency live rates
      /// </summary>
      /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
      /// <returns>Exchange rates</returns>
      public Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode)
      {
         return Task.FromResult<IList<ExchangeRate>>(new List<ExchangeRate>());
      }
   }
}
