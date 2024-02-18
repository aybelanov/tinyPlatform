using Clients.Dash.Caching;
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
using System.Text.Json;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Shared.Components;

#pragma warning disable CS1591

public partial class DeviceDropDown
{
   #region Services

   [Inject] IDeviceService DeviceService { get; set; }
   [Inject] IStaticCacheManager StaticCacheManager { get; set; }
   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region Parameters

   [Parameter]
   public DeviceSelectItem Device { get; set; }

   [Parameter]
   public EventCallback<DeviceSelectItem> DeviceChanged { get; set; }

   [Parameter]
   public EventCallback<DeviceSelectItem> Change { get; set; }

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

   IFilterableList<DeviceSelectItem> _availableDevices = new FilterableList<DeviceSelectItem>() { new() };
   DeviceSelectItem _selectedDevice;
   long? _curentUserId;
   bool? _isAdminMode;
   DynamicFilter _filter;
   RadzenDropDown<DeviceSelectItem> _dropDown;
   bool _showStub;

   #endregion

   protected override async Task OnParametersSetAsync()
   {
      var currentMode = await PermissionService.IsAdminModeAsync();
      if (_curentUserId != UserId || _isAdminMode != currentMode)
      {
         _curentUserId = UserId;
         _isAdminMode = currentMode;

         _showStub = true;
         await Task.Yield();
         _availableDevices = new FilterableList<DeviceSelectItem>() { new() };
         _filter = new();
         await DeviceChange(null);
         _showStub = false;
      }
   }

   private async Task LoadDevices(LoadDataArgs args)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(args);
      filter.Query = Query;

      if (!filter.Equals(_filter))
      {
         _filter = filter;
         await Task.Yield();
         await IsLoadingChanged.InvokeAsync(true);
         try
         {
            if (!string.IsNullOrEmpty(_filter.Filter))
               _filter.Filter = $"(Name == null ? \"\" : Name).ToLower().Contains(\"{args.Filter.ToLower()}\")";

            _filter.UserId = _curentUserId;

            Func<DynamicFilter, Task<IFilterableList<DeviceSelectItem>>> getData =
               await PermissionService.IsAdminModeAsync() ? DeviceService.GetAllDeviceSelectListAsync : DeviceService.GetUserDeviceSelectListAsync;

            _availableDevices = await getData(_filter);

            if (_selectedDevice != null)
            {
               var item = _availableDevices.FirstOrDefault(x => x.Id == _selectedDevice.Id);
               if (item != null)
               {
                  _availableDevices.Remove(item);
                  _availableDevices.Add(_selectedDevice);
                  _availableDevices = new FilterableList<DeviceSelectItem>(_availableDevices.OrderBy(x => x.Id), _availableDevices.TotalCount);
               }
            }

            //Console.WriteLine(JsonSerializer.Serialize(_availableDevices));
            
            //var newItems = await getData(_filter);

            //var items = _availableDevices.ToList();
            //items.AddRange(newItems);
            //items = items.Where(x => x.Id > 0).DistinctBy(x => x.Id).ToList();

            //if (_selectedDevice != null)
            //{
            //   var item = items.FirstOrDefault(x => x.Id == _selectedDevice.Id);
            //   if (item != null)
            //   {
            //      items.Remove(item);
            //      items.Add(_selectedDevice);
            //   }
            //}

            //_availableDevices = new FilterableList<DeviceSelectItem>(items.OrderBy(x => x.Id), newItems.TotalCount);
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

   private async Task DeviceChange(object obj)
   {
      _selectedDevice = obj as DeviceSelectItem;
      await DeviceChanged.InvokeAsync(_selectedDevice);      
      await Change.InvokeAsync(_selectedDevice);
   }
}

#pragma warning restore CS1591