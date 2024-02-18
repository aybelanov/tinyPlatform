using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news content model
/// </summary>
public partial record NewsContentModel : BaseAppModel
{
   #region Ctor

   public NewsContentModel()
   {
      NewsItems = new NewsItemSearchModel();
      NewsComments = new NewsCommentSearchModel();
      SearchTitle = new NewsItemSearchModel().SearchTitle;
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchTitle")]
   public string SearchTitle { get; set; }

   public NewsItemSearchModel NewsItems { get; set; }

   public NewsCommentSearchModel NewsComments { get; set; }

   #endregion
}
