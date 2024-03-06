using Shared.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devices.Dispatcher.Domain;

/// <summary>
/// Represents a sensor data record class
/// </summary>
public class SensoRecord : BaseEntity, ISensorRecord
{
   /// <summary>
   /// Sensor data in byte format
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
   /// Id of the sensor (a foreign key)
   /// </summary>
   public long SensorId { get; set; }

   /// <summary>
   /// Sensor system name
   /// </summary>
   [NotMapped]
   public string SensorSystemName { get; set; }

   /// <summary>
   /// value of the sensor in a time point
   /// </summary>
   public double Value { get; set; }

   /// <summary>
   /// Unix milisecond timestamp of creation by sensor
   /// </summary>
   public long Timestamp { get; set; }

   /// <summary>
   /// Unix milisecond timestamp of creation
   /// </summary>
   public long EventTimestamp { get; set; }

   /// <summary>
   ///  If the record was sent and received a server
   /// </summary>
   public bool IsSent { get; set; }
}
