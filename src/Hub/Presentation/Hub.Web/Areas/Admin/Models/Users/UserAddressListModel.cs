using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user address list model
/// </summary>
public partial record UserAddressListModel : BasePagedListModel<AddressModel>
{
}