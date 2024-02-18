using System;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.PrivateMessages;

public partial record PrivateMessageModel : BaseAppEntityModel
{
   public long FromUserId { get; set; }
   public string UserFromName { get; set; }
   public bool AllowViewingFromProfile { get; set; }

   public long ToUserId { get; set; }
   public string UserToName { get; set; }
   public bool AllowViewingToProfile { get; set; }

   public string Subject { get; set; }

   public string Message { get; set; }

   public DateTime CreatedOn { get; set; }

   public bool IsRead { get; set; }
}