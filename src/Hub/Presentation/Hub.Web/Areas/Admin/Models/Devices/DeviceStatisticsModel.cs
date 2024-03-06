using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a device statistics model
/// </summary>
public partial record DeviceStatisticsModel : BaseAppModel
{
   public string Status { get; set; }

   public string StatusValue { get; set; }

   public int NumberOfOnlineDeviceUsers { get; set; }

   public int NumberOfTotalDeviceUsers { get; set; }

   public int NumberOfSensors { get; set; }

   public int NumberOfDataRecords { get; set; }
}
