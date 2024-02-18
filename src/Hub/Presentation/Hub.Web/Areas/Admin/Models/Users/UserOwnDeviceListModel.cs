using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

public partial record UserOwnDeviceListModel : BasePagedListModel<DeviceModel>
{
}
