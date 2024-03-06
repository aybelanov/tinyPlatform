using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;

namespace Hub.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents a log model
/// </summary>
public partial record LogModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
   public string LogLevel { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
   public string ShortMessage { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
   public string FullMessage { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
   public string IpAddress { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.Client")]
   public long? EntityId { get; set; }

   public string EntityName { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.Client")]
   public string Subject { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.PageURL")]
   public string PageUrl { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
   public string ReferrerUrl { get; set; }

   [AppResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   #endregion
}