using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Models.News
{
   public partial record AddNewsCommentModel : BaseAppModel
   {
      [AppResourceDisplayName("News.Comments.CommentTitle")]
      public string CommentTitle { get; set; }

      [AppResourceDisplayName("News.Comments.CommentText")]
      public string CommentText { get; set; }

      public bool DisplayCaptcha { get; set; }
   }
}