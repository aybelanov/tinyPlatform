using System.Collections.Generic;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a associated external auth records search model
/// </summary>
public record UserAssociatedExternalAuthRecordsSearchModel : BaseSearchModel
{
   #region Properties

   public long UserId { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth")]
   public IList<UserAssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; } = new List<UserAssociatedExternalAuthModel>();

   #endregion
}
