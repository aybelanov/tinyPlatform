using Clients.Dash.Infrastructure;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Clients.Dash.Services.Security;
using Clients.Widgets;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.Dash.Pages.Home;

#pragma warning disable CS1591

public partial class DataStatistics : IDisposable
{
   #region Services

   [Inject] ISensorRecordService DataService { get; set; }
   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region fields

   private static readonly IDictionary<TimeIntervalType, string> _intervals;
   private DynamicFilter _filter;
   private EventHandler<bool> _adminModeChangedHandler;

   #endregion

   #region Ctors

   static DataStatistics()
   {
      using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
      var localizer = scope.ServiceProvider.GetRequiredService<Localizer>();
      _intervals = Enum.GetValues<TimeIntervalType>().Where(x => (int)x > 2).ToDictionary(k => k, v => localizer[$"TimeIntervalType.{v}"].ToString());
   }

   public DataStatistics()
   {
      _filter = new DynamicFilter() { TimeInterval = TimeIntervalType.Day };
      _adminModeChangedHandler = async (o, e) => await PermissionService_AdminModeChanged( o, e);
   }

   #endregion

   #region Lifecicle

   protected override void OnInitialized()
   {
      PermissionService.AdminModeChanged += _adminModeChangedHandler;
      isLoading = true;
   }

   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      if (firstRender) 
      {
         await ChartUpdate();
      }
   }

   #endregion

   #region Methods

   private async Task PermissionService_AdminModeChanged(object sender, bool e)
   {
      await chart.Clear(); 
   }

   private async Task ChartUpdate()
   {
      isLoading = true;
      try
      {
         Func<DynamicFilter, Task<IList<TimelineChart.Point>>> getData =
            await PermissionService.IsAdminModeAsync() ? DataService.GetAllDataStatistics : DataService.GetUserDataStatistics;

         var items = await getData(_filter);
         await chart.Draw(items);
      }
      catch (Exception ex)
      {
         await ErrorService.HandleError(this, ex, T["Error.DtatFetch"]);
      }
      finally
      {
         isLoading = false;
         StateHasChanged();
      }
   }

   public void Dispose()
   {
      PermissionService.AdminModeChanged += _adminModeChangedHandler;
   }

   #endregion
}

#pragma warning disable CS1591