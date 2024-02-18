using Hub.Core.Domain.Clients;
using System;

namespace Hub.Services.Media;

/// <summary>
/// Representa a HLS segment metadata
/// </summary>
public class SegmentMetadata
{
   /// <summary>
   /// Segment file extension
   /// </summary>
   public string Extension { get; set; }

   /// <summary>
   /// Segment file name
   /// </summary>
   public string FileName { get; set; }

   /// <summary>
   /// Segment file full name
   /// </summary>
   public string FullFileName { get; set; }

   /// <summary>
   /// Segment file destination directory
   /// </summary>
   public string DestinationDirectory { get; set; }

   /// <summary>
   /// Segment file exist flag
   /// </summary>
   public bool IsFileExist { get; set; }

   /// <summary>
   /// Segment creation datetime UTC
   /// </summary>
   public DateTime CreationOnUtc { get; set; }

   /// <summary>
   /// Video segment duration
   /// </summary>
   public double Duration { get; set; }

   /// <summary>
   /// Source of the video fragment (IP camera)
   /// </summary>
   public Sensor Source { get; set; }
}
