using Clients.Dash.Caching;
using Clients.Dash.Configuration;
using Clients.Dash.Infrastructure;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Clients.Widgets;
using Clients.Widgets.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Clients.Dash.Pages.Reports.Trackers.TrackRequest;
using static Clients.Widgets.Core.OpenLayerBase;

namespace Clients.Dash.Pages.Reports.Trackers;

#pragma warning disable CS1591

/// <summary>
/// Component partial class
/// </summary>
public partial class TrackerMap : IAsyncDisposable
{
   #region Parameters

   [Parameter] public bool IsLoading { get; set; }
   [Parameter] public EventCallback<bool> IsLoadingChanged { get; set; }

   #endregion

   #region Services

   [Inject] ISensorRecordService SensorRecordService { get; set; }
   [Inject] WebAssemblyHostConfiguration Config { get; set; }
   [Inject] IStaticCacheManager CacheManager { get; set; }

   #endregion

   #region fields

   private static IEnumerable<MapSelectItem> _mapSelectItems;
   private static MapView _initialView;
   private static MapView _mapView;

   DynamicFilter _filter = new();
   private OpenMap _map;
   private bool _isLoading;
   private int _acceleration;
   private bool _playDisabled;

   #endregion

   #region Ctors

   /// <summary>
   /// Static ctor
   /// </summary>
   static TrackerMap()
   {
      _initialView ??= new()
      {
         Zoom = 8,
         LayerType = LayerTypes.AerialWithLabelsOnDemand,
         Culture = Defaults.Culture,
         Theme = Defaults.Theme,
         Center = new()
         {
            Lon = 103.829658,
            Lat = 1.348738,
            Color = "rgb(255, 99, 132)",
            Height = 100,
            Ticks = DateTime.UtcNow.Ticks,
         }
      };

      if (_mapSelectItems is null)
      {
         using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
         var localizer = scope.ServiceProvider.GetRequiredService<Localizer>();
         _mapSelectItems = new List<MapSelectItem>()
         {
            new(){ MapType = LayerTypes.OSM, Locale = localizer["DeviceMap.Types.Map"].Value },
            new(){ MapType = LayerTypes.AerialWithLabelsOnDemand, Locale = localizer["DeviceMap.Types.Sat"].Value }
         };
      }
   }

   /// <summary>
   /// Default Ctor
   /// </summary>
   public TrackerMap()
   {
      _mapView = _initialView;
      _acceleration = 1;
      _playDisabled = true;
   }

   #endregion

   #region Methods


   public async Task OnRequestChanged(TrackRequestModel request)
   {
      await CacheManager.RemoveByPrefixAsync("TrackerMapPrefix");
      _filter.From = request.From.ToUniversalTime();
      _filter.To = request.To.ToUniversalTime();
      _filter.DeviceId = request.SelectedDevice.Id;
      _filter.SensorId = request.SelectedSensor.Id;

      if (_map != null)
      {
         await ShowTrack();
         await _map.Fit();
      }
   }

   private async Task ShowTrack()
   {
      _isLoading = true;
      await IsLoadingChanged.InvokeAsync(_isLoading);
      await Task.Yield();

      try
      {
         var track = await SensorRecordService.GetTrackAsync(_filter);

         _playDisabled = track.Count < 2;

         if (_map != null)
            await _map.ShowTrack(track);
      }
      catch (Exception ex)
      {
         await ErrorService.HandleError(this, ex, T["Error.DataFetch"]);
      }
      finally
      {
         _isLoading = false;
         if (IsLoadingChanged.HasDelegate)
         {
            await IsLoadingChanged.InvokeAsync(_isLoading);
         }
         else
         {
            StateHasChanged();
         }
      }
   }

   /// <inheritdoc/>
   public async ValueTask DisposeAsync()
   {
      await CacheManager.RemoveByPrefixAsync("TrackerMapPrefix");
   }

   private void SetSegmentColor(OpenLayerBase.GeoPoint[] track)
   {
      for (int i = 0; i < track.Count(); i++)
      {
         var color = track[i].Speed switch
         {
            // blue
            < 10 => "rgba(0, 0, 130, 0.9)",
            >= 10 and < 20 => "rgba(0, 99, 255, 0.9)",
            >= 20 and < 30 => "rgba(0, 160, 255, 0.9)",

            // green
            >= 30 and < 40 => "rgba(0, 123, 0, 0.9)",
            >= 40 and < 50 => "rgba(0, 172, 0, 0.9)",
            >= 50 and < 60 => "rgba(15, 255, 0, 0.9)",

            // yellow
            >= 60 and < 70 => "rgba(255, 255, 0, 0.9)",
            >= 70 and < 80 => "rgba(255, 200, 0, 0.9)",
            >= 80 and < 90 => "rgba(255, 125, 0, 0.9)",

            // red
            >= 90 and < 100 => "rgba(255, 60, 0, 0.9)",
            >= 110 => "rgba(255, 0, 0, 0.9 )",
            _ => "rgba(59, 0, 199, 0.9)"
         };

         track[i].Color = color;
      }
   }

   #endregion
}

#pragma warning restore CS1591
