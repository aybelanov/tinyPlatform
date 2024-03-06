using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news item model
/// </summary>
public partial record NewsItemModel : BaseAppEntityModel
{
   #region Ctor

   public NewsItemModel()
   {
      AvailableLanguages = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Language")]
   public long LanguageId { get; set; }

   public IList<SelectListItem> AvailableLanguages { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Language")]
   public string LanguageName { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Title")]
   public string Title { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Short")]
   public string Short { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Full")]
   public string Full { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.AllowComments")]
   public bool AllowComments { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.StartDate")]
   [UIHint("DateTimeNullable")]
   public DateTime? StartDateUtc { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.EndDate")]
   [UIHint("DateTimeNullable")]
   public DateTime? EndDateUtc { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.MetaKeywords")]
   public string MetaKeywords { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.MetaDescription")]
   public string MetaDescription { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.MetaTitle")]
   public string MetaTitle { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.SeName")]
   public string SeName { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Published")]
   public bool Published { get; set; }

   public int ApprovedComments { get; set; }

   public int NotApprovedComments { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   #endregion
}