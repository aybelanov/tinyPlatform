using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user attribute list model
/// </summary>
public partial record UserAttributeListModel : BasePagedListModel<UserAttributeModel>
{
}