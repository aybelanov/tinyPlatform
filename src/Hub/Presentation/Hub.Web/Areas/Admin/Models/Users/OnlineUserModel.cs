using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents an online user model
/// </summary>
public partial record OnlineUserModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Users.Online.Fields.UserInfo")]
   public string UserInfo { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.Fields.IPAddress")]
   public string LastIpAddress { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.Fields.Location")]
   public string Location { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.Fields.LastActivityDate")]
   public DateTime? LastActivityDate { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.Fields.Status")]
   public string Status { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.Fields.LastVisitedPage")]
   public string LastVisitedPage { get; set; }

   #endregion
}