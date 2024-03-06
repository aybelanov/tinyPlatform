namespace Clients.Dash.Domain;

/// <summary>
/// Represents a presentation view
/// </summary>
public class PresentationView
{
   /// <summary>
   /// Presentation identifier
   /// </summary>
   public long Id { get; set; }

   /// <summary>
   /// Presentation name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// View display order
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Sensor
   /// </summary>
   public Sensor Sensor { get; set; }

   /// <summary>
   /// Device
   /// </summary>
   public Device Device { get; set; }

   /// <summary>
   /// Widget
   /// </summary>
   public Widget Widget { get; set; }
}
