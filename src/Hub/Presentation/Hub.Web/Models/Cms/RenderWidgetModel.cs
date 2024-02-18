using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Cms;

public partial record RenderWidgetModel : BaseAppModel
{
   public string WidgetViewComponentName { get; set; }
   public object WidgetViewComponentArguments { get; set; }
}