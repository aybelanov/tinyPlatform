using System;

namespace SensorEmulator.Domain;

/// <summary>
/// Global navigation satelite system [custom] class
/// </summary>
[Serializable]
public class GNSS
{
   /// <summary>
   /// Longtitude
   /// </summary>
   public double Lon { get; set; }

   /// <summary>
   /// Latitude
   /// </summary>
   public double Lat { get; set; }

   /// <summary>
   /// Navigation height
   /// </summary>
   public double Height { get; set; }

   /// <summary>
   /// Fixed datetime ticks
   /// </summary>
   public long Ticks { get; set; }

   /// <summary>
   /// Reliable
   /// </summary>
   public bool Reliable { get; set; }

   /// <summary>
   /// GNSS ticks
   /// </summary>
   public double Speed { get; set; }

   /// <summary>
   /// GNSS Course
   /// </summary>
   public double Course { get; set; }
}