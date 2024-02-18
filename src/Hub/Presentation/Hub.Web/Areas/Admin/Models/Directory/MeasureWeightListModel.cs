using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a measure weight list model
/// </summary>
public partial record MeasureWeightListModel : BasePagedListModel<MeasureWeightModel>
{
}