using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices;

public partial record DeviceUserListModel : BasePagedListModel<UserModel>
{
}
