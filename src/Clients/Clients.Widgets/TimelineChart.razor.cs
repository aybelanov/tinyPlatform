using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clients.Widgets;

[SupportedOSPlatform("browser")]
public partial class TimelineChart
{
   #region 

   [Parameter]
   public ChartType Type { get; set; }

   #endregion

   #region Services

   [Inject] IStringLocalizer<TimelineChart> T { get; set; }

   #endregion

   #region fields

   private bool _isCumulative;
   private IJSObjectReference _chartJs;
   protected DotNetObjectReference<TimelineChart> _objRef;

   private static IJSObjectReference _jsModule;
   private static JSObject _jsHostModule;
   private TaskCompletionSource _jsImportReady = new();

   #endregion

   #region Methods

   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      await base.OnAfterRenderAsync(firstRender);

      if (firstRender)
      {
         _jsHostModule ??= await JSHost.ImportAsync("TimelineChart.razor.js", "../_content/Clients.Widgets/TimelineChart.razor.js");
         _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Clients.Widgets/TimelineChart.razor.js");
         _objRef = DotNetObjectReference.Create(this);
         await _jsModule.InvokeVoidAsync("initChart", Element, _objRef, EmptyText, Culture, Theme);
         _jsImportReady.SetResult();
      }
   }


   public async Task Update(ChangeEventArgs args) 
   {
      _isCumulative = Convert.ToBoolean(args.Value);
      await _jsImportReady.Task;
      Update(Id, _isCumulative); 
   }

   public async Task Draw(IEnumerable<Point> series)
   {

      var config = JsonSerializer.Serialize(new 
      {
         SeriesColor = "rgb(54, 162, 235, 0.75)",
         AreaColor = "rgb(54, 162, 235, 0.5)",
         IsCumulative = _isCumulative,
         SeriesName = T["RecordsCount"].Value,

      }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

      var unixEpoche = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
      var xValues = series.Select(x => (double)(x.Moment - unixEpoche) / TimeSpan.TicksPerMillisecond).ToArray();
      var yValues = series.Select(x => x.Value).ToArray();
      
      await _jsImportReady.Task;
      Draw(Id, config, xValues, yValues);
   }

   public async Task Clear()
   {
      await _jsImportReady.Task;
      Clear(Id);
   }

   #endregion

   #region JsInterop

   [JSImport("drawChart", "TimelineChart.razor.js")]
   internal static partial void Draw(
      [JSMarshalAs<JSType.String>] string id,
      [JSMarshalAs<JSType.String>] string config, 
      [JSMarshalAs<JSType.Array<JSType.Number>>] double[] xValues,
      [JSMarshalAs<JSType.Array<JSType.Number>>] double[] yValues);

   [JSImport("clearChart", "TimelineChart.razor.js")]
   internal static partial void Clear(
      [JSMarshalAs<JSType.String>] string id);

   [JSImport("updateChart", "TimelineChart.razor.js")]
   internal static partial void Update(
      [JSMarshalAs<JSType.String>] string id,
      [JSMarshalAs<JSType.Boolean>] bool cumulativeSum);

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
         await _jsModule.InvokeVoidAsync("destroyChart", Element).ConfigureAwait(false);

      if (_chartJs != null)
      {
         await _chartJs.DisposeAsync().ConfigureAwait(false);
         _chartJs = null;
      }

      _objRef?.Dispose();
      _objRef = null;
   }

   #endregion

   #region Nested classes 

   public enum ChartType
   {
      Line,
      Area
   }

   public record Point
   {
      public long Moment { get; set; }
      public double Value { get; set; }
   }

   #endregion
}
