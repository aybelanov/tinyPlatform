using Clients.Dash.Infrastructure.AutoMapper.Extensions;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Pages.Configuration.Monitors;

/// <summary>
/// Monitor instatnce component
/// </summary>
public partial class MonitorForm
{
   [Inject] IMonitorService MonitorService { get; set; }

   /// <summary>
   /// Prepare a model for the create or edit monitor page
   /// </summary>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns>Monitor model</returns>
   public async Task<MonitorModel> PrepareMonitorModelAsync(long monitorId)
   {
      var monitor = await MonitorService.GetByIdAsync(monitorId)
         ?? throw new Exception("Access denied or the monitor does not exist.");

      var model = monitor.ToModel<MonitorModel>();

      return model;
   }

   /// <summary>
   /// Prepare a model for the create or edit monitor page
   /// </summary>
   /// <returns>Model</returns>
   public async Task<MonitorModel> PrepareMonitorModelAsync()
   {
      var model = new MonitorModel();
      return await Task.FromResult(model);
   }
}
