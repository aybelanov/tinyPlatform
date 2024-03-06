using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a online device model
/// </summary>
public partial record OnlineDeviceModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Devices.Online.Fields.DeviceInfo")]
   public string DeviceInfo { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.Fields.IPAddress")]
   public string LastIpAddress { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.Fields.Location")]
   public string Location { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.Fields.LastActivityDate")]
   public DateTime? LastActivityDate { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.Fields.Status")]
   public string Status { get; set; }

   #endregion
}
