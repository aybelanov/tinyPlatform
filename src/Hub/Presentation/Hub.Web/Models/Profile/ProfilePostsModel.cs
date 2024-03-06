using Hub.Web.Framework.Models;
using Hub.Web.Models.Common;
using System.Collections.Generic;

namespace Hub.Web.Models.Profile
{
   public partial record ProfilePostsModel : BaseAppModel
   {
      public IList<PostsModel> Posts { get; set; }
      public PagerModel PagerModel { get; set; }
   }
}