using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Users;
using Hub.Services.Plugins;
using Hub.Services.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Directory;

/// <summary>
/// Represents an exchange rate plugin manager implementation
/// </summary>
public partial class ExchangeRatePluginManager : PluginManager<IExchangeRateProvider>, IExchangeRatePluginManager
{
   #region Fields

   private readonly CurrencySettings _currencySettings;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ExchangeRatePluginManager(CurrencySettings currencySettings,
         IUserService userService,
         IPluginService pluginService) : base(userService, pluginService)
   {
      _currencySettings = currencySettings;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Load primary active exchange rate provider
   /// </summary>
   /// <param name="user">Filter by user; pass null to load all plugins</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the exchange rate provider
   /// </returns>
   public virtual async Task<IExchangeRateProvider> LoadPrimaryPluginAsync(User user = null)
   {
      return await LoadPrimaryPluginAsync(_currencySettings.ActiveExchangeRateProviderSystemName, user);
   }

   /// <summary>
   /// Check whether the passed exchange rate provider is active
   /// </summary>
   /// <param name="exchangeRateProvider">Exchange rate provider to check</param>
   /// <returns>Result</returns>
   public virtual bool IsPluginActive(IExchangeRateProvider exchangeRateProvider)
   {
      return IsPluginActive(exchangeRateProvider, new List<string> { _currencySettings.ActiveExchangeRateProviderSystemName });
   }

   #endregion
}