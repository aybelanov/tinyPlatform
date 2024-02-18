using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum group list model
/// </summary>
public partial record ForumGroupListModel : BasePagedListModel<ForumGroupModel>
{
}