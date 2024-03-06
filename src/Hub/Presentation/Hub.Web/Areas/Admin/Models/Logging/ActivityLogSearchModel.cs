using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents an activity log search model
/// </summary>
public partial record ActivityLogSearchModel : BaseSearchModel
{
   #region Ctor

   public ActivityLogSearchModel()
   {
      ActivityLogType = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.CreatedOnFrom")]
   [UIHint("DateNullable")]
   public DateTime? CreatedOnFrom { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.CreatedOnTo")]
   [UIHint("DateNullable")]
   public DateTime? CreatedOnTo { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.ActivityLogType")]
   public int ActivityLogTypeId { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.ActivityLogType")]
   public IList<SelectListItem> ActivityLogType { get; set; }

   [AppResourceDisplayName("Admin.Users.ActivityLog.Fields.IpAddress")]
   public string IpAddress { get; set; }

   public string SubjectType { get; set; }

   #endregion
}