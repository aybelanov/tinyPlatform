using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Boards;

public partial record TopicMoveModel : BaseAppEntityModel
{
   public TopicMoveModel()
   {
      ForumList = new List<SelectListItem>();
   }

   public long ForumSelected { get; set; }
   public string TopicSeName { get; set; }

   public IEnumerable<SelectListItem> ForumList { get; set; }
}