using Devices.Dispatcher.Services.Settings;
using Shared.Common;
using Shared.Devices;

namespace Devices.Dispatcher.Configuration;

/// <summary>
/// Represent a device setting
/// </summary>
public class DeviceSettings : BaseEntity, ISettings, IDevice
{
   /// <summary>
   /// Delay between the data flow seances in miliseconds
   /// </summary>
   public int DataflowReconnectDelay { get; set; } = 10_000;

   /// <summary>
   /// Size of data pakage for a seance of dataflow
   /// </summary>
   public int DataPacketSize { get; set; } = 1_000;

   /// <summary>
   /// Max count of the records in a database for SensorRecords table
   /// </summary>
   public int CountDataRows { get; set; } = 1_000_000;

   /// <summary>
   /// Delay for repeat clearing the database in seconds
   /// </summary>
   public int ClearDataDelay { get; set; } = 3600;

   /// <summary>
   /// JSON configuration of the device
   /// </summary>
   public string Configuration { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// Gets or sets the date and time of device update
   /// </summary>
   public long ModifiedTicks { get; set; }

   /// <summary>
   /// Delay between repeat of sending data stream in miliseconds  
   /// </summary>
   public int DataSendingDelay { get; set; } = 1_000;

   /// <summary>
   /// Storage time of video segments in hours
   /// </summary>
   public int VideoSegmentsExpiration { get; set; } = 72;
}