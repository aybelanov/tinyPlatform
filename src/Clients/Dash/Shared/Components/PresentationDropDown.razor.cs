using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.ErrorServices;
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
public partial class PresentationDropDown
{
#pragma warning disable CS1591

   #region Services

   [Inject] IPresentationService PresentationService { get; set; }
   [Inject] IStaticCacheManager StaticCacheManager { get; set; }
   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region Parameters

   [Parameter]
   public PresentationSelectItem Presentation { get; set; }

   [Parameter]
   public EventCallback<PresentationSelectItem> PresentationChanged { get; set; }

   [Parameter]
   public EventCallback<PresentationSelectItem> Change { get; set; }

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

   IFilterableList<PresentationSelectItem> _availablePresentations = new FilterableList<PresentationSelectItem>() { new() };
   DynamicFilter _filter;
   RadzenDropDown<PresentationSelectItem> _presenstationDropDown;

   #endregion

   #region Methods

   private async Task LoadItems(LoadDataArgs args)
   {
      if (args.Top == 0)
         args.Top = 10;

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
               _filter.Filter =
                  $"(WidgetName == null ? \"\" : WidgetName).ToLower().Contains(\"{args.Filter.ToLower()}\")" +
                  $" || (SensorName == null ? \"\" : SensorName).ToLower().Contains(\"{args.Filter.ToLower()}\")" +
                  $" || (DeviceName == null ? \"\" : DeviceName).ToLower().Contains(\"{args.Filter.ToLower()}\")";


            Func<DynamicFilter, Task<IFilterableList<PresentationSelectItem>>> getData = 
               await PermissionService.IsAdminModeAsync() ? PresentationService.GetAllPresentationSelectListAsync : PresentationService.GetOwnPresentationSelectListAsync;

            _availablePresentations = await getData(_filter);
            
            if (Presentation != null)
            {
               var item = _availablePresentations.FirstOrDefault(x => x.Id == Presentation.Id);
               if (item != null)
                  _availablePresentations.Remove(item);
             
               _availablePresentations.Add(Presentation);
               _availablePresentations = new FilterableList<PresentationSelectItem>(_availablePresentations.OrderBy(x => x.Id), _availablePresentations.TotalCount);
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

   private async Task PresentationChange(object obj)
   {
      Presentation = obj as PresentationSelectItem;
      await PresentationChanged.InvokeAsync(Presentation);
      await Change.InvokeAsync(Presentation);

      if (obj is null)
         await StaticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
   }

   #endregion

#pragma warning restore CS1591
}
