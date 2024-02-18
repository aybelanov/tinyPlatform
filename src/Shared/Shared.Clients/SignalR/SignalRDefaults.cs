namespace Shared.Clients.SignalR;

/// <summary>
/// Represents solution constant as static props for SignalR
/// </summary>
public static class SignalRDefaults
{
   /// <summary>
   /// SignalR connection identifier header
   /// </summary>
   public static string SignalrConnectionIdHeader => "X-SignalR-ConnectionId";

   /// <summary>
   /// Send sensor data method
   /// </summary>
   public static string SensorDataMethod => "SensorDataMethod";
   
   /// <summary>
   /// Download task status changed method
   /// </summary>
   public static string DownloadTaskStatusChanged => "DownloadTaskStatusChanged";

   /// <summary>
   /// 'From a device' notification method
   /// </summary>
   public static string DeviceNotificationMethod => "DeviceNotification";

   /// <summary>
   /// 'From a device' notification method
   /// </summary>
   public static string DeviceCommandMethod => "DeviceCommand";

   /// <summary>
   /// Common communication server-client by user
   /// </summary>
   public static string ClientMessageMethod => "ClientMessageMethod";

   /// <summary>
   /// Subscribe to a group method name
   /// </summary>
   public static string SubScribeToSensorGroup => "AddToSensorGoup";

   /// <summary>
   /// Unsubscribe from a group method name
   /// </summary>
   public static string UnsubScribeToSensorGroup => "RemoveFromSensorGroup";

   /// <summary>
   /// Device online status changed method
   /// </summary>
   public static string DeviceStatusChanged => "DeviceStatusChanged";

   /// <summary>
   /// Use for update device status after reconecting for opdate status (connect)
   /// </summary>
   public static string SubscribeToDeviceStatus => "SubscribeToDeviceStatus";

   /// <summary>
   /// Use for update device status after reconecting for opdate status (disconnect)
   /// </summary>
   public static string UnsubscribeFromDeviceStatus => "UnsubscribeFromDeviceStatus";

   /// <summary>
   /// Subscribes to groups
   /// </summary>
   public static string SubscibeToGroups => "SubscibeToGroups";

   /// <summary>
   /// Unsubscribes to groups
   /// </summary>
   public static string UnsubscibeFromGroups => "UnsubscibeFromGroups";

   /// <summary>
   /// Subscribes to common log messages
   /// </summary>
   public static string SubscribeToCommonLogMessages => "SubscribeToCommonLog";

   /// <summary>
   /// Unsubscribes from common log messages
   /// </summary>
   public static string UnsubscribeFromCommonLogMessages => "UnsubscribeFromCommonLog";

   /// <summary>
   /// Subscribes to sensor data stream method
   /// </summary>
   public static string SubscribeToSensorDataStream => "SubscribeToSensorDataStream";

   /// <summary>
   /// Subscribes to sensor data stream method
   /// </summary>
   public static string UnsubscribeFromSensorDataStream => "UnsubscribeFromSensorDataStream";
}
