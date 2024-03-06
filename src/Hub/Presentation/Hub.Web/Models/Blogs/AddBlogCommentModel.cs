using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Models.Blogs;

public partial record AddBlogCommentModel : BaseAppEntityModel
{
   [AppResourceDisplayName("Blog.Comments.CommentText")]
   public string CommentText { get; set; }

   public bool DisplayCaptcha { get; set; }
}