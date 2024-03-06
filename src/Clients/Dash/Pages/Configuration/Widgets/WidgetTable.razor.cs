using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Configuration.Widgets;

/// <summary>
/// Component partial class
/// </summary>
public partial class WidgetTable
{
   [Inject] IWidgetService WidgetService { get; set; }

   /// <summary>
   /// Default ctor
   /// </summary>
   public WidgetTable()
   {
      AddButton = AddWidget;
      DeleteButton = OnDeleteItemAsync;
   }

   /// <summary>
   /// Prepapre model for a widget grid 
   /// </summary>
   /// <returns>Widget entity coolection (async opertaion)</returns>
   public async Task<IFilterableList<WidgetModel>> PrepareWidgetModelsAsync(DynamicFilter filter)
   {
      Func<DynamicFilter, Task<IFilterableList<Widget>>> getData = await PermissionService.IsAdminModeAsync()
         ? WidgetService.GetAllWidgetsAsync
         : WidgetService.GetUserWidgetsAsync;

      var widgets = await getData(filter);
      var model = Auto.Mapper.Map<FilterableList<WidgetModel>>(widgets);
      model.TotalCount = widgets.TotalCount;

      return model;
   }

   Task AddWidget()
   {
      Navigation.NavigateTo("configuration/widget/create");
      return Task.CompletedTask;
   }
}
