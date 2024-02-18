using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a user device search model
/// </summary>
public partial record DeviceUserSearchModel : BaseSearchModel
{
   /// <summary>
   /// User identifier
   /// </summary>
   public long DeviceId { get; set; } 
}
