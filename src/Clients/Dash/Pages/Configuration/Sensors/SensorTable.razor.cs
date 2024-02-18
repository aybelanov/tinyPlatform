using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;


namespace Clients.Dash.Pages.Configuration.Sensors;

/// <summary>
/// Partial component  class
/// </summary>
public partial class SensorTable
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public SensorTable()
   {
      AddButton = AddSensor;
      DeleteButton = OnDeleteItemAsync;
   }

   /// <summary>
   /// Prepare Sensor model by a dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Sensor model collection</returns>
   async Task<IFilterableList<SensorModel>> PrepareSensorModelsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var sensors = await SensorService.GetSensorsAsync(filter);
      var model = Auto.Mapper.Map<FilterableList<SensorModel>>(sensors);
      model.TotalCount = sensors.TotalCount;

      return model;
   }

   Task AddSensor()
   {
      Navigation.NavigateTo($"configuration/sensor/create?device={DeviceId}");
      return Task.CompletedTask;
   }
}
