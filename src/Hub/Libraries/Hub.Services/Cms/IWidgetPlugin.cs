using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Services.Plugins;

namespace Hub.Services.Cms
{
   /// <summary>
   /// Provides an interface for creating widgets
   /// </summary>
   public partial interface IWidgetPlugin : IPlugin
   {
      /// <summary>
      /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
      /// </summary>
      bool HideInWidgetList { get; }

      /// <summary>
      /// Gets widget zones where this widget should be rendered
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the widget zones
      /// </returns>
      Task<IList<string>> GetWidgetZonesAsync();

      /// <summary>
      /// Gets a name of a view component for displaying widget
      /// </summary>
      /// <param name="widgetZone">FieName of the widget zone</param>
      /// <returns>View component name</returns>
      string GetWidgetViewComponentName(string widgetZone);
   }
}
