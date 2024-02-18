using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Cms;
using Hub.Data.Extensions;
using Hub.Services.Users;
using Hub.Services.Plugins;

namespace Hub.Services.Cms
{
   /// <summary>
   /// Represents a widget plugin manager implementation
   /// </summary>
   public partial class WidgetPluginManager : PluginManager<IWidgetPlugin>, IWidgetPluginManager
   {
      #region Fields

      private readonly WidgetSettings _widgetSettings;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      /// <param name="userService"></param>
      /// <param name="pluginService"></param>
      /// <param name="widgetSettings"></param>
      public WidgetPluginManager(IUserService userService,
          IPluginService pluginService,
          WidgetSettings widgetSettings) : base(userService, pluginService)
      {
         _widgetSettings = widgetSettings;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Load active widgets
      /// </summary>
      /// <param name="user">Filter by user; pass null to load all plugins</param>
      /// <param name="widgetZone">Widget zone; pass null to load all plugins</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the list of active widget
      /// </returns>
      public virtual async Task<IList<IWidgetPlugin>> LoadActivePluginsAsync(User user = null, string widgetZone = null)
      {
         var widgets = await LoadActivePluginsAsync(_widgetSettings.ActiveWidgetSystemNames, user);

         //filter by widget zone
         if (!string.IsNullOrEmpty(widgetZone))
            widgets = await widgets.WhereAwait(async widget =>
                (await widget.GetWidgetZonesAsync()).Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToListAsync();

         return widgets;
      }

      /// <summary>
      /// Check whether the passed widget is active
      /// </summary>
      /// <param name="widget">Widget to check</param>
      /// <returns>Result</returns>
      public virtual bool IsPluginActive(IWidgetPlugin widget)
      {
         return IsPluginActive(widget, _widgetSettings.ActiveWidgetSystemNames);
      }

      /// <summary>
      /// Check whether the widget with the passed system name is active
      /// </summary>
      /// <param name="systemName">System name of widget to check</param>
      /// <param name="user">Filter by user; pass null to load all plugins</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null)
      {
         var widget = await LoadPluginBySystemNameAsync(systemName, user);

         return IsPluginActive(widget);
      }

      #endregion
   }
}