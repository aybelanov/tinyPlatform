using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Gdpr;

/// <summary>
/// Represents a GDPR request list model
/// </summary>
public partial record GdprLogListModel : BasePagedListModel<GdprLogModel>
{
}