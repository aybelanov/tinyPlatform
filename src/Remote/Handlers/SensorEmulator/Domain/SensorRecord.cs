namespace SensorEmulator.Domain;

/// <summary>
/// Represents a sensor record
/// </summary>
public class SensorRecord
{
   /// <summary>
   /// Sensor sustem name
   /// </summary>
   public string SensorSystemName { get; set; }

   /// <summary>
   /// Sensor scalar value
   /// </summary>
   public double Value { get; set; }

   /// <summary>
   /// Record metadata description
   /// </summary>
   public string Metadata { get; set; }

   /// <summary>
   /// Complex sensor data as json object
   /// </summary>
   public string JsonValue { get; set; }

   /// <summary>
   /// Byte array of sensor data
   /// </summary>
   public byte[] Bytes { get; set; }

   /// <summary>
   /// Record timestamp
   /// </summary>
   public long Timestamp { get; set; }

   /// <summary>
   /// Record event timestamp
   /// </summary>
   public long EventTimestamp { get; set; }
}
