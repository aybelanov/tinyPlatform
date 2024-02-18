using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Cms;

/// <summary>
/// Represents a widget list model
/// </summary>
public partial record WidgetListModel : BasePagedListModel<WidgetModel>
{
}