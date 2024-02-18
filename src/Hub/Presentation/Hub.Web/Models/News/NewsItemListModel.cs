using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.News
{
   public partial record NewsItemListModel : BaseAppModel
   {
      public NewsItemListModel()
      {
         PagingFilteringContext = new NewsPagingFilteringModel();
         NewsItems = new List<NewsItemModel>();
      }

      public long WorkingLanguageId { get; set; }
      public NewsPagingFilteringModel PagingFilteringContext { get; set; }
      public IList<NewsItemModel> NewsItems { get; set; }
   }
}