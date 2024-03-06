using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Security;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Shared.Clients;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Components;

#pragma warning disable CS1591

public partial class SensorMultiDropDown
{
   #region Services

   [Inject] ISensorService SensorService { get; set; }

   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region Parameters

   [Parameter]
   public IEnumerable<SensorSelectItem> Sensors { get; set; }

   [Parameter]
   public EventCallback<IEnumerable<SensorSelectItem>> SensorsChanged { get; set; }

   [Parameter]
   public int MaxSelectedLabels { get; set; }

   [Parameter, EditorRequired]
   public DeviceSelectItem Device { get; set; }

   [Parameter]
   public bool IsLoading { get; set; }

   [Parameter]
   public EventCallback<bool> IsLoadingChanged { get; set; }

   [Parameter]
   public EventCallback<IEnumerable<SensorSelectItem>> Change { get; set; }

   [Parameter]
   public string Style { get; set; }

   [Parameter]
   public bool Disabled { get; set; }

   [Parameter]
   public string Name { get; set; }

   /// <summary>
   /// Clears previous selecting after divice has changed
   /// </summary>
   [Parameter]
   public bool Clear { get; set; }

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
   IEnumerable<SensorSelectItem> _selectedSensors;
   long? _currentDeviceId;
   RadzenDropDown<IEnumerable<SensorSelectItem>> _sensorDropDown;

   #endregion

   #region Ctor

   /// <summary>
   /// Default Ctor
   /// </summary>
   public SensorMultiDropDown()
   {
      _availableSensors = new FilterableList<SensorSelectItem>();
      MaxSelectedLabels = 5;
      //_selectedSensors = new List<SensorSelectItem>();
   }

   #endregion

   #region Lifecicle

   protected override async Task OnParametersSetAsync()
   {
      if (_currentDeviceId != Device?.Id)
      {
         _currentDeviceId = Device?.Id;
         await LoadSensors();
      }

      await base.OnParametersSetAsync();
   }

   #endregion

   #region Methods

   public async Task LoadSensors()
   {
      await IsLoadingChanged.InvokeAsync(true);
      await Task.Yield();
      if (Clear)
      {
         _selectedSensors = null;// new List<SensorSelectItem>().AsEnumerable();
         _sensorDropDown.Reset();
         await SensorChange(_selectedSensors);
      }
      _availableSensors.Clear();

      if (Device is not null && Device.Id > 0)
      {
         try
         {
            var filter = new DynamicFilter() { DeviceId = Device.Id };
            filter.Query = Query;

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
   }

   private async Task SensorChange(object obj)
   {
      _selectedSensors = (obj as IEnumerable)?.Cast<SensorSelectItem>();
      await SensorsChanged.InvokeAsync(_selectedSensors);
      await Change.InvokeAsync(_selectedSensors);
   }

   #endregion
}

#pragma warning restore CS1591