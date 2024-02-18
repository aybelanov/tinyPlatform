using System;

namespace SensorEmulator.Domain;

/// <summary>
/// Represent a sensor message
/// </summary>
internal class SensorMessage
{
   /// <summary>
   /// Sensor instance
   /// </summary>
   public Sensor Sensor { get; set; }

   /// <summary>
   /// Sensor value
   /// </summary>
   public double Value { get; set; }

   /// <summary>
   /// Message data byte array
   /// </summary>
   public byte[] Bytes { get; set; }

   /// <summary>
   /// Complex sensor data as json object
   /// </summary>
   public string JsonValue { get; set; }

   /// <summary>
   /// ShortDescription of the data values
   /// </summary>
   public string Metadata { get; set; }

   /// <summary>
   /// Message timestamp
   /// </summary>
   public DateTime Timestamp { get; set; }
}
