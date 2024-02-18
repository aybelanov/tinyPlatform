using Hub.Core.Domain.Forums;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Boards;

public partial record ForumTopicRowModel : BaseAppModel
{
   public long Id { get; set; }
   public string Subject { get; set; }
   public string SeName { get; set; }
   public long LastPostId { get; set; }

   public int NumPosts { get; set; }
   public int Views { get; set; }
   public int Votes { get; set; }
   public int NumReplies { get; set; }
   public ForumTopicType ForumTopicType { get; set; }

   public long UserId { get; set; }
   public bool AllowViewingProfiles { get; set; }
   public string UserName { get; set; }

   //posts
   public int TotalPostPages { get; set; }
}