using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Models.Common;

public partial record SitemapModel : BaseAppModel
{
   #region Ctor

   public SitemapModel()
   {
      Items = new List<SitemapItemModel>();
      PageModel = new SitemapPageModel();
   }

   #endregion

   #region Properties

   public List<SitemapItemModel> Items { get; set; }

   public SitemapPageModel PageModel { get; set; }

   #endregion

   #region Nested classes

   public record SitemapItemModel
   {
      public string GroupTitle { get; set; }
      public string Url { get; set; }
      public string Name { get; set; }
   }

   #endregion
}