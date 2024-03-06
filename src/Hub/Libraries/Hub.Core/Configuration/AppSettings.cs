﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hub.Core.Configuration;

/// <summary>
/// Represents the app settings
/// </summary>
public partial class AppSettings
{
   #region Fields

   private readonly Dictionary<Type, IConfig> _configurations = new();

   #endregion

   #region Ctor

   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="configurations"></param>
   public AppSettings(IList<IConfig> configurations = null)
   {
      _configurations = configurations
          ?.OrderBy(config => config.GetOrder())
          ?.ToDictionary(config => config.GetType(), config => config)
          ?? new Dictionary<Type, IConfig>();
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets or sets raw configuration parameters
   /// </summary>
   [JsonExtensionData]
   public Dictionary<string, JToken> Configuration { get; set; }

   #endregion

   #region Methods

   /// <summary>
   /// Get configuration parameters by type
   /// </summary>
   /// <typeparam name="TConfig">Configuration type</typeparam>
   /// <returns>Configuration parameters</returns>
   public TConfig Get<TConfig>() where TConfig : class, IConfig
   {
      if (_configurations[typeof(TConfig)] is not TConfig config)
         throw new AppException($"No configuration with type '{typeof(TConfig)}' found");

      return config;
   }

   /// <summary>
   /// Update app settings
   /// </summary>
   /// <param name="configurations">Configurations to update</param>
   public void Update(IList<IConfig> configurations)
   {
      foreach (var config in configurations)
      {
         _configurations[config.GetType()] = config;
      }
   }

   #endregion
}