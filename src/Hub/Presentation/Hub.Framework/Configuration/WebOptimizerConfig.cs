using System.Text.Json.Serialization;
using Hub.Core.Configuration;
using WebOptimizer;

namespace Hub.Web.Framework.Configuration;

/// <summary>
/// Represents WebOptimizer configuration
/// </summary>
public class WebOptimizerConfig : WebOptimizerOptions, IConfig
{
   #region Ctor

   /// <summary>
   /// Default Ctor
   /// </summary>
   public WebOptimizerConfig()
   {
      EnableDiskCache = true;
      EnableTagHelperBundling = false;
   }

   #endregion

   #region Properties

   /// <summary>
   /// A value indicating whether JS file bundling and minification is enabled
   /// </summary>
   public bool EnableJavaScriptBundling { get; private set; } = true;

   /// <summary>
   /// A value indicating whether CSS file bundling and minification is enabled
   /// </summary>
   public bool EnableCssBundling { get; private set; } = true;

   /// <summary>
   /// Gets or sets a suffix for the js-file name of generated bundles
   /// </summary>
   public string JavaScriptBundleSuffix { get; private set; } = ".scripts";

   /// <summary>
   /// Gets or sets a suffix for the css-file name of generated bundles
   /// </summary>
   public string CssBundleSuffix { get; private set; } = ".styles";

   /// <summary>
   /// Gets a section name to load configuration
   /// </summary>
   [JsonIgnore]
   public string Name => "WebOptimizer";

   /// <summary>
   /// Gets an order of configuration
   /// </summary>
   /// <returns>Order</returns>
   public int GetOrder() => 2;

   #endregion
}