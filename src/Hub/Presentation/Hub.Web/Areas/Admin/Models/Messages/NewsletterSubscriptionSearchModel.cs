using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription search model
/// </summary>
public partial record NewsletterSubscriptionSearchModel : BaseSearchModel
{
   #region Ctor

   public NewsletterSubscriptionSearchModel()
   {
      ActiveList = new List<SelectListItem>();
      AvailableUserRoles = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.List.SearchEmail")]
   public string SearchEmail { get; set; }

   [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.List.SearchActive")]
   public int ActiveId { get; set; }

   [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.List.SearchActive")]
   public IList<SelectListItem> ActiveList { get; set; }

   [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.List.UserRoles")]
   public long UserRoleId { get; set; }

   public IList<SelectListItem> AvailableUserRoles { get; set; }

   [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.List.StartDate")]
   [UIHint("DateNullable")]
   public DateTime? StartDate { get; set; }

   [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.List.EndDate")]
   [UIHint("DateNullable")]
   public DateTime? EndDate { get; set; }

   #endregion
}