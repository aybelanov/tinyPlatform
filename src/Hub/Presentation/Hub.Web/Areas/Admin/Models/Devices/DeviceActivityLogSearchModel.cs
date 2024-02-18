using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a device activity log search model
/// </summary>
public partial record DeviceActivityLogSearchModel : BaseSearchModel
{
   #region Properties

   public long DeviceId { get; set; }

   #endregion
}