using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices
{
   /// <summary>
   /// Represents a device activity log list model
   /// </summary>
   public partial record DeviceActivityLogListModel : BasePagedListModel<DeviceActivityLogModel>
   {
   }
}