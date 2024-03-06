using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Topics;

/// <summary>
/// Represents a topic search model
/// </summary>
public partial record TopicSearchModel : BaseSearchModel
{
   #region Ctor

   public TopicSearchModel()
   {
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.Topics.List.SearchKeywords")]
   public string SearchKeywords { get; set; }


   #endregion
}