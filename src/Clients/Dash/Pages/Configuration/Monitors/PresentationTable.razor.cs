using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;


namespace Clients.Dash.Pages.Configuration.Monitors;

/// <summary>
/// Component partial class
/// </summary>
public partial class PresentationTable
{
   async Task<IFilterableList<PresentationModel>> PreparePresentationModelAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var presentation = await PresentationService.GetPresentationsAsync(filter);
      var model = Auto.Mapper.Map<FilterableList<PresentationModel>>(presentation);
      model.TotalCount = presentation.TotalCount;

      return model;
   }
}
