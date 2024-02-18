using Clients.Dash.Pages.Configuration.Monitors;
using System;
using System.Collections.Generic;

namespace Clients.Dash.Services.UI;

/// <summary>
/// Menu changed event arguments
/// </summary>
public class MonitorMenuEventArg : EventArgs
{
   /// <summary>
   /// Changed menu collection
   /// </summary>
   public IEnumerable<MonitorModel> Monitors { get; set; }

   /// <summary>
   /// Category ("own" or "shared")
   /// </summary>
   public string Category { get; set; }
}
