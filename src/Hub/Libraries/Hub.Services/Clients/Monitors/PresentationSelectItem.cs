using Shared.Common;

namespace Hub.Services.Clients.Monitors;

/// <summary>
/// Represenst sensor-to-widget mapping select list item
/// </summary>
public class PresentationSelectItem : BaseEntity
{
   /// <summary>
   /// Sensor identifier
   /// </summary>
   public long SensorId { get; set; }

   /// <summary>
   /// Sensor name
   /// </summary>
   public string SensorName { get; set; }

   /// <summary>
   /// Widget identifier
   /// </summary>
   public long WidgetId { get; set; }

   /// <summary>
   /// Widget name
   /// </summary>
   public string WidgetName { get; set; }

   /// <summary>
   /// Device identifier
   /// </summary>
   public long DeviceId { get; set; }

   /// <summary>
   /// Device name
   /// </summary>
   public string DeviceName { get; set; }
}