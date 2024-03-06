using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Models.Blogs;

public partial record BlogPostYearModel : BaseAppModel
{
   public BlogPostYearModel()
   {
      Months = new List<BlogPostMonthModel>();
   }
   public int Year { get; set; }
   public IList<BlogPostMonthModel> Months { get; set; }
}

public partial record BlogPostMonthModel : BaseAppModel
{
   public int Month { get; set; }

   public int BlogPostCount { get; set; }
}