using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Models.Boards;

public partial record ForumGroupModel : BaseAppModel
{
   public ForumGroupModel()
   {
      Forums = new List<ForumRowModel>();
   }
   public long Id { get; set; }
   public string Name { get; set; }
   public string SeName { get; set; }

   public IList<ForumRowModel> Forums { get; set; }
}