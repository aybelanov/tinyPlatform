using Hub.Core.Configuration;

namespace Hub.Core.Domain.Directory;

/// <summary>
/// Currency settings
/// </summary>
public class CurrencySettings : ISettings
{
   /// <summary>
   /// Display the currency selector
   /// </summary>
   public bool DisplayCurrencySelector { get; set; }

   /// <summary>
   /// A value indicating whether to display currency labels
   /// </summary>
   public bool DisplayCurrencyLabel { get; set; }

   /// <summary>
   /// Primary platform currency identifier
   /// </summary>
   public long PrimaryPlatformCurrencyId { get; set; }

   /// <summary>
   ///  Primary exchange rate currency identifier
   /// </summary>
   public long PrimaryExchangeRateCurrencyId { get; set; }

   /// <summary>
   /// Active exchange rate provider system name (of a plugin)
   /// </summary>
   public string ActiveExchangeRateProviderSystemName { get; set; }

   /// <summary>
   /// A value indicating whether to enable automatic currency rate updates
   /// </summary>
   public bool AutoUpdateEnabled { get; set; }
}