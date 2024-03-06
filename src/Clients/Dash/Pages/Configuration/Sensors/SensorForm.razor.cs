using Clients.Dash.Infrastructure.AutoMapper.Extensions;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;


namespace Clients.Dash.Pages.Configuration.Sensors;

/// <summary>
/// Component partial class
/// </summary>
public partial class SensorForm
{
   [Inject] IDeviceService DeviceService { get; set; }

   /// <summary>
   /// Prepare Sensor model from the init model
   /// </summary>
   /// <param name="model">Init model</param>
   /// <returns>Sensor model (async operation)</returns>
   public async Task<SensorModel> PrepareSensorModelAsync(SensorModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      model.Device = await DeviceService.GetByIdAsync(model.DeviceId);

      return model;
   }

   /// <summary>
   /// Prepare sensor model 
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <returns>Sensor model</returns>
   public async Task<SensorModel> PrepareSensorModelAsync(long sensorId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorId, 1);

      var sensor = await SensorService.GetSensorByIdAsync(sensorId);
      var model = sensor.ToModel<SensorModel>();

      model = await PrepareSensorModelAsync(model);

      return model;
   }
}
