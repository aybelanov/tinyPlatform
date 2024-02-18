namespace Clients.Dash.Shared.Widgets;

/// <summary>
/// Component partial class
/// </summary>
public partial class HistoryChart
{
   /// <summary>
   /// helper class for history chart
   /// </summary>
   public class HistoryItemModel
   {
      /// <summary>
      /// Sensor value
      /// </summary>
      public double Value { get; set; }

      /// <summary>
      /// Уlapsed time
      /// </summary>
      public double TimeAgo { get; set; }

      /// <summary>
      /// Timestamp of the sensor data acquisition event
      /// </summary>
      public long EventTimestamp { get; set; }
   }
}
