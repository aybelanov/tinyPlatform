using Clients.Dash.Caching;
using Clients.Dash.Configuration;
using Clients.Dash.Domain;
using Clients.Dash.Infrastructure;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Clients.Dash.Services.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Clients.Widgets.Core.OpenLayerBase;

namespace Clients.Dash.Pages.Home;

/// <summary>
/// Represents device map on the main page 
/// </summary>
public partial class DeviceMap : IAsyncDisposable, IDisposable
{
   #region Services

   [Inject] PermissionService PermissionService { get; set; }
   [Inject] IDeviceService DeviceService { get; set; }
   [Inject] IStaticCacheManager CacheManager { get; set; }

   #endregion

   #region fields

   private static IEnumerable<MapSelectItem> _mapSelectItems;
   private static MapView _initialView;
   DeviceMapModel Model;
   MapView _mapView;

   #endregion

   #region Ctors

   static DeviceMap()
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
   public DeviceMap()
   {
      _mapView = _initialView;
      Model = new DeviceMapModel()
      {
         ShowBeenRecently = true,
         ShowMobile = true,
         ShowNoActivities = true,
         ShowOffline = true,
         ShowOnline = true,
         ShowStationary = true,
      };
   }

   #endregion

   #region Methods

   private async Task<IEnumerable<Marker>> PrepareMarkersAsync()
   {
      Func<DynamicFilter, Task<IFilterableList<DeviceMapItem>>> getData = await PermissionService.IsAdminModeAsync()
         ? DeviceService.GetAllDeviceMapItemsAsync
         : DeviceService.GetUserDeviceMapItemsAsync;

      var items = (await getData(new DynamicFilter() { UserId = Model?.UserId })).AsEnumerable();
      items = FilterView(items);

      var models = new List<Marker>();
      foreach (var item in items)
      {
         var mapObj = new Marker
         {
            Name = item.Name,
            EntityId = item.Id,
            Lat = item.Lat,
            Lon = item.Lon,
            Icon = "img/" + (item.IsMobile ? "mobile" : "stationary") + "_" + ClientHelper.GetMapMarkerStatusColor(item.Status) + ".svg",
            Visible = GetVisible(item.Status),
            Content = $"<a href='configuration/device/edit/{item.Id}'>{item.Name}</a>",
            TextColor = ClientHelper.GetMapMarkerTextColor(item.Status),
            Link = $"configuration/device/edit/{item.Id}"
         };
         models.Add(mapObj);
      }

      return models;
   }

   private IEnumerable<DeviceMapItem> FilterView(IEnumerable<DeviceMapItem> items)
   {
      if (!Model.ShowStationary)
         items = items.Where(x => x.IsMobile);

      if (!Model.ShowMobile)
         items = items.Where(x => !x.IsMobile);

      if (!items.Any())
         return new List<DeviceMapItem>();

      if (!Model.ShowOnline)
         items = items.Where(x => x.Status != OnlineStatus.Online);

      if (!Model.ShowBeenRecently)
         items = items.Where(x => x.Status != OnlineStatus.BeenRecently);

      if (!Model.ShowOffline)
         items = items.Where(x => x.Status != OnlineStatus.Offline);

      if (!Model.ShowNoActivities)
         items = items.Where(x => x.Status != OnlineStatus.NoActivities);

      return items;
   }
  
   private bool GetVisible(OnlineStatus onlineStatus)
   {
      return onlineStatus switch
      {
         OnlineStatus.Online => Model.ShowOnline,
         OnlineStatus.BeenRecently => Model.ShowBeenRecently,
         OnlineStatus.Offline => Model.ShowOffline,
         OnlineStatus.NoActivities => Model.ShowNoActivities,
         _ => true
      };
   }

   #endregion

   #region Disposing

   /// <summary>
   /// <see href="https://learn.microsoft.com/ru-ru/dotnet/standard/garbage-collection/implementing-disposeasync"/>
   /// </summary>
   public void Dispose()
   {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
   }

   /// <summary>
   /// Disposable implementation
   /// </summary>
   public async ValueTask DisposeAsync()
   {
      await DisposeAsyncCore().ConfigureAwait(false);
      Dispose(disposing: false);
      GC.SuppressFinalize(this);
   }

   /// <summary>
   /// Disposable implementation
   /// </summary>
   protected virtual void Dispose(bool disposing)
   {
      if (disposing)
      {

      }
   }

   /// <summary>
   /// Disposable implementation
   /// </summary>
   protected virtual async ValueTask DisposeAsyncCore()
   {
      await CacheManager.RemoveByPrefixAsync(CacheDefaults<DeviceMapItem>.Prefix);
   }

   #endregion

   #region Nested classes

   /// <summary>
   /// Represents a main device map model
   /// </summary>
   public class DeviceMapModel
   {
      /// <summary>
      /// Show stationary devices on the map
      /// </summary>
      public bool ShowStationary { get; set; }

      /// <summary>
      /// Show mobile devices on the map
      /// </summary>
      public bool ShowMobile { get; set; }

      /// <summary>
      /// Show online device on the map
      /// </summary>
      public bool ShowOnline { get; set; }

      /// <summary>
      /// Show 'been recently' devices on the map
      /// </summary>
      public bool ShowBeenRecently { get; set; }

      /// <summary>
      /// Show offlien devices on the map
      /// </summary>
      public bool ShowOffline { get; set; }

      /// <summary>
      /// Show 'no activities' devices on the map
      /// </summary>
      public bool ShowNoActivities { get; set; }

      /// <summary>
      /// User identifier
      /// </summary>
      public long? UserId { get; set; }

      /// <summary>
      /// Device (map object) collection 
      /// </summary>
      public IEnumerable<Marker> Markers { get; set; }
   }

   #endregion
}
