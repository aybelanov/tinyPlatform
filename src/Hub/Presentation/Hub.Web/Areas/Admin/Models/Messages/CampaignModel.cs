using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a campaign model
/// </summary>
public partial record CampaignModel : BaseAppEntityModel
{
   #region Ctor

   public CampaignModel()
   {
      AvailableUserRoles = new List<SelectListItem>();
      AvailableEmailAccounts = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.Subject")]
   public string Subject { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.Body")]
   public string Body { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.UserRole")]
   public long UserRoleId { get; set; }
   public IList<SelectListItem> AvailableUserRoles { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.DontSendBeforeDate")]
   [UIHint("DateTimeNullable")]
   public DateTime? DontSendBeforeDate { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.AllowedTokens")]
   public string AllowedTokens { get; set; }

   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.EmailAccount")]
   public long EmailAccountId { get; set; }
   public IList<SelectListItem> AvailableEmailAccounts { get; set; }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Milticast.Campaigns.Fields.TestEmail")]
   public string TestEmail { get; set; }

   #endregion
}