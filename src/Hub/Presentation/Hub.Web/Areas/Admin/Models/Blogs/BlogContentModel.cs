using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog content model
/// </summary>
public partial record BlogContentModel : BaseAppModel
{
   #region Ctor

   public BlogContentModel()
   {
      BlogPosts = new BlogPostSearchModel();
      BlogComments = new BlogCommentSearchModel();
      SearchTitle = new BlogPostSearchModel().SearchTitle;
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle")]
   public string SearchTitle { get; set; }

   public BlogPostSearchModel BlogPosts { get; set; }

   public BlogCommentSearchModel BlogComments { get; set; }

   #endregion
}
