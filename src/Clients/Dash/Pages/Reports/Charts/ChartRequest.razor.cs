using Clients.Dash.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.Dash.Pages.Reports.Charts;

/// <summary>
/// Represents a request data for charts rendering
/// </summary>
public partial class ChartRequest
{
   #region Methods

   /// <summary>
   /// Prepares chart request model
   /// </summary>
   /// <returns>Chart request model</returns>
   protected async Task<ChartRequestModel> PrepareChartRequestModelAsync()
   {
      var now = DateTime.UtcNow;
      var model = new ChartRequestModel();
      model.To = now.ToLocalTime();
      model.From = model.To.AddDays(-1);

      return await Task.FromResult(model);
   }

   #endregion

   #region Model

   /// <summary>
   /// Represents a chart request model
   /// </summary>
   public class ChartRequestModel //: ChartRequest
   {
      /// <summary>
      /// Date time "from"
      /// </summary>
      public DateTime From { get; set; }

      /// <summary>
      /// Date time "to"
      /// </summary>
      public DateTime To { get; set; }

      /// <summary>
      /// Chart html-element width on the screen in pixels
      /// </summary>
      public int ChartWidth { get; set; }

      /// <summary>
      /// Selected device
      /// </summary>
      public DeviceSelectItem SelectedDevice { get; set; }

      /// <summary>
      /// Sensor ids
      /// </summary>
      public IEnumerable<long> SensorIds => SelectedSensors?.Select(x => x.Id);

      /// <summary>
      /// Selected sensors
      /// </summary>
      public IEnumerable<SensorSelectItem> SelectedSensors { get; set; }
   }

   #endregion
}