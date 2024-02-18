using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog post model
/// </summary>
public partial record BlogPostModel : BaseAppEntityModel
{
   #region Ctor

   public BlogPostModel()
   {
      AvailableLanguages = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
   public long LanguageId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.IncludeInSitemap")]
   public bool IncludeInSitemap { get; set; }

   public IList<SelectListItem> AvailableLanguages { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
   public string LanguageName { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Title")]
   public string Title { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Body")]
   public string Body { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.BodyOverview")]
   public string BodyOverview { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.AllowComments")]
   public bool AllowComments { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags")]
   public string Tags { get; set; }

   public string InitialBlogTags { get; set; }

   public int ApprovedComments { get; set; }

   public int NotApprovedComments { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate")]
   [UIHint("DateTimeNullable")]
   public DateTime? StartDateUtc { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate")]
   [UIHint("DateTimeNullable")]
   public DateTime? EndDateUtc { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaKeywords")]
   public string MetaKeywords { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaDescription")]
   public string MetaDescription { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaTitle")]
   public string MetaTitle { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.SeName")]
   public string SeName { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   #endregion
}