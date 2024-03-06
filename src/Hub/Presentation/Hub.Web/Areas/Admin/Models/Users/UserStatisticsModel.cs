using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user statistics model
/// </summary>
public partial record UserStatisticsModel : BaseAppModel
{
   public string Status { get; set; }

   public string StatusValue { get; set; }

   public int NumberOfOnlineOwnDevices { get; set; }

   public int NumberOfTotalOwnDevices { get; set; }

   public int NumberOfSensors { get; set; }

   public int NumberOfDataRecords { get; set; }
}
