using System;
using System.ComponentModel.DataAnnotations;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents an activity log model
/// </summary>
public partial record ActivityLogModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.ActivityLogType")]
   public string ActivityLogTypeName { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.User")]
   public long SubjectId { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.UserEmail")]
   [DataType(DataType.EmailAddress)]
   public string Subject { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.Comment")]
   public string Comment { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.IpAddress")]
   public string IpAddress { get; set; }

   #endregion
}