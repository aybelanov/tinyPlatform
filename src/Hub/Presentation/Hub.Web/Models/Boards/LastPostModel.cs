using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Boards;

public partial record LastPostModel : BaseAppModel
{
   public long Id { get; set; }
   public long ForumTopicId { get; set; }
   public string ForumTopicSeName { get; set; }
   public string ForumTopicSubject { get; set; }

   public long UserId { get; set; }
   public bool AllowViewingProfiles { get; set; }
   public string UserName { get; set; }

   public string PostCreatedOnStr { get; set; }

   public bool ShowTopic { get; set; }
}