using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user device search model
/// </summary>
public partial record UserDeviceSearchModel : BaseSearchModel
{
   /// <summary>
   /// User identifier
   /// </summary>
   public long UserId { get; set; } 
}
