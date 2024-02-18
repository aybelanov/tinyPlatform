using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a mapping device-to-users
/// </summary>
public partial record UserDevicesModel : BaseAppModel
{
   public long UserId { get; set; }

   public IList<long> SelectedDeviceIds { get; set; } = new List<long>();
}
