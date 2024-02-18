using Clients.Dash.Infrastructure.AutoMapper.Extensions;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Clients.Dash.Pages.Configuration.Widgets;

/// <summary>
/// Component partial class
/// </summary>
public partial class WidgetForm
{
   [Inject] IWidgetService WidgetService { get; set; }

   /// <summary>
   /// Prepare a widget model
   /// </summary>
   /// <returns>Widget model (async operation)</returns>
   public Task<WidgetModel> PrepareWidgetModelAsync()
   {
      var model = new WidgetModel();
      return Task.FromResult(model);
   }

   /// <summary>
   /// Prepare a widget model by the widget id
   /// </summary>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns>Widget model (async operation)</returns>
   public async Task<WidgetModel> PrepareWidgetModelAsync(long widgetId)
   {
      var widget = await WidgetService.GetByIdAsync(widgetId);
      var model = widget.ToModel<WidgetModel>();
      return model;
   }
}
