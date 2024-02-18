using Shared.Common;

namespace Shared.Clients.Domain;

/// <summary>
/// Required data of the sensor
/// </summary>
public interface ISensor
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   long Id { get; set; }

   /// <summary>
   /// Json seriliaze configuration of a sensor
   /// </summary>
   string Configuration { get; set; }

   /// <summary>
   /// Priority of sensding data from a sensor
   /// </summary>
   PriorityType PriorityType { get; set; }

   /// <summary>
   /// System name of the sensor. Unique in a whatcher system 
   /// </summary>
   string SystemName { get; set; }

   /// <summary>
   /// Sensor' device id
   /// </summary>
   long DeviceId { get; set; }

   /// <summary>
   /// Short description of a sensor
   /// </summary>
   string Description { get; set; }

   /// <summary>
   /// If the sensor is on/off
   /// </summary>
   bool Enabled { get; set; }

   /// <summary>
   /// Type of measured value
   /// </summary>
   SensorType SensorType { get; set; }

   /// <summary>
   /// Measure unit
   /// </summary>
   string MeasureUnit { get; set; }

   /// <summary>
   /// Name of a sensor
   /// </summary>
   string Name { get; set; }

   /// <summary>
   /// Show sensor data on the main page
   /// </summary>
   bool ShowInCommonLog { get; set; }
}