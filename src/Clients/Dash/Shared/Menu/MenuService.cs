using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.ErrorServices;
using Clients.Dash.Services.Localization;
using Clients.Dash.Services.Security;
using Clients.Dash.Shared.Menu;
using Radzen;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.Dash.Services.UI;

/// <summary>
/// Represents a menu service implementation
/// </summary>
public class MenuService
{
   #region fields

   private readonly Localizer T;
   private readonly ErrorService _errorService;
   private readonly IMonitorService _monitorService;
   private readonly PermissionService _adminService;
   private readonly IStaticCacheManager _staticCacheManager;

   private CategoryMenuModel ownMonitorItem;
   private CategoryMenuModel sharedMonitorItem;
   private static readonly DynamicFilter _menuQuery;
   #endregion

   #region Events

   /// <summary>
   /// If the monitor collcetion state has ahnged
   /// </summary>
   public event EventHandler<MonitorMenuEventArg> MonitorMenuChanged;

   #endregion

   #region Event handlers

   /// <summary>
   /// OnMonitorMenuChanged notify
   /// </summary>
   /// <param name="o"></param>
   /// <param name="e"></param>
   public void OnMonitorMenuHasChanged(object o, MonitorMenuEventArg e) => MonitorMenuChanged?.Invoke(o, e);

   #endregion

   #region Ctors

   static MenuService()
   {
      _menuQuery = new DynamicFilter() { Query = "query => query.Where(x => x.ShowInMenu)" };
   }

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public MenuService(ErrorService errorService,
      IMonitorService monitorService,
      IStaticCacheManager staticCacheManager,
      PermissionService adminService,
      Localizer localizer)
   {
      T = localizer;
      _errorService = errorService;
      _monitorService = monitorService;
      _adminService = adminService;
      _staticCacheManager = staticCacheManager;
   }


   #endregion

   #region Methods

   /// <summary>
   /// Gets the overview menu item 
   /// </summary>
   /// <returns></returns>
   public Task<CategoryMenuModel> GetOverviewMenuItemAsync()
   => Task.FromResult(new CategoryMenuModel()
   {
      Name = T["Sidebar.Menu.Overview.Name"],
      Path = "./",
      Icon = "&#xe871",
      Description = T["Sidebar.Menu.Overview.Description"],
      Title = T["Sidebar.Menu.Overview.Name"],
   });


   /// <summary>
   /// Gets the report menu item
   /// </summary>
   /// <returns></returns>
   public Task<CategoryMenuModel> GetReportMenuItemAsync()
   => Task.FromResult(new CategoryMenuModel()
   {
      Name = T["Sidebar.Menu.Report.Name"],
      //Path = "reports/",
      Icon = "summarize",
      Description = T["Sidebar.Menu.Report.Description"],
      Title = T["Sidebar.Menu.Report.Name"],
      Children = new List<CategoryMenuModel>()
      {
         new()
         {
            Name = T["Sidebar.Menu.Report.Charts"],
            Path = "reports/charts",
            Icon = "bar_chart",
            Description = T["Sidebar.Menu.Report.Charts"],
            Title = T["Sidebar.Menu.Report.Charts"],
         },
         new()
         {
            Name = T["Sidebar.Menu.Report.Export"],
            Path = "reports/exportdata",
            Icon = "system_update_alt",
            Description = T["Sidebar.Menu.Report.Export"],
            Title = T["Sidebar.Menu.Report.Export"],
         },
         new()
         {
            Name = T["Sidebar.Menu.Report.Tracker"],
            Path = "reports/tracker",
            Icon = "map",
            Description = T["Sidebar.Menu.Report.Tracker"],
            Title = T["Sidebar.Menu.Report.Tracker"],
         },
         new()
         {
            Name = T["Sidebar.Menu.Report.Player"],
            Path = "reports/player",
            Icon = "play_circle",
            Description = T["Sidebar.Menu.Report.Player"],
            Title = T["Sidebar.Menu.Report.Player"],
         }
      }
   });


   /// <summary>
   /// Gets the configuration menu item
   /// </summary>
   /// <returns></returns>
   public async Task<CategoryMenuModel> GetConfigurationMenuItemAsync()
   {
      var menuModel = new CategoryMenuModel()
      {
         Name = T["Sidebar.Menu.Config.Name"],
         //Path = "configuration/",
         Icon = "&#xe8b8",
         Description = T["Sidebar.Menu.Config.Description"],
         Title = T["Sidebar.Menu.Config.Name"],
         Children = new List<CategoryMenuModel>()
      };

      if (await _adminService.IsAdminModeAsync())
      {
         menuModel.Children.Add(new CategoryMenuModel()
         {
            Name = T["Sidebar.Menu.Config.Users.Name"],
            Path = "configuration/users",
            Icon = "&#xea21",
            Description = T["Sidebar.Menu.Config.Users.Description"],
            Title = T["Sidebar.Menu.Config.Users.Name"],
         });
      }

      menuModel.Children.Add(new CategoryMenuModel()
      {
         Name = T["Sidebar.Menu.Config.Monitors.Name"],
         Path = "configuration/monitors",
         Icon = "&#xeb97",
         Description = T["Sidebar.Menu.Config.Monitors.Description"],
         Title = T["Sidebar.Menu.Config.Monitors.Name"],
      });

      menuModel.Children.Add(new CategoryMenuModel()
      {
         Name = T["Sidebar.Menu.Config.Devices.Name"],
         Path = "configuration/devices",
         Icon = "&#xe328",
         Description = T["Sidebar.Menu.Config.Devices.Description"],
         Title = T["Sidebar.Menu.Config.Devices.Name"],
      });

      menuModel.Children.Add(new CategoryMenuModel()
      {
         Name = T["Sidebar.Menu.Config.Widgets.Name"],
         Path = "configuration/widgets",
         Icon = "&#xe6e1",
         Description = T["Sidebar.Menu.Config.Widgets.Description"],
         Title = T["Sidebar.Menu.Config.Widgets.Name"],
      });

      return menuModel;
   }

