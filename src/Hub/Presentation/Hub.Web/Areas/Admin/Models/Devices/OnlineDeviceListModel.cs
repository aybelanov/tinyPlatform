using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a onlone device list model
/// </summary>
public partial record OnlineDeviceListModel : BasePagedListModel<OnlineDeviceModel>
{
}
