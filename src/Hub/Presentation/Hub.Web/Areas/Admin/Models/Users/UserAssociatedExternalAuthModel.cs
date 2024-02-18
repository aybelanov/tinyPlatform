using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user associated external authentication model
/// </summary>
public partial record UserAssociatedExternalAuthModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.Email")]
   public string Email { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.ExternalIdentifier")]
   public string ExternalIdentifier { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.AuthMethodName")]
   public string AuthMethodName { get; set; }

   #endregion
}