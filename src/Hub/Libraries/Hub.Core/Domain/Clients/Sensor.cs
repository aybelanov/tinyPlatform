using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Security;
using Shared.Clients.Domain;
using Shared.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// represent a sensor entity
/// </summary>
public class Sensor : BaseEntity, IModifiedEntity, ILocalizedEntity, IAclSupported, ISoftDeletedEntity, ISensor, Shared.Devices.ISensor
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   [NotMapped, Localizable]
   public string Name { get; set; }

   /// <summary>
   /// System name of a sensor. Must be a unique among the device sensors
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Sensor picture idetnifier
   /// </summary>
   public long PictureId { get; set; }

   /// <summary>
   /// Id of the wather entity in the data base (foreign key)
   /// </summary>
   public long DeviceId { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   [NotMapped, Localizable]
   public string Description { get; set; }

   /// <summary>
   /// Gets or sets the admin comment
   /// </summary>
   public string AdminComment { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is subject to ACL
   /// </summary>
   public bool SubjectToAcl { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Enabled { get; set; }


   /// <summary>
   /// Json seriliaze configuration of a sensor
   /// </summary>
   public string Configuration { get; set; }

   /// <summary>
   /// Measure unit
   /// </summary>
   [NotMapped, Localizable]
   public string MeasureUnit { get; set; }

   /// <summary>
   /// Type of measured value
   /// </summary>
   public SensorType SensorType { get; set; }

   /// <summary>
   /// Priority of sensding data from a sensor
   /// </summary>
   public PriorityType PriorityType { get; set; }

   /// <summary>
   /// Show sensor data in the common log
   /// </summary>
   public bool ShowInCommonLog { get; set; }

   /// <summary>
   /// Gets or sets the date and time of product creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit updating
   /// </summary>
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Soft deleted flag
   /// </summary>
   public bool IsDeleted { get; set; }

   #region data transfer properties

   /// <summary>
   /// Device system name
   /// </summary>
   [NotMapped]
   public string DeviceSystemName { get; set; }

   /// <summary>
   /// Icon picture url
   /// </summary>
   [NotMapped]
   public string PictureUrl { get; set; }

   #endregion
}