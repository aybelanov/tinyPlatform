using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an URL record list model
/// </summary>
public partial record UrlRecordListModel : BasePagedListModel<UrlRecordModel>
{
}