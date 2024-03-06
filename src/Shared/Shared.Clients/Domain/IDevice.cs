using System;

namespace Shared.Clients.Domain;

/// <summary>
/// Device entity interface
/// </summary>
public interface IDevice
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   long Id { get; set; }

   /// <summary>
   /// JSON configuration of the device
   /// </summary>
   string Configuration { get; set; }

   /// <summary>
   /// Max count of the records in a database for SensorRecords table
   /// </summary>
   int CountDataRows { get; set; }

   /// <summary>
   /// Delay between repeat of sending data stream in miliseconds  
   /// </summary>
   int DataSendingDelay { get; set; }

   /// <summary>
   /// Period  of the repeat clearing the database in seconds
   /// </summary>
   int ClearDataDelay { get; set; }

   /// <summary>
   /// Size of data pakage for a seance of dataflow
   /// </summary>
   int DataPacketSize { get; set; }

   /// <summary>
   /// Delay between the data flow seances in miliseconds
   /// </summary>
   int DataflowReconnectDelay { get; set; }

   /// <summary>
   /// Name of the watcher
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Name of the watcher
   /// </summary>
   string Name { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   string Description { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting and groping by device
   /// </summary>
   int DisplayOrder { get; set; }

   /// <summary>
   /// Is device active (approved or not blocked/banned)
   /// </summary>
   bool IsActive { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   bool Enabled { get; set; }

   /// <summary>
   /// Gets or sets the last IP address
   /// </summary>
   string LastIpAddress { get; set; }

   /// <summary>
   /// Gets or sets the date and time of last activity
   /// </summary>
   DateTime? LastActivityOnUtc { get; set; }

   /// <summary>
   /// If it be set true
   /// the device location will be shown
   /// from GNNS sensor (must be set)
   /// </summary>
   bool IsMobile { get; set; }

   /// <summary>
   /// Device location's longitude
   /// </summary>
   /// <remarks>
   /// If a device has a GNSS sensor
   /// GNNS location will be shown
   /// </remarks>
   double Lon { get; set; }

   /// <summary>
   /// Device location's latitude
   /// </summary>
   /// <remarks>
   /// If a device has a GNSS sensor
   /// GNNS location will be shown
   /// </remarks>
   double Lat { get; set; }

   /// <summary>
   /// Show device location on the main page's map
   /// </summary>
   bool ShowOnMain { get; set; }
}