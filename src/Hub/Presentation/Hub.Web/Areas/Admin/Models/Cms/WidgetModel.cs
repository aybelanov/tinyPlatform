using Microsoft.AspNetCore.Routing;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Cms;

/// <summary>
/// Represents a widget model
/// </summary>
public partial record WidgetModel : BaseAppModel, IPluginModel
{
   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.Widgets.Fields.FriendlyName")]
   public string FriendlyName { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Widgets.Fields.SystemName")]
   public string SystemName { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Widgets.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Widgets.Fields.IsActive")]
   public bool IsActive { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Widgets.Configure")]
   public string ConfigurationUrl { get; set; }

   public string LogoUrl { get; set; }

   public string WidgetViewComponentName { get; set; }

   public RouteValueDictionary WidgetViewComponentArguments { get; set; }

   #endregion
}