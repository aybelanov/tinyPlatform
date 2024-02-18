namespace Hub.Core.Configuration;

/// <summary>
/// Represents installation configuration parameters
/// </summary>
public partial class InstallationConfig : IConfig
{
   /// <summary>
   /// Gets or sets a value indicating whether a server owner can install sample data during installation
   /// </summary>
   public bool DisableSampleData { get; private set; } = false;

   /// <summary>
   /// Gets or sets a list of plugins ignored during application installation
   /// </summary>
   public string DisabledPlugins { get; private set; } = string.Empty;

   /// <summary>
   /// Gets or sets a value indicating whether to download and setup the regional language pack during installation
   /// </summary>
   public bool InstallRegionalResources { get; private set; } = true;

   /// <summary>
   /// Gets or sets a value indicating whether a platform owner can install sample data during installation
   /// </summary>
   public InstallSampleData InstallSampleData { get; set; } = new();

}

/// <summary>
/// Sample data config
/// </summary>
public class InstallSampleData
{
   /// <summary>
   ///  Gets or sets a value indicating whether a platform owner can install junk sample data during installation
   /// </summary>
   public bool InstallJunk { get; set; } = false;

   /// <summary>
   /// Counts of junk users
   /// </summary>
   public int SampleUsersCount { get; set; }

   /// <summary>
   /// Count of units
   /// </summary>
   public int SampleMonitorsCount { get; set; } = 10;

   /// <summary>
   /// Count of widget
   /// </summary>
   public int SampleWidgetsCount { get; set; } = 10;

   /// <summary>
   /// Count of units
   /// </summary>
   public int SampleDevicesCount { get; set; } = 10;

   /// <summary>
   /// Count of sensors
   /// </summary>
   public int SampleSensorsCount { get; set; } = 15;
}