   /// <summary>
   /// Gets the own monitor menu items
   /// </summary>
   /// <returns></returns>
   public async Task<CategoryMenuModel> GetOwnMonitorMenuAsync(string term = null)
   {
      IEnumerable<Monitor> monitors = null;
      CategoryMenuModel monitorItem = null;

      try
      {
         monitors = (await _monitorService.GetOwnMonitorsAsync(_menuQuery)).OrderBy(x => x.DisplayOrder);

         if (!string.IsNullOrWhiteSpace(term))
            monitors = monitors.Where(x => x.Name.Contains(term));
      }
      catch (Exception ex)
      {
         await _errorService.HandleError(this, ex);
      }

      if (monitors?.Any() ?? false)
      {
         monitorItem = new CategoryMenuModel()
         {
            Name = T["Sidebar.Menu.OwnMonitors.Name"],
            Icon = "&#xeb97",
            Description = T["Sidebar.Menu.OwnMonitors.Description"],
            Title = T["Sidebar.Menu.OwnMonitors.Name"],
            Expanded = true,
            Children = new List<CategoryMenuModel>()
         };

         foreach (var m in monitors)
         {
            var child = new CategoryMenuModel()
            {
               Name = m.MenuItem,
               Path = $"monitor/{m.Id}",
               Description = m.Description,
               Expanded = false,
               Title = m.MenuItem,
               OrderId = m.DisplayOrder,
               Icon = "&#xeb97",
            };

            monitorItem.Children.Add(child);
         }
      }

      this.ownMonitorItem = monitorItem;
      return this.ownMonitorItem;
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="cacheType">"own" or "shared"</param>
   /// <returns></returns>
   public async Task ClearmonitorMenuCache(string cacheType)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Monitor>.ByDynamicFilterCacheKey, cacheType, _menuQuery);
      await _staticCacheManager.RemoveAsync(cacheKey, cacheKey, _menuQuery);
   }

   /// <summary>
   /// Gets the shared monitor menu items
   /// </summary>
   /// <returns></returns>
   public async Task<CategoryMenuModel> GetSharedMonitorMenuAsync(string term = null)
   {
      IEnumerable<Monitor> monitors = null;
      CategoryMenuModel monitorItem = null;

      try
      {
         monitors = (await _monitorService.GetSharedMonitorsAsync(_menuQuery)).OrderBy(x => x.DisplayOrder);

         if (!string.IsNullOrWhiteSpace(term))
            monitors = monitors.Where(x => x.Name.Contains(term));
      }
      catch (Exception ex)
      {
         await _errorService.HandleError(this, ex);
      }

      if (monitors?.Any() ?? false)
      {
         monitorItem = new CategoryMenuModel()
         {
            Name = T["Sidebar.Menu.SharedMonitors.Name"],
            Icon = "&#xeb97",
            Description = T["Sidebar.Menu.SharedMonitors.Description"],
            Title = T["Sidebar.Menu.SharedMonitors.Name"],
            Expanded = true,
            Children = new List<CategoryMenuModel>()
         };

         foreach (var m in monitors)
         {
            var child = new CategoryMenuModel()
            {
               Name = m.MenuItem,
               Path = $"monitor/{m.Id}",
               Description = m.Description,
               Expanded = false,
               Title = m.MenuItem,
               OrderId = m.DisplayOrder,
               Icon = "&#xeb97",
            };

            monitorItem.Children.Add(child);
         }
      }

      this.sharedMonitorItem = monitorItem;
      return this.sharedMonitorItem;
   }


   /// <summary>
   /// Gets all categories for the search filter
   /// </summary>
   /// <returns></returns>
   public async Task<IEnumerable<CategoryMenuModel>> GetAllCategoriesForFilters()
   {
      var categories = new List<CategoryMenuModel>
      {
         await GetOverviewMenuItemAsync(),
         await GetConfigurationMenuItemAsync(),
         await GetReportMenuItemAsync()
      };

      if (ownMonitorItem != null)
         categories.Add(ownMonitorItem);

      if (sharedMonitorItem != null)
         categories.Add(sharedMonitorItem);

      return categories;
   }

   /// <summary>
   /// Filters menu items by the term
   /// </summary>
   /// <param name="term">Search term</param>
   /// <returns></returns>
   public async Task<IEnumerable<CategoryMenuModel>> FilterAsync(string term)
   {
      var allCats = await GetAllCategoriesForFilters();

      if (string.IsNullOrEmpty(term))
         return allCats;

      bool contains(string value) => value != null && value.Contains(term, StringComparison.OrdinalIgnoreCase);

      bool filter(CategoryMenuModel category) => contains(category.Name) || category.Tags != null && category.Tags.Any(contains);

      bool deepFilter(CategoryMenuModel example) => filter(example) || example.Children?.Any(filter) == true;

      return allCats.Where(category => category.Children?.Any(deepFilter) == true || filter(category)).Select(category => new CategoryMenuModel
      {
         Name = category.Name,
         Expanded = true,
         Children = category.Children?.Where(deepFilter).Select(example => new CategoryMenuModel
         {
            Name = example.Name,
            Path = example.Path,
            Icon = example.Icon,
            Expanded = true,
            Children = example.Children

         }).ToArray()

      }).ToList();
   }

   #endregion
}