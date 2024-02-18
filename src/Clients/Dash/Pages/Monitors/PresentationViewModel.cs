using Clients.Dash.Domain;

namespace Clients.Dash.Pages.Monitors;

/// <summary>
/// Representa a presentation view model
/// </summary>
public class PresentationViewModel
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
   /// Presentation description
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
   /// Device identifier
   /// </summary>
   public Device Device { get; set; }

   /// <summary>
   /// Widget
   /// </summary>
   public Widget Widget { get; set; }
}
