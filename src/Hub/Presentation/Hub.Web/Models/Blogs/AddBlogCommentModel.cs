using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Blogs;

public partial record AddBlogCommentModel : BaseAppEntityModel
{
   [AppResourceDisplayName("Blog.Comments.CommentText")]
   public string CommentText { get; set; }

   public bool DisplayCaptcha { get; set; }
}