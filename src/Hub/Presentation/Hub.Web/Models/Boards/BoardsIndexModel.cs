using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Boards;

public partial record BoardsIndexModel : BaseAppModel
{
   public BoardsIndexModel()
   {
      ForumGroups = new List<ForumGroupModel>();
   }

   public IList<ForumGroupModel> ForumGroups { get; set; }
}