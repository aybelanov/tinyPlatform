using Clients.Dash.Infrastructure;
using Shared.Clients;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;


namespace Clients.Dash.Pages.Configuration.Devices;


/// <summary>
/// Component partial class
/// </summary>
public partial class SharedDeviceTable
{
   /// <summary>
   /// Prepare shared device model for grid by a dynamic filter
   /// </summary>
   /// <returns>Device model collection (async operation)</returns>
   public async Task<FilterableList<DeviceModel>> PrepareSharedDevicesModelAsync(DynamicFilter filter)
   {
      var devices = await Service.GetSharedDevicesAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<DeviceModel>>(devices);
      models.TotalCount = devices.TotalCount;

      foreach (var model in models)
      {
         if (model.LastActivityOnUtc != null)
            model.LastActivityString = ClientHelper.ConvertUtcToBrowserTime(model.LastActivityOnUtc.Value).ToString("g");
      }

      return models;
   }
}
