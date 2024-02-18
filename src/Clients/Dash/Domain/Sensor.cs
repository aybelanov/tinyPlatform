using Shared.Clients.Domain;
using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Represent a sensor class
/// </summary>
public class Sensor : BaseEntity, ISensor
{
   /// <summary>
   /// System name of a sensor. Must be a unique for a device
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Id of the wather entity in the data base (foreign key)
   /// </summary>
   public long DeviceId { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// Gets or sets a display order.
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
   /// Type of measured value
   /// </summary>
   public SensorType SensorType { get; set; }
   
   /// <summary>
   /// Measure unit
   /// </summary>
   public string MeasureUnit { get; set; }

   /// <summary>
   /// Priority of sensding data from a sensor
   /// </summary>
   public PriorityType PriorityType { get; set; }

   /// <summary>
   /// Show sensor data on the main page
   /// </summary>
   public bool ShowInCommonLog { get; set; }

   /// <summary>
   /// Sensor picture url
   /// </summary>
   public string PictureUrl { get; set; }

   /// <summary>
   /// Device system name
   /// </summary>
   public string DeviceSystemName { get; set; }
}
