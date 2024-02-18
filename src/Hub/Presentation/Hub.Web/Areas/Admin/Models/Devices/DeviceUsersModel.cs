using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a mapping device-to-users
/// </summary>
public partial record DeviceUsersModel : BaseAppModel
{
   public long DeviceId { get; set; }

   public IList<long> SelectedUserIds { get; set; } = new List<long>();
}
