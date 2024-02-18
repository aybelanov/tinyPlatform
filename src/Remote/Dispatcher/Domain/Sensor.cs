using System.Text.Json.Serialization;
using Shared.Common;
using Shared.Devices;

namespace Devices.Dispatcher.Domain;


/// <summary>
/// Represents a device sensor class
/// </summary>
public class Sensor : BaseEntity, ISensor
{
   /// <summary>
   /// System name of the sensor. Unique in a whatcher system 
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Json seriliaze configuration of a sensor
   /// </summary>
   public string Configuration { get; set; }

   /// <summary>
   /// Priority of sensding data from a sensor
   /// </summary>
   [JsonConverter(typeof(JsonStringEnumConverter))]
   public PriorityType PriorityType { get; set; }
}
