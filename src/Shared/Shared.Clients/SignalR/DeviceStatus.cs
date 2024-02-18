namespace Shared.Clients.SignalR;

/// <summary>
/// Represents a device status state
/// </summary>
public class DeviceConnectionStatus
{
   /// <summary>
   /// Device identifier
   /// </summary>
   public long DeviceId { get; set; }

   /// <summary>
   /// IP addres of the connected device
   /// </summary>
   public string IPAddress { get; set; }
}
