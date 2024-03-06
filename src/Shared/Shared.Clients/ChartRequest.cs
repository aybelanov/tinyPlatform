using System;
using System.Collections.Generic;

namespace Shared.Clients;

/// <summary>
/// represents achart request
/// </summary>
public class ChartRequest
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
   /// Sensor identifiers
   /// </summary>
   public IEnumerable<long> SensorIds { get; set; }

   /// <summary>
   /// Chart html-element width on the screen in pixels
   /// </summary>
   public int ChartWidth { get; set; }
}
