﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Clients.Widgets;

[SupportedOSPlatform("browser")]
public partial class StreamChart
{
   #region Parameters

   [Parameter] public ChartType Type { get; set; }
   [Parameter] public bool ShowCumulative { get; set; }
   [Parameter] public string MeasureUnit { get; set; }
   [Parameter] public double MinValue { get; set; }
   [Parameter] public double MaxValue { get; set; }
   [Parameter] public double Duration { get; set; }
   [Parameter] public double Delay { get; set; }
   [Parameter] public bool Smooth { get; set; }
   [Parameter] public string PointStyle { get; set; }

   #endregion

   #region Services

   [Inject] IStringLocalizer<TimelineChart> T { get; set; }

   #endregion

   #region fields

   private bool _isCumulative;
   private IJSObjectReference _chartJs;
   protected DotNetObjectReference<StreamChart> _objRef;

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
         _jsHostModule ??= await JSHost.ImportAsync("StreamChart.razor.js", "../_content/Clients.Widgets/StreamChart.razor.js");
         _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Clients.Widgets/StreamChart.razor.js");
         _objRef = DotNetObjectReference.Create(this);

         await _jsModule.InvokeVoidAsync("initChart",
            Element,
            _objRef,
            EmptyText,
            Culture,
            Theme,
            Type.ToString(),
            MeasureUnit,
            MinValue,
            MaxValue,
            Duration,
            Delay,
            Smooth,
            PointStyle);

         _jsImportReady.SetResult();
      }
   }

   public async Task ShowCumulativeSum(ChangeEventArgs args)
   {
      // TODO show cummulative sum
      await _jsImportReady.Task;
      //Update(Id);
   }

   public async Task Update(IEnumerable<Point> series)
   {
      var xValues = series.Select(x => x.Moment).ToArray();
      var yValues = series.Select(x => x.Value).ToArray();

      await _jsImportReady.Task;
      Update(Id, xValues, yValues);
   }

   public async Task Clear()
   {
      await _jsImportReady.Task;
      Clear(Id);
   }

   #endregion

   #region JsInterop

   [JSImport("onReceive", "StreamChart.razor.js")]
   internal static partial void Update(
      [JSMarshalAs<JSType.String>] string id,
      [JSMarshalAs<JSType.Array<JSType.Number>>] double[] xValues,
      [JSMarshalAs<JSType.Array<JSType.Number>>] double[] yValues);

   [JSImport("clearChart", "StreamChart.razor.js")]
   internal static partial void Clear(
      [JSMarshalAs<JSType.String>] string id);

   [JSImport("updateChart", "StreamChart.razor.js")]
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
      /// <summary>
      /// Date time in javascript ticks (unix epoch milliseconds)
      /// </summary>
      public double Moment { get; set; }

      /// <summary>
      /// Current value
      /// </summary>
      public double Value { get; set; }
   }

   #endregion
}
