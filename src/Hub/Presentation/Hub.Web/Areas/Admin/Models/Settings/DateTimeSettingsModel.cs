using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a date time settings model
/// </summary>
public partial record DateTimeSettingsModel : BaseAppModel, ISettingsModel
{
   #region Ctor

   public DateTimeSettingsModel()
   {
      AvailableTimeZones = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToSetTimeZone")]
   public bool AllowUsersToSetTimeZone { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultPlatformTimeZone")]
   public string DefaultPlatformTimeZoneId { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultPlatformTimeZone")]
   public IList<SelectListItem> AvailableTimeZones { get; set; }

   #endregion
}