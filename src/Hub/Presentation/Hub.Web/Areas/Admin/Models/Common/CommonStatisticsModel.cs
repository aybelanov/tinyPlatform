using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

public partial record CommonStatisticsModel : BaseAppModel
{
   public int NumberOfUsers { get; set; }
   public int NumberOfOnlineUsers { get; set; }
   public int NumberOfDevices { get; set; }
   public int NumberOfOnlineDevices { get; set; }
   public int NumberOfSensors { get; set; }
   public int NumberOfRecords { get; set; }
}