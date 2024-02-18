namespace Hub.Core.Domain.Clients;

/// <summary>
/// Device logged-in event
/// </summary>
public class DeviceLoggedinEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="device">Device</param>
    public DeviceLoggedinEvent(Device device)
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