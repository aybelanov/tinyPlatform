using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// represents a device list model
/// </summary>
public partial record DeviceListModel : BasePagedListModel<DeviceModel>
{
}
