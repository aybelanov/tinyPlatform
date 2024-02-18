using System.Collections.Generic;

namespace Shared.Clients;

/// <summary>
/// Represents chart data set
/// </summary>
public class ChartSet
{
   /// <summary>
   /// Entity identifier
   /// </summary>
   public long EntityId { get; set; }

   /// <summary>
   /// Chart name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Ordinate axis name
   /// </summary>
   public string OrdinateName { get; set; }

   /// <summary>
   /// Abscissa axiы тame
   /// </summary>
   public string AbscissaName { get; set; }

   /// <summary>
   /// Chart point data set
   /// </summary>
   public IEnumerable<ChartPoint> Data { get; set; }
}
