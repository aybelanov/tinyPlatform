using System;
using System.ComponentModel.DataAnnotations;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Shared.Clients.Domain;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a device model 
/// </summary>
public partial record DeviceModel : BaseAppEntityModel
{
   /// <summary>
   /// gets or sets unique device system name
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.SystemName")]
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.Name")]
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.ShortDescription")]
   public string Description { get; set; }

   /// <summary>
   /// JSON configuration of the unit
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.Configuration")]
   public string Configuration { get; set; }

   /// <summary>
   /// Gets or sets the admin comment
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.AdminComment")]
   public string AdminComment { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting for presentation
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.Enabled")]
   public bool Enabled { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity has been deleted
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.IsDeleted")]
   public bool IsDeleted { get; set; }

   /// <summary>
   ///  Is the device is not active
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.IsActive")]
   public bool IsActive { get; set; }

   /// <summary>
   /// Gets or sets the date and time until which a user cannot login (locked out)
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.CannotLoginUntilDateUtc")]
   [UIHint("DateTimeNullable")]
   public DateTime? CannotLoginUntilDateUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of last activity
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.LastActivityDateUtc")]
   public DateTime? LastActivityDateUtc { get; set; }

   /// <summary>
   /// Gets or sets the last IP address
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.IPAddress")]
   public string IPAddress { get; set; }

   /// <summary>
   /// Max count of the records in a database for SensorDatas table
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.CountDataRows")]
   public int CountDataRows { get; set; }

   /// <summary>
   /// Delay between repeat of sending data stream in miliseconds  
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.DataSendingDelay")]
   public int DataSendingDelay { get; set; }

   /// <summary>
   /// Delay between the data flow seances in miliseconds
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.DataflowReconnectDelay")]
   public int DataflowReconnectDelay { get; set; }

   /// <summary>
   /// Size of data pakage for a seance of dataflow
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.DataPacketSize")]
   public int DataPacketSize { get; set; }

   /// <summary>
   /// Delay for repeat clearing the database in seconds
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.ClearDataDelay")]
   public int ClearDataDelay { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit creating
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.CreatedOnUtc")]
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit updating
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.UpdatedOnUtc")]
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Device credential identifier
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.Password")]
   [DataType(DataType.Password)]
   public string Password { get; set; }

   /// <summary>
   /// Gets or sets the date and time of last login
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.LastLoginDateUtc")]
   public DateTime? LastLoginDateUtc { get; set; }

   /// <summary>
   /// User-owner identifier
   /// </summary>
   [AppResourceDisplayName("Admin.Devices.Devices.Fields.OwnerName")]
   public string OwnerName { get; set; }

   public long OwnerId { get; set; }

   public DeviceUserSearchModel DeviceUserSearchModel { get; set; } = new();

   public DeviceActivityLogSearchModel DeviceActivityLogSearchModel { get; set; } = new();

   public OnlineDeviceSearchModel OnlineDeviceSearchModel { get; set; } = new();

   public DeviceStatisticsModel DeviceStatisticsModel { get; set; } = new();
}
