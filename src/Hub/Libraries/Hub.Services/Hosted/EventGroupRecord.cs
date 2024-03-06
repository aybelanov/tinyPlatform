using System.Collections.Generic;

namespace Hub.Services.Hosted;

/// <summary>
/// Represents an event records to prepare data file upload
/// </summary>
public class EventGroupRecord
{
   /// <summary>
   /// Sensor even timestamp
   /// </summary>
   public long EventTimestamp { get; set; }

   /// <summary>
   /// value records
   /// </summary>
   public IEnumerable<ReportRecord> Records { get; set; }
}

/// <summary>
/// Simplified sensor data record for preparing reports
/// </summary>
public class ReportRecord
{
   /// <summary>
   /// Sensor identifier
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
}
