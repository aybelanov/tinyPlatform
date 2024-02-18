using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Configuration.Monitors;

/// <summary>
/// Represents a monitor table 
/// </summary>
public partial class MonitorTable
{
   [Inject] IStaticCacheManager StaticCacheManager { get; set; }
   [Inject] private IMonitorService MonitorService { get; set; }

   /// <summary>
   /// Prepare a model for the all monitor grid (admins only) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Model (a collection of the monitor model entities)</returns>
   public async Task<IFilterableList<MonitorModel>> PrepareMonitorsModelAsync(DynamicFilter filter)
   {

      Func<DynamicFilter, Task<IFilterableList<Monitor>>> getData = await AdminService.IsAdminModeAsync()
         ? MonitorService.GetAllMonitorsAsync
         : MonitorService.GetOwnMonitorsAsync;

      var monitors = await getData(filter);
      var model = Auto.Mapper.Map<FilterableList<MonitorModel>>(monitors);
      model.TotalCount = monitors.TotalCount;

      return model;
   }

   /// <summary>
   /// Prepare a model for the monitor grid by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Model (a collection of the monitor model entities)</returns>
   public async Task<IFilterableList<MonitorModel>> PrepareOwnMonitorsModelAsync(DynamicFilter filter)
   {
      var monitors = await MonitorService.GetOwnMonitorsAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<MonitorModel>>(monitors);
      models.TotalCount = monitors.TotalCount;

      return models;
   }
}
