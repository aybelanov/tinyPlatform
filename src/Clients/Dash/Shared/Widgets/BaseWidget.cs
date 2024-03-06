using Clients.Dash.Domain;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Widgets;

/// <summary>
/// Represents a base client widget class
/// </summary>
public abstract class BaseWidget : ComponentBase, IWidgetComponent
{
   /// <summary>
   /// Component unmach attributes
   /// </summary>
   [Parameter(CaptureUnmatchedValues = true)]
   public Dictionary<string, object> Attributes { get; set; }

   /// <summary>
   /// Updates a widget
   /// </summary>
   /// <param name="records"></param>
   /// <returns></returns>
   public virtual Task Update(IEnumerable<SensorRecord> records)
   {
      return Task.CompletedTask;
   }
}
