using Shared.Clients.Domain;
using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Represents a sensor data entity
/// </summary>
public class SensorRecord : BaseEntity, ISensorRecord
{
   /// <summary>
   /// Id of the sensor (a foreign key)
   /// </summary>
   public long SensorId { get; set; }

   /// <summary>
   /// value of the sensor in a time point
   /// </summary>
   public double Value { get; set; }

   /// <summary>
   /// Sensor data in byte format
   /// </summary>
   public byte[] Bytes { get; set; }

   /// <summary>
   /// Complex sensor data as json object
   /// </summary>
   public string JsonValue { get; set; }

   /// <summary>
   /// Description of the data values
   /// </summary>
   public string Metadata { get; set; }

   /// <summary>
   /// Unix milisecond timestamp of creation
   /// </summary>
   public long Timestamp { get; set; }

   /// <summary>
   /// Unix milisecond timestamp of creation
   /// </summary>
   public long EventTimestamp { get; set; }
}
