using System.Threading.Tasks;
using System;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;
using Microsoft.AspNetCore.Components;
using Clients.Dash.Services.EntityServices;

namespace Clients.Dash.Pages.Monitors;

/// <summary>
/// Component partial cass
/// </summary>
public partial class _Monitor
{
   #region Services

   [Inject] IMonitorService MonitorService { get; set; }

   #endregion

   #region Methods

   async Task<MonitorViewModel> PrepareMonitorViewModelAsync(long monitorId)
   {
      var view = await MonitorService.GetMonitorViewAsync(monitorId);
      var model = Auto.Mapper.Map<MonitorViewModel>(view);  
      
      return model;
   }

   #endregion

}
