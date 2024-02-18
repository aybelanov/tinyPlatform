using Clients.Dash.Domain;
using Clients.Dash.Models;
using Shared.Clients.Domain;
using Shared.Common;
using System;

namespace Clients.Dash.Pages.Configuration.Sensors;

/// <summary>
/// Represents a sensor model class
/// </summary>
public class SensorModel : BaseEntityModel, ISensor
{
   /// <summary>
   /// System name of a sensor. Must be a unique for the device
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Device picture url
   /// </summary>
   public string PictureUrl { get; set; }

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
   /// This value is used when sorting associated products (used with "grouped" products)
   /// This value is used when sorting home page products
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
   /// String value of the measured value type
   /// </summary>
   public string SensorTypeString
   {
      get => Enum.GetName(SensorType);
      set
      {
         if (Enum.TryParse<SensorType>(value, true, out var sensorType))
            SensorType = sensorType;
      }
   }

   /// <summary>
   /// Measure unit
   /// </summary>
   public string MeasureUnit { get; set; }

   /// <summary>
   /// Priority of sensding data from a sensor
   /// </summary>
   public PriorityType PriorityType { get; set; }

   /// <summary>
   /// 
   /// </summary>
   public string PriorityTypeString
   {
      get => Enum.GetName(PriorityType);
      set
      {
         if (Enum.TryParse<PriorityType>(value, true, out var priorityType))
            PriorityType = priorityType;
      }
   }

   /// <summary>
   /// Device-owner of the sensor
   /// </summary>
   public Device Device { get; set; }

   /// <summary>
   /// Show sensor data in the common log 
   /// </summary>
   public bool ShowInCommonLog { get; set; }

   /// <summary>
   /// Device system name
   /// </summary>
   public string DeviceSystemName { get; set; }
}
