using System;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Blogs;

public partial record BlogCommentModel : BaseAppEntityModel
{
   public long UserId { get; set; }

   public string UserName { get; set; }

   public string UserAvatarUrl { get; set; }

   public string CommentText { get; set; }

   public DateTime CreatedOn { get; set; }

   public bool AllowViewingProfiles { get; set; }
}