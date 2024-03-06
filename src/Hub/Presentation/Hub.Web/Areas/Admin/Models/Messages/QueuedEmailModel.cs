using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a queued email model
/// </summary>
public partial record QueuedEmailModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.Id")]
   public override long Id { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.Priority")]
   public string PriorityName { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.From")]
   public string From { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.FromName")]
   public string FromName { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.To")]
   public string To { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.ToName")]
   public string ToName { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyTo")]
   public string ReplyTo { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyToName")]
   public string ReplyToName { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.CC")]
   public string CC { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.Bcc")]
   public string Bcc { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.Subject")]
   public string Subject { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.Body")]
   public string Body { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.AttachmentFilePath")]
   public string AttachmentFilePath { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.AttachedDownload")]
   [UIHint("Download")]
   public long AttachedDownloadId { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.SendImmediately")]
   public bool SendImmediately { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.DontSendBeforeDate")]
   [UIHint("DateTimeNullable")]
   public DateTime? DontSendBeforeDate { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.SentTries")]
   public int SentTries { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.SentOn")]
   public DateTime? SentOn { get; set; }

   [AppResourceDisplayName("Admin.System.QueuedEmails.Fields.EmailAccountName")]
   public string EmailAccountName { get; set; }

   #endregion
}