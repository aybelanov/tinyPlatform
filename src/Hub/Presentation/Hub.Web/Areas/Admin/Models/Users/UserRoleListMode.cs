using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user role list model
/// </summary>
public partial record UserRoleListModel : BasePagedListModel<UserRoleModel>
{
}