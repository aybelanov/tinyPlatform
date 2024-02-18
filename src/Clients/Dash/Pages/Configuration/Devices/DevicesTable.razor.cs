using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Infrastructure;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Configuration.Devices;


/// <summary>
/// Component partial class
/// </summary>
public partial class DevicesTable
{
   [Inject] IStaticCacheManager StaticCacheManager { get; set; }

   /// <summary>
   /// Prepare device model with method routing for grid by a dynamic filter
   /// </summary>
   /// <returns>Device model collection (async operation)</returns>
   public async Task<FilterableList<DeviceModel>> PrepareDevicesModelAsync(DynamicFilter filter)
   {      
      Func<DynamicFilter, Task<IFilterableList<Device>>> getData = await PermissionService.IsAdminModeAsync()
         ? Service.GetAllDevicesAsync
         : Service.GetOwnDevicesAsync;

      var devices = await getData(filter);
      var models = Auto.Mapper.Map<FilterableList<DeviceModel>>(devices);
      models.TotalCount = devices.TotalCount;

      foreach (var model in models)
      {
         if (model.LastActivityOnUtc != null && model.ConnectionStatus != OnlineStatus.Online)
            model.LastActivityString = ClientHelper.ConvertUtcToBrowserTime(model.LastActivityOnUtc.Value).ToString("g");
      }

      return models;
   }

   /// <summary>
   /// Prepare own device model for grid by a dynamic filter
   /// </summary>
   /// <returns>Device model collection (async operation)</returns>
   public async Task<FilterableList<DeviceModel>> PrepareOwnDevicesModelAsync(DynamicFilter filter)
   {
      var devices = await Service.GetOwnDevicesAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<DeviceModel>>(devices);
      models.TotalCount = devices.TotalCount;

      foreach (var model in models)
      {
         if (model.LastActivityOnUtc != null && model.ConnectionStatus != OnlineStatus.Online)
            model.LastActivityString = ClientHelper.ConvertUtcToBrowserTime(model.LastActivityOnUtc.Value).ToString("g");
      }

      return models;
   }
}
