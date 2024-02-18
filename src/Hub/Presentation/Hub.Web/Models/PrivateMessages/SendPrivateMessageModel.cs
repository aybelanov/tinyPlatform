using Hub.Web.Framework.Models;

namespace Hub.Web.Models.PrivateMessages;

public partial record SendPrivateMessageModel : BaseAppEntityModel
{
   public long ToUserId { get; set; }
   public string UserToName { get; set; }
   public bool AllowViewingToProfile { get; set; }

   public long ReplyToMessageId { get; set; }

   public string Subject { get; set; }

   public string Message { get; set; }
}