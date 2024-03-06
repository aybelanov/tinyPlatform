namespace Hub.Core.Domain.Clients;

/// <summary>
/// "Device is logged out" event
/// </summary>
public class DeviceLoggedOutEvent
{
   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="device">Device</param>
   public DeviceLoggedOutEvent(Device device)
   {
      Device = device;
   }

   /// <summary>
   /// Get or set the device
   /// </summary>
   public Device Device { get; }
}