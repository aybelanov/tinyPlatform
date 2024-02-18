using Hub.Core.Domain.Localization;
using Shared.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represent a monitor to presentation (sensor-to-widget) mapping
/// </summary>
public class Presentation : BaseEntity, ILocalizedEntity
{
   /// <summary>
   /// Get or set a monitor identifier
   /// </summary>
   public long MonitorId { get; set; }

   /// <summary>
   /// Get or set sensor-to-widget identifier
   /// </summary>
   public long SensorWidgetId { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Mapping name
   /// </summary>
   [NotMapped, Localizable]
   public string Name { get; set; }

   /// <summary>
   /// Mapping description
   /// </summary>
   [NotMapped, Localizable]
   public string Description { get; set; }

   #region data transfer properties

   /// <summary>
   /// Sensor identifier
   /// </summary>
   [NotMapped]
   public long SensorId { get; set; }

   /// <summary>
   /// Widget identifier
   /// </summary>
   [NotMapped]
   public long WidgetId { get; set; }

   /// <summary>
   /// Device identifier
   /// </summary>
   [NotMapped]
   public long DeviceId { get; set; }

   /// <summary>
   /// Device name
   /// </summary>
   [NotMapped]
   public string DeviceName { get; set; }

   /// <summary>
   /// Widget name
   /// </summary>
   [NotMapped]
   public string WidgetName { get; set; }

   /// <summary>
   /// Sensor name
   /// </summary>
   [NotMapped]
   public string SensorName { get; set; }

   /// <summary>
   /// Sensor
   /// </summary>
   [NotMapped]
   public Sensor Sensor { get; set; }

   /// <summary>
   /// Device
   /// </summary>
   [NotMapped]
   public Device Device { get; set; }

   /// <summary>
   /// Widget
   /// </summary>
   [NotMapped]
   public Widget Widget { get; set; }

   #endregion
}
