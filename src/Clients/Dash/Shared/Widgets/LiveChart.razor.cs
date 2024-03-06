namespace Clients.Dash.Shared.Widgets;

/// <summary>
/// Component partial chart
/// </summary>
public partial class LiveChart
{
   /// <summary>
   /// Live data item class
   /// </summary>
   private class LiveItemModel
   {
      /// <summary>
      /// Sensor value
      /// </summary>
      public double Value { get; set; }

      /// <summary>
      /// Уlapsed time
      /// </summary>
      public int TimeAgo { get; set; }

      /// <summary>
      /// Timestamp of the sensor data acquisition event
      /// </summary>
      public long EventTimestamp { get; set; }
   }
}
