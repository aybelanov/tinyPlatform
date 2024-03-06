using Clients.Dash.Configuration;
using Clients.Dash.Domain;
using Clients.Dash.Infrastructure;
using Clients.Dash.Pages.Monitors;
using Clients.Dash.Services.EntityServices;
using Clients.Widgets;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static Clients.Dash.Pages.Configuration.Widgets.AdjustLiveTracker;
using static Clients.Widgets.Core.OpenLayerBase;

namespace Clients.Dash.Shared.Widgets;

/// <summary>
/// Component partial class
/// </summary>
public partial class LiveTracker
{
   [CascadingParameter] PresentationViewModel Presentation { get; set; }

   [Inject] WebAssemblyHostConfiguration Config { get; set; }
   [Inject] ISensorRecordService SensorRecordService { get; set; }

   OpenMap _map;
   MapView _mapView;
   IEnumerable<MapSelectItem> _mapSelectItems;
   List<GeoPoint> _track = new();
   LiveTrackerAdjustment _adjustment;

   /// <summary>
   /// Default ctor
   /// </summary>
   public LiveTracker()
   {

   }

   /// <inheritdoc/>
   protected override void OnInitialized()
   {
      _adjustment = string.IsNullOrEmpty(Presentation.Widget.Adjustment)
     ? new LiveTrackerAdjustment()
     : JsonSerializer.Deserialize<LiveTrackerAdjustment>(Presentation.Widget.Adjustment);

      _mapView = new MapView()
      {
         Center = new() { Lat = Presentation.Device.Lat, Lon = Presentation.Device.Lon },
         Zoom = 8,
         LayerType = LayerTypes.AerialWithLabelsOnDemand,
         Culture = Defaults.Culture,
         Theme = Defaults.Theme,
      };

      _mapSelectItems = new List<MapSelectItem>()
      {
         new(){ MapType = LayerTypes.OSM, Locale = T["DeviceMap.Types.Map"].Value },
         new(){ MapType = LayerTypes.AerialWithLabelsOnDemand, Locale = T["DeviceMap.Types.Sat"].Value }
      };

      base.OnInitialized();
   }

   /// <inheritdoc/>
   protected override async Task OnInitializedAsync()
   {
      try
      {
         var lastPoint = await SensorRecordService.GetLastRecordAsync(Presentation.Sensor.Id);

         if (lastPoint != null)
         {
            _track.Add(lastPoint);
            await _map?.UpdateTrack(_track);
         }
      }
      catch (Exception ex)
      {
         await ErrorService.HandleError(this, ex, T["Error.DataFetch"]);
      }
   }

   void MapOnChange(object newLayer) => _map?.ChangeMapLayer((LayerTypes)newLayer);

   /// <inheritdoc/>
   public override async Task Update(IEnumerable<SensorRecord> records)
   {
      foreach (var record in records)
      {
         var point = ClientHelper.ParseGeoRecord(record);

         var historyPoint = _adjustment.HistoryPointsCount < 1 ? 10 : _adjustment.HistoryPointsCount;

         if (_track.Count >= historyPoint && _track.Count > 0)
            _track.RemoveAt(0);

         _track.Add(point);
      }

      _track = _track.OrderBy(x => x.Ticks).ToList();
      await _map.UpdateTrack(_track);
   }
}
