using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Services.Plugins;

namespace Hub.Services.Directory
{
   /// <summary>
   /// Represents an exchange rate plugin manager
   /// </summary>
   public partial interface IExchangeRatePluginManager : IPluginManager<IExchangeRateProvider>
   {
      /// <summary>
      /// Load primary active exchange rate provider
      /// </summary>
      /// <param name="user">Filter by user; pass null to load all plugins</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the exchange rate provider
      /// </returns>
      Task<IExchangeRateProvider> LoadPrimaryPluginAsync(User user = null);

      /// <summary>
      /// Check whether the passed exchange rate provider is active
      /// </summary>
      /// <param name="exchangeRateProvider">Exchange rate provider to check</param>
      /// <returns>Result</returns>
      bool IsPluginActive(IExchangeRateProvider exchangeRateProvider);
   }
}