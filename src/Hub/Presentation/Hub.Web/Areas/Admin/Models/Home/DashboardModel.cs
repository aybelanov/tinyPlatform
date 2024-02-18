using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Areas.Admin.Models.Reports;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Home;

/// <summary>
/// Represents a dashboard model
/// </summary>
public partial record DashboardModel : BaseAppModel
{
   #region Ctor

   public DashboardModel()
   {
      PopularSearchTerms = new PopularSearchTermSearchModel();
   }

   #endregion

   #region Properties

   public PopularSearchTermSearchModel PopularSearchTerms { get; set; }


   #endregion
}