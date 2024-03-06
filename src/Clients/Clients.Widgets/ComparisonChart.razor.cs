using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clients.Widgets;

[SupportedOSPlatform("browser")]
public partial class ComparisonChart
{
   #region Propertires & Parameters

   /// <summary>
   /// Chart title
   /// </summary>
   [Parameter]
   public string Title { get; set; }

   /// <summary>
   /// Initial show (check boxes) config
   /// </summary>
   [Parameter]
   public ShowConfig InitShowConfig { get; set; }

   /// <summary>
   /// Initial clear chart text
   /// </summary>
   [Parameter]
   public string InitChartText { get; set; }

   [Parameter]
   public EventCallback<int> Callback { get; set; }

   /// <summary>
   /// dotnet instance reference
   /// </summary>
   protected DotNetObjectReference<ComparisonChart> _objRef;

   #endregion

   #region fields

   private bool _isCleaned = true;

   private IJSObjectReference _jsChart;

   private static IJSObjectReference _jsModule;
   private static JSObject _jsHostModule;
   private TaskCompletionSource _jsImportReady = new();

   #endregion

   #region Ctors

   public ComparisonChart() : base()
   {
      InitChartText = "No loaded data yet. Click \"Build\" to load.";
      EmptyText = "No data";
   }

   #endregion

   #region Lifecicle

   //protected override bool ShouldRender() => false;

   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      await base.OnAfterRenderAsync(firstRender);

      if (firstRender)
      {
         _jsHostModule ??= await JSHost.ImportAsync("ComparisonChart.razor.js", "../_content/Clients.Widgets/ComparisonChart.razor.js");
         _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Clients.Widgets/ComparisonChart.razor.js");
         _objRef = DotNetObjectReference.Create(this);
         await _jsModule.InvokeVoidAsync("initChart", Element, _objRef, InitChartText, Culture, Theme);
         _jsImportReady.SetResult();
      }
   }

   #endregion

   #region Methods

   public async Task Clear()
   {
      if (!_isCleaned)
      {

         var showCfg = InitShowConfig is null ? null : JsonSerializer.Serialize(InitShowConfig,
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

         await _jsImportReady.Task;
         ClearChart(Id, InitChartText, Culture, Theme, showCfg);
         _isCleaned = true;
      }
   }

   public async Task Draw(ComprasionChartContext chart)
   {
      ArgumentNullException.ThrowIfNull(chart);

      var meta = JsonSerializer.Serialize(new
      {
         Title,
         Type = "line",
         chart.SeriesName,
         chart.SeriesName2,
         chart.TitleX,
         chart.TitleY,
         chart.TitleY2

      }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

      var series1 = new List<List<object>>();
      if (chart.Series is not null)
      {
         series1.Add(chart.Series.Select(p => p.X).ToList());
         series1.Add(chart.Series.Select(p => p.Y).Cast<object>().ToList());
         series1.Add(chart.Series.Select(p => p.MinY).Cast<object>().ToList());
         series1.Add(chart.Series.Select(p => p.MaxY).Cast<object>().ToList());
      }

      var series2 = new List<List<object>>();
      if (!string.IsNullOrEmpty(chart.SeriesName2) && chart.Series2 != null)
      {
         series2.Add(chart.Series2.Select(x => x.X).ToList());
         series2.Add(chart.Series2.Select(p => p.Y).Cast<object>().ToList());
         series2.Add(chart.Series2.Select(p => p.MinY).Cast<object>().ToList());
         series2.Add(chart.Series2?.Select(p => p.MaxY).Cast<object>().ToList());
      }

      await _jsImportReady.Task;
      DrawChart(Id, meta, series1.Select(x => x.ToArray()).ToArray(), series2.Select(x => x.ToArray()).ToArray());
      _isCleaned = false;
   }

   #endregion

   #region JS methods

   [JSImport("drawChart", "ComparisonChart.razor.js")]
   internal static partial void DrawChart(
      [JSMarshalAs<JSType.String>] string id,
      [JSMarshalAs<JSType.String>] string meta,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] series1,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] series2);

   [JSImport("clearChart", "ComparisonChart.razor.js")]
   internal static partial void ClearChart([JSMarshalAs<JSType.String>] string id,
      [JSMarshalAs<JSType.String>] string emptyText,
      [JSMarshalAs<JSType.String>] string culture,
      [JSMarshalAs<JSType.String>] string theme,
      [JSMarshalAs<JSType.String>] string showCfg);

   #endregion

   #region Disposing

   /// <summary>
   /// <see href="https://learn.microsoft.com/ru-ru/dotnet/standard/garbage-collection/implementing-disposeasync"/>
   /// </summary>
   public override void Dispose()
   {
      Dispose(disposing: true);
      base.Dispose();
      GC.SuppressFinalize(this);
   }

   public override async ValueTask DisposeAsync()
   {
      await DisposeAsyncCore().ConfigureAwait(false);
      await base.DisposeAsync().ConfigureAwait(false);

      Dispose(disposing: false);
      GC.SuppressFinalize(this);
   }

   protected virtual void Dispose(bool disposing)
   {
      if (disposing)
      {
         _objRef?.Dispose();
         _objRef = null;
      }
   }

   protected virtual async ValueTask DisposeAsyncCore()
   {
      if (_jsModule is not null)
         await _jsModule.InvokeVoidAsync("destroyChart", Element).ConfigureAwait(false); ;

      if (_jsChart != null)
      {
         await _jsChart.DisposeAsync().ConfigureAwait(false);
         _jsChart = null;
      }

      _objRef?.Dispose();
      _objRef = null;
   }

   #endregion

   #region Nested classes

   public class ShowConfig
   {
      public bool HideY { get; set; }
      public bool HideMinY { get; set; }
      public bool HideMaxY { get; set; }
      public bool HideY2 { get; set; }
      public bool HideMinY2 { get; set; }
      public bool HideMaxY2 { get; set; }
   }

   public class Point
   {
      public object X { get; set; }
      public double Y { get; set; }
      public double MinY { get; set; }
      public double MaxY { get; set; }
   }

   public class ComprasionChartContext
   {
      public string ChartTitle { get; set; }
      public string TitleX { get; set; }
      public string TitleY { get; set; }
      public string TitleY2 { get; set; }
      public string SeriesName { get; set; }
      public string SeriesName2 { get; set; }
      //public int? MinY => Series is null ? null : Convert.ToInt32(Math.Round(Series.Min(x => x.Y), 0, MidpointRounding.ToNegativeInfinity));
      //public int? MinY2 => Series2 is null ? null : Convert.ToInt32(Math.Round(Series2.Min(x => x.Y), 0, MidpointRounding.ToNegativeInfinity));
      //public int? MaxY => Series is null ? null : Convert.ToInt32(Math.Round(Series.Max(x => x.Y), 0, MidpointRounding.ToNegativeInfinity));
      //public int? MaxY2 => Series2 is null ? null : Convert.ToInt32(Math.Round(Series2.Max(x => x.Y), 0, MidpointRounding.ToNegativeInfinity));
      public IEnumerable<Point> Series { get; set; }
      public IEnumerable<Point> Series2 { get; set; }
   }

   #endregion
}

