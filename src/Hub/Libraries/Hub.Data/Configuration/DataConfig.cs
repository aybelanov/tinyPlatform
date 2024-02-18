﻿using Hub.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hub.Data.Configuration;

/// <summary>
/// Data configuration
/// </summary>
public partial class DataConfig : IConfig
{
   /// <summary>
   /// Gets or sets a connection string
   /// </summary>
   public string ConnectionString { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets a data provider
   /// </summary>
   [JsonConverter(typeof(StringEnumConverter))]
   public DataProviderType DataProvider { get; set; } = DataProviderType.SqlServer;

   /// <summary>
   /// Gets or sets the wait time (in seconds) before terminating the attempt to execute a command and generating an error.
   /// By default, timeout isn't set and a default value for the current provider used. 
   /// GetTable 0 to use infinite timeout.
   /// </summary>
   public int? SQLCommandTimeout { get; set; } = null;

   /// <summary>
   /// Gets a section name to load configuration
   /// </summary>
   [JsonIgnore]
   public string Name => "ConnectionStrings";

   /// <summary>
   /// Gets an order of configuration
   /// </summary>
   /// <returns>Order</returns>
   public int GetOrder() => 0; //display first
}