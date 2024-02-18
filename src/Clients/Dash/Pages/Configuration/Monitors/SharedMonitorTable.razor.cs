using Shared.Clients;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;


namespace Clients.Dash.Pages.Configuration.Monitors;

/// <summary>
/// Represents shared monitors table component
/// </summary>
public partial class SharedMonitorTable
{

   /// <summary>
   /// Prepare a model for the monitor grid by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Model (a collection of the monitor model entities)</returns>
   public async Task<IFilterableList<MonitorModel>> PrepareSharedMonitorsModelAsync(DynamicFilter filter)
   {
      var monitors = await MonitorService.GetSharedMonitorsAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<MonitorModel>>(monitors);
      models.TotalCount = monitors.TotalCount;

      return models;
   }
}
