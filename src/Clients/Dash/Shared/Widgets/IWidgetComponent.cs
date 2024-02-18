using Clients.Dash.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Widgets;

/// <summary>
/// Widget component interface
/// </summary>
public interface IWidgetComponent
{
   /// <summary>
   /// Updates widget
   /// </summary>
   /// <param name="records">Sensor record collection</param>
   /// <returns></returns>
   Task Update(IEnumerable<SensorRecord> records);
}
