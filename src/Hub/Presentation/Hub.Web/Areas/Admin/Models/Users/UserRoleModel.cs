using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user role model
/// </summary>
public partial record UserRoleModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Users.UserRoles.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.Users.UserRoles.Fields.Active")]
   public bool Active { get; set; }

   [AppResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
   public bool IsSystemRole { get; set; }

   [AppResourceDisplayName("Admin.Users.UserRoles.Fields.SystemName")]
   public string SystemName { get; set; }

   [AppResourceDisplayName("Admin.Users.UserRoles.Fields.EnablePasswordLifetime")]
   public bool EnablePasswordLifetime { get; set; }

   #endregion
}