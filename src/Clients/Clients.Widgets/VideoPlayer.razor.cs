using Clients.Widgets.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Clients.Widgets;

public partial class VideoPlayer : VideojsBase
{
   #region Parameters

   [Parameter]
   public RenderFragment Header { get; set; }

   [Parameter]
   public string TokenKey { get; set; }

   [Parameter]
   public bool IsLive { get; set; }

   [Parameter, EditorRequired]
   public string Endpoint { get; set; }

   #endregion

   #region fields

   private IJSObjectReference _player;
   protected DotNetObjectReference<VideoPlayer> _objRef;

   private static IJSObjectReference _jsModule;
   private static JSObject _jsHostModule;
   private TaskCompletionSource _jsImportReady = new();

   #endregion

   #region Ctors

   /// <summary>
   /// Default Ctor
   /// </summary>
   public VideoPlayer() : base()
   {

   }

   #endregion

   #region Lifecycle

   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      if (firstRender)
      {
         _jsHostModule ??= await JSHost.ImportAsync("VideoPlayer.razor.js", "../_content/Clients.Widgets/VideoPlayer.razor.js");
         _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Clients.Widgets/VideoPlayer.razor.js");
         _objRef = DotNetObjectReference.Create(this);
         await _jsModule.InvokeVoidAsync("initPlayer", Element, _objRef, Culture, Theme, TokenKey, Endpoint, IsLive);
         _jsImportReady.SetResult();

      }

      await base.OnAfterRenderAsync(firstRender);
   }

   #endregion

   #region Methods

   public async Task Play(IEnumerable<Segment> segments)
   {
      await _jsImportReady.Task;

      if (segments?.Any() != true)
         return;

      var targetDuration = (int)Math.Round(segments.Max(x => x.Extinf), 0, MidpointRounding.ToPositiveInfinity);

      var res = new List<object>()
      {
         segments.Select(x=>x.Extinf).ToArray(),
         segments.Select(x=>x.SegmentName).ToArray(),

      }.ToArray();


      Play(Id, res, targetDuration);
   }

   public async Task Update(IEnumerable<Segment> segments)
   {
      await _jsImportReady.Task;

      if (!(segments?.Any() ?? false))
         return;

      var targetDuration = (int)Math.Round(segments.Max(x => x.Extinf), 0, MidpointRounding.ToPositiveInfinity);

      var res = new List<object>()
      {
         segments.Select(x=>x.Extinf).ToArray(),
         segments.Select(x=>x.SegmentName).ToArray(),

      }.ToArray();


      Update(Id, res, targetDuration);
   }

   public async Task Pause()
   {
      await _jsImportReady.Task;
      Pause(Id);
   }

   public async Task Stop()
   {
      await _jsImportReady.Task;
      Stop(Id);
   }

   [JSImport("play", "VideoPlayer.razor.js")]
   internal static partial void Play(
      [JSMarshalAs<JSType.String>] string containerId,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] segments,
      [JSMarshalAs<JSType.Number>] int targetDuration);

   [JSImport("updatePlaylist", "VideoPlayer.razor.js")]
   internal static partial void Update(
     [JSMarshalAs<JSType.String>] string containerId,
     [JSMarshalAs<JSType.Array<JSType.Any>>] object[] segments,
     [JSMarshalAs<JSType.Number>] int targetDuration);

   [JSImport("stop", "VideoPlayer.razor.js")]
   internal static partial void Stop([JSMarshalAs<JSType.String>] string containerId);

   [JSImport("pause", "VideoPlayer.razor.js")]
   internal static partial void Pause([JSMarshalAs<JSType.String>] string containerId);

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
         await _jsModule.InvokeVoidAsync("destroyPlayer", Element).ConfigureAwait(false);

      if (_player != null)
      {
         await _player.DisposeAsync().ConfigureAwait(false);
         _player = null;
      }

      _objRef?.Dispose();
      _objRef = null;
   }

   #endregion

   #region nested classes

   public class Segment
   {
      /// <summary>
      /// Entity identifier
      /// </summary>
      public long Id { get; set; }

      /// <summary>
      /// Video segment duration [#EXTINF]
      /// </summary>
      /// <remarks>
      /// <see href="https://datatracker.ietf.org/doc/html/rfc8216#section-4.3.2.1"/>
      /// </remarks>
      public double Extinf { get; set; }

      /// <summary>
      /// Segment file name
      /// </summary>
      public string SegmentName { get; set; }

      /// <summary>
      /// IP cam identifier
      /// </summary>
      public long IpcamId { get; set; }

      /// <summary>
      /// Segment creation datetime UTC
      /// </summary>
      public DateTime OnCreatedUtc { get; set; }

      /// <summary>
      /// Segment receiver datetime UTC
      /// </summary>
      public DateTime OnReceivedUtc { get; set; }
   }

   public class VideoRequest
   {
      /// <summary>
      /// Device identifier
      /// </summary>
      public long? DeviceId { get; set; }

      /// <summary>
      /// Sensor identifier
      /// </summary>
      public long? IpcamId { get; set; }

      /// <summary>
      /// Date time from in seconds of UNIX epoch
      /// </summary>
      public long? From { get; set; }

      /// <summary>
      /// Date time to in seconds of UNIX epoch
      /// </summary>
      public long? To { get; set; }
   }

   /// <summary>
   /// Standard video resolutions
   /// </summary>
   public enum VideoResolution
   {
      /// <summary>
      /// 640x480
      /// </summary>
      SD4x3,

      /// <summary>
      /// 640x360
      /// </summary>
      SD16x9,

      /// <summary>
      /// _1280x720
      /// </summary>
      p720,

      /// <summary>
      /// _1920x1080,
      /// </summary>
      FullHD,


      /// <summary>
      /// 2560x1440, 
      /// </summary>
      _2K,

      /// <summary>
      /// 3480x2160
      /// </summary>
      _4K,


      /// <summary>
      /// 7680x4320,
      /// </summary>
      _8K
   }

   #endregion
}