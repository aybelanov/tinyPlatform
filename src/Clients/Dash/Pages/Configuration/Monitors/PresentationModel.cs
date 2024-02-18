using Clients.Dash.Domain;
using Clients.Dash.Models;
using Shared.Clients.Domain;

namespace Clients.Dash.Pages.Configuration.Monitors;

/// <summary>
/// Represents a monitor presentation (sensor-to-widget) model
/// </summary>
public class PresentationModel : BaseEntityModel
{
   /// <summary>
   /// Monitor identifier
   /// </summary>
   public long MonitorId { get; set; }

   /// <summary>
   /// Get or set sensor-to-widget identifier
   /// </summary>
   public long SensorWidgetId { get; set; }

   /// <summary>
   /// Monitor name
   /// </summary>
   public string MonitorName { get; set; }

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

   /// <summary>
   /// Presentation name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Presentation description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// Display order
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Presentation select item
   /// </summary>
   public PresentationSelectItem PresentationSelectItem { get; set; }
}
