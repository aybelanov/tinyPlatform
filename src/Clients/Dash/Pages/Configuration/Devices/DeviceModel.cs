using Clients.Dash.Domain;
using Clients.Dash.Models;
using Shared.Clients;
using Shared.Clients.Domain;
using System;

namespace Clients.Dash.Pages.Configuration.Devices;

/// <summary>
/// Represents a device model class
/// </summary>
public class DeviceModel : BaseEntityModel, IDevice
{
    /// <summary>
    /// System name of the device. Must be unique for a device
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Device password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the short description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Device picture url
    /// </summary>
    public string PictureUrl { get; set; }

    /// <summary>
    /// JSON configuration of the device
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// Gets or sets a display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 
    /// the data from device will be saved by the hub
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// IP address of the connected device
    /// </summary>
    public string LastIpAddress { get; set; }

    /// <summary>
    /// Max count of the records in a database for SensorRecords table
    /// </summary>
    public int CountDataRows { get; set; }

    /// <summary>
    /// Delay between repeat of sending data stream in miliseconds  
    /// </summary>
    public int DataSendingDelay { get; set; }

    /// <summary>
    /// Period  of the repeat clearing the database in seconds
    /// </summary>
    public int ClearDataDelay { get; set; }

    /// <summary>
    /// Size of data pakage for a seance of dataflow
    /// </summary>
    public int DataPacketSize { get; set; }

    /// <summary>
    /// Delay between the data flow seances in miliseconds
    /// </summary>
    public int DataflowReconnectDelay { get; set; }

    /// <summary>
    /// Gets or sets the date and time of device update
    /// </summary>
    public long ModifiedTicks { get; set; }

    /// <summary>
    /// Gets or sets the date and time of last activity
    /// </summary>
    public DateTime? LastActivityOnUtc { get; set; }

    /// <summary>
    /// Last activity datetime string 
    /// </summary>
    public string LastActivityString { get; set; }

    /// <summary>
    /// Gets or sets the date and time of device creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Device connection status
    /// </summary>
    public OnlineStatus ConnectionStatus { get; set; }

    ///// <summary>
    ///// Device connection status string
    ///// </summary>
    //public string ConnectionStatusString { get; set; }

    /// <summary>
    /// Device user-owner
    /// </summary>
    public long OwnerId { get; set; }

    /// <summary>
    /// Device user-owner name
    /// </summary>
    public string OwnerName { get; set; }

    /// <summary>
    /// Is device active (approved or not blocked/banned)
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// If it be set true
    /// the device location will be shown
    /// from GNNS sensor (must be set)
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// Device location's longitude
    /// </summary>
    /// <remarks>
    /// If a device has a GNSS sensor
    /// GNNS location will be shown
    /// </remarks>
    public double Lon { get; set; }

    /// <summary>
    /// Device location's latitude
    /// </summary>
    /// <remarks>
    /// If a device has a GNSS sensor
    /// GNNS location will be shown
    /// </remarks>
    public double Lat { get; set; }

    /// <summary>
    /// Show device location on the main page's map
    /// </summary>
    public bool ShowOnMain { get; set; }
}
