using System;
using System.Collections.Generic;

namespace Shared.Clients;

/// <summary>
/// Download data file request
/// </summary>
public class DownloadRequest
{
   /// <summary>
   /// Date time "from"
   /// </summary>
   public DateTime From { get; set; }

   /// <summary>
   /// Date time "to"
   /// </summary>
   public DateTime To { get; set; }

   /// <summary>
   /// Device identifiers
   /// </summary>
   public long DeviceId { get; set; }

   /// <summary>
   /// Sensor identifiers
   /// </summary>
   public IEnumerable<long> SensorIds { get; set; }

   /// <summary>
   /// File format
   /// </summary>
   public ExportFileType Format { get; set; }

   /// <summary>
   /// File compression
   /// </summary>
   public FileCompressionType Compression { get; set; }

   /// <summary>
   /// Current page size (need after added to show firsy page)
   /// </summary>
   public int Top { get; set; }
}