using System.Collections.Generic;
using Hub.Web.Models.Common;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Profile
{
   public partial record ProfilePostsModel : BaseAppModel
   {
      public IList<PostsModel> Posts { get; set; }
      public PagerModel PagerModel { get; set; }
   }
}