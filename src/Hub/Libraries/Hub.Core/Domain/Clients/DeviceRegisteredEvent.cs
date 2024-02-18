namespace Hub.Core.Domain.Clients;

/// <summary>
/// Device registered event
/// </summary>
public class DeviceRegisteredEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="device">user</param>
    public DeviceRegisteredEvent(Device device)
    {
        Device = device;
    }

    /// <summary>
    /// Device
    /// </summary>
    public Device Device
    {
        get;
    }
}