using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
