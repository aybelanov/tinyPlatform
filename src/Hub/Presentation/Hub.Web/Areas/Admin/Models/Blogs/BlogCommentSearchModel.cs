using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog comment search model
/// </summary>
public partial record BlogCommentSearchModel : BaseSearchModel
{
   #region Ctor

   public BlogCommentSearchModel()
   {
      AvailableApprovedOptions = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   public long? BlogPostId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.CreatedOnFrom")]
   [UIHint("DateNullable")]
   public DateTime? CreatedOnFrom { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.CreatedOnTo")]
   [UIHint("DateNullable")]
   public DateTime? CreatedOnTo { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.SearchText")]
   public string SearchText { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.SearchApproved")]
   public long SearchApprovedId { get; set; }

   public IList<SelectListItem> AvailableApprovedOptions { get; set; }

   #endregion
}