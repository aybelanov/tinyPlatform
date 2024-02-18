using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a message template model
/// </summary>
public partial record MessageTemplateModel : BaseAppEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
{
   #region Ctor

   public MessageTemplateModel()
   {
      Locales = new List<MessageTemplateLocalizedModel>();
      AvailableEmailAccounts = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
   public string AllowedTokens { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
   public string BccEmailAddresses { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
   public string Subject { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
   public string Body { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
   public bool IsActive { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.SendImmediately")]
   public bool SendImmediately { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend")]
   [UIHint("Int32Nullable")]
   public int? DelayBeforeSend { get; set; }

   public int DelayPeriodId { get; set; }

   public bool HasAttachedDownload { get; set; }
   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
   [UIHint("Download")]
   public long AttachedDownloadId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
   public long EmailAccountId { get; set; }

   public IList<SelectListItem> AvailableEmailAccounts { get; set; }


   public IList<MessageTemplateLocalizedModel> Locales { get; set; }

   #endregion
}

public partial record MessageTemplateLocalizedModel : ILocalizedLocaleModel
{
   public MessageTemplateLocalizedModel()
   {
      AvailableEmailAccounts = new List<SelectListItem>();
   }

   public long LanguageId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
   public string BccEmailAddresses { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
   public string Subject { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
   public string Body { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
   public long EmailAccountId { get; set; }

   public IList<SelectListItem> AvailableEmailAccounts { get; set; }
}