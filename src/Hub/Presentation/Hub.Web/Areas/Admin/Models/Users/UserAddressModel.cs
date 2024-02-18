using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user address model
/// </summary>
public partial record UserAddressModel : BaseAppModel
{
   #region Ctor

   public UserAddressModel()
   {
      Address = new AddressModel();
   }

   #endregion

   #region Properties

   public long UserId { get; set; }

   public AddressModel Address { get; set; }

   #endregion
}