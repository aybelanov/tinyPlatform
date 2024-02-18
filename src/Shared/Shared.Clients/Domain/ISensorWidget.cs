namespace Shared.Clients.Domain;

/// <summary>
/// Sensor to sensor presentation mapping entity interface
/// </summary>
public interface ISensorWidget
{
   /// <summary>
   /// Get or set a sensor identifier
   /// </summary>
   long SensorId { get; set; }

   /// <summary>
   /// Get or set a sensor presentation indentifier
   /// </summary>
   long WidgetId { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   int DisplayOrder { get; set; }
}