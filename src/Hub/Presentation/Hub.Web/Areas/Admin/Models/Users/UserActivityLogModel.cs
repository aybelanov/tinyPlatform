using System;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user activity log model
/// </summary>
public partial record UserActivityLogModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Users.Users.ActivityLog.ActivityLogType")]
   public string ActivityLogTypeName { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.ActivityLog.Comment")]
   public string Comment { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.ActivityLog.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.ActivityLog.IpAddress")]
   public string IpAddress { get; set; }

   #endregion
}