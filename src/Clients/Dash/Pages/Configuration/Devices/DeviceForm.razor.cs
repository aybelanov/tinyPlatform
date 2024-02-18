using System.Threading.Tasks;
using System;
using Clients.Dash.Infrastructure.AutoMapper.Extensions;

namespace Clients.Dash.Pages.Configuration.Devices;

/// <summary>
/// Component partial class
/// </summary>
public partial class DeviceForm
{
   /// <summary>
   /// Prepare a device model by device id
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Device model (async operation)</returns>
   public async Task<DeviceModel> PrepareModelAsync(long deviceId)
   {
      var device = await DeviceService.GetByIdAsync(deviceId)
         ?? throw new Exception("Access denied or the device does not exist.");

      var model = device.ToModel<DeviceModel>();

      return model;
   }

   /// <summary>
   /// Prepare a device model
   /// </summary>
   /// <returns>Device model</returns>
   public Task<DeviceModel> PrepareModelAsync(DeviceModel model)
   {
      model ??= new DeviceModel();
      return Task.FromResult(model);
   }
}
