using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.News;

public partial record HomepageNewsItemsModel : BaseAppModel
{
   public HomepageNewsItemsModel()
   {
      NewsItems = new List<NewsItemModel>();
   }

   public long WorkingLanguageId { get; set; }
   public IList<NewsItemModel> NewsItems { get; set; }
}