using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;

namespace Hub.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news comment model
/// </summary>
public partial record NewsCommentModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.NewsItem")]
   public long NewsItemId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.NewsItem")]
   public string NewsItemTitle { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.User")]
   public long UserId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.User")]
   public string UserInfo { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.CommentTitle")]
   public string CommentTitle { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.CommentText")]
   public string CommentText { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.IsApproved")]
   public bool IsApproved { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   #endregion
}