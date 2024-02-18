using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Cms;

public partial record RenderWidgetModel : BaseAppModel
{
   public string WidgetViewComponentName { get; set; }
   public object WidgetViewComponentArguments { get; set; }
}