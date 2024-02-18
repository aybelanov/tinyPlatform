using System;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Gdpr;

 /// <summary>
 /// Represents a GDPR log (request) model
 /// </summary>
 public partial record GdprLogModel : BaseAppEntityModel
 {
     #region Properties

     [AppResourceDisplayName("Admin.Users.GdprLog.Fields.UserInfo")]
     public string UserInfo { get; set; }

     [AppResourceDisplayName("Admin.Users.GdprLog.Fields.RequestType")]
     public string RequestType { get; set; }

     [AppResourceDisplayName("Admin.Users.GdprLog.Fields.RequestDetails")]
     public string RequestDetails { get; set; }

     [AppResourceDisplayName("Admin.Users.GdprLog.Fields.CreatedOn")]
     public DateTime CreatedOn { get; set; }

     #endregion
 }