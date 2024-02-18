using System;
using System.ComponentModel.DataAnnotations;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a queued email search model
/// </summary>
public partial record QueuedEmailSearchModel : BaseSearchModel
{
   #region Properties

   [AppResourceDisplayName("Admin.System.QueuedEmails.List.StartDate")]
   [UIHint("DateNullable")]
   public DateTime? SearchStartDate { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.List.EndDate")]
   [UIHint("DateNullable")]
   public DateTime? SearchEndDate { get; set; }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.System.QueuedEmails.List.FromEmail")]
   public string SearchFromEmail { get; set; }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.System.QueuedEmails.List.ToEmail")]
   public string SearchToEmail { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.List.LoadNotSent")]
   public bool SearchLoadNotSent { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.List.MaxSentTries")]
   public int SearchMaxSentTries { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.List.GoDirectlyToNumber")]
   public int GoDirectlyToNumber { get; set; }

   #endregion
}