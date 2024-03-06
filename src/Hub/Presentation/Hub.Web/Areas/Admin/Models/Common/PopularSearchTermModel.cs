using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a popular search term model
/// </summary>
public partial record PopularSearchTermModel : BaseAppModel
{
   #region Properties

   [AppResourceDisplayName("Admin.SearchTermReport.Keyword")]
   public string Keyword { get; set; }

   [AppResourceDisplayName("Admin.SearchTermReport.Count")]
   public int Count { get; set; }

   #endregion
}
