using System.Collections.Generic;
using Hub.Web.Models.Common;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.PrivateMessages
{
   public partial record PrivateMessageListModel : BaseAppModel
   {
      public IList<PrivateMessageModel> Messages { get; set; }
      public PagerModel PagerModel { get; set; }
   }
}