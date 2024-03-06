using Hub.Web.Framework.Models;
using Hub.Web.Models.Common;
using System.Collections.Generic;

namespace Hub.Web.Models.PrivateMessages
{
   public partial record PrivateMessageListModel : BaseAppModel
   {
      public IList<PrivateMessageModel> Messages { get; set; }
      public PagerModel PagerModel { get; set; }
   }
}