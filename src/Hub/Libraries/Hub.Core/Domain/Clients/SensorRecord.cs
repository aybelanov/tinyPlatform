using Shared.Common;
using System;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents a sensordata class
/// </summary>
public class SensorRecord : BaseEntity, ISensorRecord
{
   /// <summary>
   /// Id of the sensor (a foreign key)
   /// </summary>
   public long SensorId { get; set; }

   /// <summary>
   /// Scalar value of the sensor in a time point
   /// </summary>
   public double Value { get; set; }

   /// <summary>
   /// Complex sensor data in byte format (as byte array)
   /// </summary>
   public byte[] Bytes { get; set; }

   /// <summary>
   /// Complex sensor data as json object
   /// </summary>
   public string JsonValue { get; set; }

   /// <summary>
   /// Metadata of the data values
   /// </summary>
   public string Metadata { get; set; }

   /// <summary>
   /// Unix milisecond timestamp
   /// </summary>
   public long Timestamp { get; set; }

   /// <summary>
   /// Unix milisecond timestamp
   /// </summary>
   public long EventTimestamp { get; set; }

   /// <summary>
   /// Created time on UTC in Hub database
   /// </summary>
   public DateTime CreatedTimeOnUtc { get; set; }
}
