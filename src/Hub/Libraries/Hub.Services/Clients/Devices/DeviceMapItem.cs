using Shared.Clients;
using Shared.Common;

namespace Hub.Services.Clients.Devices;

/// <summary>
/// Represents a device map item
/// </summary>
public class DeviceMapItem : BaseEntity
{
    /// <summary>
    /// Devie locale name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Device location's latitude
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Device location's longtitude
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// Is a device mobile
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// Device online status
    /// </summary>
    public OnlineStatus Status { get; set; }

    /// <summary>
    /// Is device "shared"
    /// </summary>
    public bool IsShared { get; set; }
}
