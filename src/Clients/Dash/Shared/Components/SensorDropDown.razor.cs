using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Security;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Components;

#pragma warning disable CS1591

public partial class SensorDropDown
{
   #region Services

   [Inject] ISensorService SensorService { get; set; }

   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region Parameters

   [Parameter]
   public SensorSelectItem Sensor { get; set; }

   [Parameter]
   public EventCallback<SensorSelectItem> SensorChanged { get; set; }
  
   [Parameter]
   public bool IsLoading { get; set; }

   [Parameter]
   public EventCallback<bool> IsLoadingChanged { get; set; }

   [Parameter, EditorRequired]
   public DeviceSelectItem Device { get; set; }

   [Parameter]
   public EventCallback<SensorSelectItem> Change { get; set; }

   [Parameter]
   public string Style { get; set; }

   [Parameter]
   public bool Disabled { get; set; }

   [Parameter]
   public string Name { get; set; }

   [Parameter] 
   public string Placeholder { get; set; }

   /// <summary>
   /// Additional query
   /// </summary>
   [Parameter]
   public string Query { get; set; }

   [Parameter(CaptureUnmatchedValues = true)]
   public Dictionary<string, object> Attributes { get; set; }

   #endregion

   #region fields

   IList<SensorSelectItem> _availableSensors;
   SensorSelectItem _selectedSensor;
   RadzenDropDown<SensorSelectItem> _sensorDropDown;
   long? _currentDeviceId;

   #endregion

   #region Ctor

   /// <summary>
   /// Default Ctor
   /// </summary>
   public SensorDropDown()
   {
      _availableSensors = new FilterableList<SensorSelectItem>();
   }

   #endregion

   #region Lifecicle

   protected override async Task OnParametersSetAsync()
   {
      if (_currentDeviceId != Device?.Id)
      {
         _currentDeviceId = Device.Id;
         await LoadSensors();
      }
      
      await base.OnParametersSetAsync();
   }

   #endregion

   #region Methods

   public async Task LoadSensors()
   {
      await IsLoadingChanged.InvokeAsync(true);
      _selectedSensor = null;
      _sensorDropDown?.Reset();
      _availableSensors.Clear();
      await Task.Yield();

      if (Device is not null)
      {
         try
         {
            var filter = new DynamicFilter() { DeviceId = Device.Id, Query = Query };

            Func<DynamicFilter, Task<IFilterableList<SensorSelectItem>>> getData =
               await PermissionService.IsAdminModeAsync() ? SensorService.GetForAllSensorSelectItemListAsync : SensorService.GetSensorSelectItemListAsync;

            _availableSensors = (await getData(filter)).ToList();
         }
         catch (Exception ex)
         {
            await ErrorService.HandleError(this, ex);
         }
      }

      await IsLoadingChanged.InvokeAsync(false);
      await SensorChange(_selectedSensor);
   }

   private async Task SensorChange(object obj)
   {
      _selectedSensor = obj as SensorSelectItem;
      await SensorChanged.InvokeAsync(_selectedSensor);
      await Change.InvokeAsync(_selectedSensor);
   }

   #endregion
}

#pragma warning restore CS1591