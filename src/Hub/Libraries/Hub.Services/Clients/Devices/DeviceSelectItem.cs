using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Devices;

/// <summary>
/// Device select item for client UI elemets like a dropdown list
/// </summary>
public class DeviceSelectItem : BaseEntity
{
    /// <summary>
    /// Device system name
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Device localized name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Is device "shared"
    /// </summary>
    public bool IsShared { get; set; }
}
