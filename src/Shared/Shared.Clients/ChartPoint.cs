namespace Shared.Clients;

/// <summary>
/// Represents a point on a cahrt
/// </summary>
public class ChartPoint
{
   /// <summary>
   /// Ordinate value
   /// </summary>
   public double Y { get; set; }

   /// <summary>
   /// Ordinate minimal value
   /// if the source was been compressed 
   /// </summary>
   public double MinY { get; set; }

   /// <summary>
   /// Ordinate maximal value
   /// if the source was been compressed 
   /// </summary>
   public double MaxY { get; set; }

   /// <summary>
   /// Abscissa value
   /// </summary>
   public double X { get; set; }
}
