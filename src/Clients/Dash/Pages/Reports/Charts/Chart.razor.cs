using Clients.Dash.Services.EntityServices;
using Clients.Widgets;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Clients.Dash.Pages.Reports.Charts.ChartRequest;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Reports.Charts;

/// <summary>
/// Chart
/// </summary>
public partial class Chart
{
   [Inject] ISensorRecordService SensorRecordService { get; set; }

   /// <summary>
   /// Prepares chart series
   /// </summary>
   /// <param name="requestModel">Chart request model</param>
   /// <returns>Chart series</returns>
   protected async Task<ComparisonChart.ComprasionChartContext> PrepareChartSeriesAsync(ChartRequestModel requestModel)
   {
      var request = Auto.Mapper.Map<global::Shared.Clients.ChartRequest>(requestModel);
      var series = await SensorRecordService.GetChartSeriesAsync(request);
      var chartContext = new ComparisonChart.ComprasionChartContext();

      var first = requestModel.SelectedSensors.First();
      chartContext.SeriesName = first.Name;
      chartContext.TitleY = first.MeasureUnit;

      chartContext.Series = series.FirstOrDefault(x => x.EntityId == first.Id)?.Data
      .Select(x => new ComparisonChart.Point()
      {
         X = TicksToUnixTimestamp(x.X),
         Y = x.Y,
         MinY = x.MinY,
         MaxY = x.MaxY

      }).OrderBy(x => x.X).ToList();

      var last = requestModel.SelectedSensors.Last();
      if (last.Id != first.Id)
      {
         chartContext.SeriesName2 = last.Name;
         chartContext.TitleY2 = last.MeasureUnit;
         chartContext.Series2 = series.FirstOrDefault(x => x.EntityId == last.Id)?.Data
         .Select(x => new ComparisonChart.Point()
         {
            X = TicksToUnixTimestamp(x.X),
            Y = x.Y,
            MinY = x.MinY,
            MaxY = x.MaxY

         }).OrderBy(x => x.X).ToList();
      }

      return chartContext;

      // in seconds
      static int TicksToUnixTimestamp(double ticksvalue)
      {
         var tiks = Convert.ToInt64(Math.Round(ticksvalue, 0, MidpointRounding.ToEven));
         var epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
         var res = Convert.ToInt32((tiks - epochTicks) / TimeSpan.TicksPerSecond);
         return res;
      }
   }
}
