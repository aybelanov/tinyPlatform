using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Shared.Clients;
using Shared.Clients.Domain;
using Shared.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represent a device entity
/// </summary>
public class Device : BaseEntity, IDevice, Shared.Devices.IDevice, IModifiedEntity, ILocalizedEntity, ISoftDeletedEntity
{
   /// <summary>
   /// Unique id of the device
   /// </summary>
   public Guid Guid { get; set; }

   /// <summary>
   /// gets or sets unique device system name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   [NotMapped, Localizable]
   public string Name { get; set; }

   /// <summary>
   /// Device picture idetnifier
   /// </summary>
   public long PictureId { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   [NotMapped, Localizable]
   public string Description { get; set; }

   /// <summary>
   /// JSON configuration of the unit
   /// </summary>
   public string Configuration { get; set; }

   /// <summary>
   /// Gets or sets the admin comment
   /// </summary>
   public string AdminComment { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting on a page
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Enabled { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity has been deleted
   /// </summary>
   public bool IsDeleted { get; set; }

   /// <summary>
   ///  Is the device is not active
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
   public double Lon {  get; set; }

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

   /// <summary>
   /// Gets or sets the last IP address
   /// </summary>
   public string LastIpAddress { get; set; }

   /// <summary>
   /// Max count of the records in a database for SensorDatas table
   /// </summary>
   public int CountDataRows { get; set; }

   /// <summary>
   /// Delay between repeat of sending data stream in miliseconds  
   /// </summary>
   public int DataSendingDelay { get; set; }

   /// <summary>
   /// Delay between the data flow seances in miliseconds
   /// </summary>
   public int DataflowReconnectDelay { get; set; }

   /// <summary>
   /// Size of data pakage for a seance of dataflow
   /// </summary>
   public int DataPacketSize { get; set; }

   /// <summary>
   /// Delay for repeat clearing the database in seconds
   /// </summary>
   public int ClearDataDelay { get; set; }

   /// <summary>
   /// Last device activity on UTC
   /// </summary>
   public DateTime? LastActivityOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit creating
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit updating
   /// </summary>
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets a value indicating number of failed login attempts (wrong password)
   /// </summary>
   public int FailedLoginAttempts { get; set; }

   /// <summary>
   /// Gets or sets the date and time until which a user cannot login (locked out)
   /// </summary>
   public DateTime? CannotLoginUntilDateUtc { get; set; }

   /// <summary>
   /// User-owner identifier
   /// </summary>
   public long OwnerId { get; set; }

   #region data transfer properties

   /// <summary>
   /// Owner name (uses for client proto projection)
   /// </summary>
   [NotMapped]
   public string OwnerName { get; set; }

   /// <summary>
   /// Device online connection status
   /// </summary>
   [NotMapped]
   public OnlineStatus ConnectionStatus { get; set; }

   /// <summary>
   /// Icon picture url
   /// </summary>
   [NotMapped]
   public string PictureUrl { get; set; }

   #endregion
}