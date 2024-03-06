using Clients.Widgets.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Clients.Widgets;

public partial class OpenMap : OpenLayerBase
{
   #region Parameters

   [Parameter]
   public RenderFragment Header { get; set; }

   [Parameter]
   public string ScrollbarId { get; set; }

   [Parameter]
   public string PlayButtonId { get; set; }

   [Parameter]
   public string StopButtonId { get; set; }

   [Parameter]
   public string FitButtonId { get; set; }

   [Parameter]
   public string ClearButtonId { get; set; }

   #endregion

   #region fields

   public Task Ready => _jsImportReady.Task;

   [Inject] NavigationManager Navigation { get; set; }

   private IJSObjectReference _olMap;
   protected DotNetObjectReference<OpenMap> _objRef;

   private static IJSObjectReference _jsModule;
   private static JSObject _jsHostModule;
   private TaskCompletionSource _jsImportReady = new();

   #endregion

   #region Ctors

   /// <summary>
   /// 
   /// </summary>
   static OpenMap()
   {

   }

   /// <summary>
   /// Default Ctor
   /// </summary>
   public OpenMap() : base()
   {

   }

   #endregion

   #region Lifecicle

   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      if (firstRender)
      {
         _jsHostModule ??= await JSHost.ImportAsync("OpenMap.razor.js", "../_content/Clients.Widgets/OpenMap.razor.js");
         _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Clients.Widgets/OpenMap.razor.js");
         _objRef = DotNetObjectReference.Create(this);
         await _jsModule.InvokeVoidAsync("initMap", Element, _objRef, new { View, ScrollbarId, PlayButtonId, StopButtonId, FitButtonId, ApiKey = BingKey });
         _jsImportReady.SetResult();
      }
      await base.OnAfterRenderAsync(firstRender);
   }

   #endregion

   #region Methods

   public Task Fit()
   {
      Fit(Id);
      return Task.CompletedTask;
   }

   public async Task ShowTrack(IEnumerable<GeoPoint> track)
   {
      if (track is not null)
      {
         var data = new List<object>()
         {
            track.Select(x=>(double)x.Ticks).ToArray(),
            track.Select(x=>x.Lon).ToArray(),
            track.Select(x=>x.Lat).ToArray(),
            track.Select(x=>x.Speed).ToArray(),
            track.Select(x=>x.Course).ToArray(),

         }.ToArray();

         await _jsImportReady.Task;
         ShowTrack(Id, data.ToArray());
      }
   }

   public async Task UpdateTrack(IEnumerable<GeoPoint> track)
   {
      if (track is not null)
      {
         var data = new List<object>()
         {
            track.Select(x=>(double)x.Ticks).ToArray(),
            track.Select(x=>x.Lon).ToArray(),
            track.Select(x=>x.Lat).ToArray(),
            track.Select(x=>x.Speed).ToArray(),
            track.Select(x=>x.Course).ToArray(),

         }.ToArray();

         await _jsImportReady.Task;
         UpdateOnlineTrack(Id, data);
      }
   }

   public async Task ShowMarkers(IEnumerable<Marker> markers)
   {
      if (markers is not null)
      {
         var data = new List<object>()
         {
            markers.Select(x=>(double)x.EntityId).ToArray(),
            markers.Select(x=>x.Icon).ToArray(),
            markers.Select(x=>x.Name).ToArray(),
            markers.Select(x=>x.Content).ToArray(),
            markers.Select(x=>x.Lon).ToArray(),
            markers.Select(x=>x.Lat).ToArray(),
            markers.Select(x=>x.Visible).Cast<object>().ToArray(),
            markers.Select(x=>x.TextColor).ToArray(),
            markers.Select(x=>x.Link).ToArray()

         }.ToArray();

         await _jsImportReady.Task;
         ShowMarkers(Id, data);
      }
   }

   public async Task StartAnimation()
   {
      StartTrackAnimation(Id);
      await Task.CompletedTask;
   }

   public async Task StopAnimation()
   {
      StopTrackAnimation(Id);
      await Task.CompletedTask;
   }

   public async Task PauseAnimation()
   {
      PauseTrackAnimation(Id);
      await Task.CompletedTask;
   }

   public void SetPlayerSpeed(int speed)
   {
      SetPlayerSpeed(Id, speed);
   }

   [JSInvokable]
   public void MapLink(string link) => Navigation.NavigateTo(link);

   [JSInvokable]
   public void ScrollCallback(double distance)
   {

   }

   public void ChangeMapLayer(LayerTypes newLayer) => ChangeMapLayer(Id, newLayer.ToString());

   [JSImport("changeLayer", "OpenMap.razor.js")]
   internal static partial void ChangeMapLayer(string containerId, string layer);

   [JSImport("showObjects", "OpenMap.razor.js")]
   internal static partial void ShowMarkers(
      [JSMarshalAs<JSType.String>] string containerId,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] data);

   [JSImport("showPath", "OpenMap.razor.js")]
   internal static partial void ShowTrack(
      [JSMarshalAs<JSType.String>] string containerId,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] data);

   [JSImport("updateOnlinePath", "OpenMap.razor.js")]
   internal static partial void UpdateOnlineTrack(
      [JSMarshalAs<JSType.String>] string containerId,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] data);

   [JSImport("showAllOnMap", "OpenMap.razor.js")]
   internal static partial void Fit(string containerId);

   [JSImport("startTrackAnimation", "OpenMap.razor.js")]
   internal static partial void StartTrackAnimation(
      [JSMarshalAs<JSType.String>] string containerId);

   [JSImport("stopTrackAnimation", "OpenMap.razor.js")]
   internal static partial void StopTrackAnimation(
      [JSMarshalAs<JSType.String>] string containerId);

   [JSImport("pauseTrackAnimation", "OpenMap.razor.js")]
   internal static partial void PauseTrackAnimation(
      [JSMarshalAs<JSType.String>] string containerId);

   [JSImport("setPlayerSpeed", "OpenMap.razor.js")]
   internal static partial void SetPlayerSpeed(
      [JSMarshalAs<JSType.String>] string containerId,
      [JSMarshalAs<JSType.Number>] int speedValue);

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
         await _jsModule.InvokeVoidAsync("destroyOlMap", Element).ConfigureAwait(false); ;

      if (_olMap != null)
      {
         await _olMap.DisposeAsync().ConfigureAwait(false);
         _olMap = null;
      }

      _objRef?.Dispose();
      _objRef = null;
   }

   #endregion
}