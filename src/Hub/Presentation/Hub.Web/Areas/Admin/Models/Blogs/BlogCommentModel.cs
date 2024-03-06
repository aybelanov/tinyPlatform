using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;

namespace Hub.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog comment model
/// </summary>
public partial record BlogCommentModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.BlogPost")]
   public long BlogPostId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.BlogPost")]
   public string BlogPostTitle { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.User")]
   public long UserId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.User")]
   public string UserInfo { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.Comment")]
   public string Comment { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.IsApproved")]
   public bool IsApproved { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   #endregion
}