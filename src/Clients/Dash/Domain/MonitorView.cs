using System.Collections.Generic;

namespace Clients.Dash.Domain;

/// <summary>
/// Represent a monitor view class
/// </summary>
public class MonitorView : Monitor
{
   /// <summary>
   /// Presentation collection
   /// </summary>
   public IList<PresentationView> Presentations { get; set; }
}
