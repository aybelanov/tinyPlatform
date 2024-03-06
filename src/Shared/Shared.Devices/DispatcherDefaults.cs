namespace Shared.Devices;

/// <summary>
/// Device shared defaults
/// </summary>
public static class DispatcherDefaults
{
   #region Communication

   /// <summary>
   /// Header command key
   /// </summary>
   public static string HeaderCommandKey => "X-NeedToRequestCommand";

   /// <summary>
   /// Header device identifier
   /// </summary>
   public static string DeviceIdHeaderKey => "X-DeviceId";

   /// <summary>
   /// Header configuration key
   /// </summary>
   public static string HeaderConfigurationKey => "X-Configuration-Version";

   /// <summary>
   /// Header process identifier key
   /// </summary>
   public static string HeaderProcessIdKey => "X-ProcessGuid";

   /// <summary>
   /// Curtent distcher application version
   /// </summary>
   public static string AppVersionHeader => "X-Dispatcher-Version";

   #endregion
}
