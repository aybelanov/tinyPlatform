using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user address search model
/// </summary>
public partial record UserAddressSearchModel : BaseSearchModel
{
   #region Properties

   public long UserId { get; set; }

   #endregion
}