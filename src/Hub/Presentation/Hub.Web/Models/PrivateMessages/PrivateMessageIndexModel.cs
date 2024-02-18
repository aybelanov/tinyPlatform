using Hub.Web.Framework.Models;

namespace Hub.Web.Models.PrivateMessages
{
   public partial record PrivateMessageIndexModel : BaseAppModel
   {
      public int InboxPage { get; set; }
      public int SentItemsPage { get; set; }
      public bool SentItemsTabSelected { get; set; }
   }
}