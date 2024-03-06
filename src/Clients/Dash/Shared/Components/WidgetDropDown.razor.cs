using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Security;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Shared.Components;

/// <summary>
/// Component partial class
/// </summary>
public partial class WidgetDropDown
{
#pragma warning disable CS1591

   #region Services

   [Inject] IWidgetService WidgetService { get; set; }
   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region Parameters

   [Parameter]
   public WidgetSelectItem Widget { get; set; }

   [Parameter]
   public EventCallback<WidgetSelectItem> WidgetChanged { get; set; }

   [Parameter]
   public EventCallback<WidgetSelectItem> Change { get; set; }

   [Parameter]
   public string Style { get; set; }

   [Parameter]
   public bool IsLoading { get; set; }

   [Parameter]
   public EventCallback<bool> IsLoadingChanged { get; set; }

   [Parameter]
   public bool Disabled { get; set; }

   [Parameter]
   public string Placeholder { get; set; }

   [Parameter]
   public string Name { get; set; }

   [Parameter]
   public bool AllowClear { get; set; }

   [Parameter]
   public long? UserId { get; set; }

   /// <summary>
   /// Additional query
   /// </summary>
   [Parameter]
   public string Query { get; set; }

   [Parameter(CaptureUnmatchedValues = true)]
   public Dictionary<string, object> Attributes { get; set; }

   #endregion

   #region fields

   IFilterableList<WidgetSelectItem> _availableWidgets = new FilterableList<WidgetSelectItem>() { new() };
   WidgetSelectItem _selectedWidget;
   long? _curentUserId;
   DynamicFilter _filter = new();
   RadzenDropDown<WidgetSelectItem> _widgetDropDown;

   #endregion

   #region Methods

   private async Task LoadWidgets(LoadDataArgs args)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(args);
      filter.Query = Query;

      if (!filter.Equals(_filter))
      {
         _filter = filter;
         await IsLoadingChanged.InvokeAsync(true);
         await Task.Yield();
         try
         {
            if (!string.IsNullOrEmpty(_filter.Filter))
               _filter.Filter = $"(Name == null ? \"\" : Name).ToLower().Contains(\"{args.Filter.ToLower()}\")";

            _filter.UserId = _curentUserId;

            Func<DynamicFilter, Task<IFilterableList<WidgetSelectItem>>> getData =
               await PermissionService.IsAdminModeAsync() ? WidgetService.GetAllWidgetSelectListAsync : WidgetService.GetUserWidgetSelectListAsync;

            _availableWidgets = await getData(_filter);

            if (_selectedWidget != null)
            {
               var item = _availableWidgets.FirstOrDefault(x => x.Id == _selectedWidget.Id);
               if (item != null)
               {
                  _availableWidgets.Remove(item);
                  _availableWidgets.Add(_selectedWidget);
                  _availableWidgets = new FilterableList<WidgetSelectItem>(_availableWidgets.OrderBy(x => x.Id), _availableWidgets.TotalCount);
               }
            }
         }
         catch (Exception ex)
         {
            await ErrorService.HandleError(this, ex);
         }
         finally
         {
            if (IsLoadingChanged.HasDelegate)
            {
               await IsLoadingChanged.InvokeAsync(false);
            }
            else
            {
               StateHasChanged();
            }
         }
      }
   }

   private async Task WidgetChange(object obj)
   {
      _selectedWidget = obj as WidgetSelectItem;
      await WidgetChanged.InvokeAsync(_selectedWidget);
      await Change.InvokeAsync(_selectedWidget);
   }

   #endregion

#pragma warning restore CS1591
}
