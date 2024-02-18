using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user attribute value search model
/// </summary>
public partial record UserAttributeValueSearchModel : BaseSearchModel
{
   #region Properties

   public long UserAttributeId { get; set; }

   #endregion
}