namespace Shared.Common;

/// <summary>
/// Represents a sensor record intreface
/// </summary>
public interface ISensorRecord
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   long Id { get; set; }

   /// <summary>
   /// Sensor data in byte format
   /// </summary>
   byte[] Bytes { get; set; }

   /// <summary>
   /// Complex sensor data as json object
   /// </summary>
   public string JsonValue { get; set; }

   /// <summary>
   /// ShortDescription of the data values
   /// </summary>
   string Metadata { get; set; }

   /// <summary>
   /// Unix milisecond timestamp of creation
   /// </summary>
   long EventTimestamp { get; set; }

   /// <summary>
   /// Id of the sensor (a foreign key)
   /// </summary>
   long SensorId { get; set; }

   /// <summary>
   /// Unix milisecond timestamp of creation by sensor
   /// </summary>
   long Timestamp { get; set; }

   /// <summary>
   /// Scalar value of the sensor in a time point
   /// </summary>
   double Value { get; set; }
}