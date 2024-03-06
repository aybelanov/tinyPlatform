using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a device activity log model
/// </summary>
public partial record DeviceActivityLogModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Devices.Devices.ActivityLog.ActivityLogType")]
   public string ActivityLogTypeName { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.ActivityLog.Comment")]
   public string Comment { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.ActivityLog.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.ActivityLog.IpAddress")]
   public string IpAddress { get; set; }

   #endregion
}