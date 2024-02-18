using System;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.News
{
   public partial record NewsCommentModel : BaseAppEntityModel
   {
      public long UserId { get; set; }

      public string UserName { get; set; }

      public string UserAvatarUrl { get; set; }

      public string CommentTitle { get; set; }

      public string CommentText { get; set; }

      public DateTime CreatedOn { get; set; }

      public bool AllowViewingProfiles { get; set; }
   }
}