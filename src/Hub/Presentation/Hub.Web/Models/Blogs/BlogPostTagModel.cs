using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Blogs;

public partial record BlogPostTagModel : BaseAppModel
{
   public string Name { get; set; }

   public int BlogPostCount { get; set; }
}