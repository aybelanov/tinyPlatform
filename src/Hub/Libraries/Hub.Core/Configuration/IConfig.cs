using Newtonsoft.Json;

namespace Hub.Core.Configuration;

/// <summary>
/// Represents a configuration from app settings
/// </summary>
public partial interface IConfig
{
   /// <summary>
   /// Gets a section name to load configuration
   /// </summary>
   [JsonIgnore]
   string Name => GetType().Name;

   /// <summary>
   /// Gets an order of configuration
   /// </summary>
   /// <returns>Order</returns>
   public int GetOrder() => 1;

   /// <summary>
   /// If set true a section with the class name will be created in the top level of the appSettings.json
   /// othervise all properties will be put into the the top level of the appSettings.json
   /// </summary>
   public bool HasOwnSection() => true;
}