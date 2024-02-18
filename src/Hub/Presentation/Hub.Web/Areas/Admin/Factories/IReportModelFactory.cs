using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Models.Reports;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the report model factory
/// </summary>
public partial interface IReportModelFactory
{
   #region User reports

   /// <summary>
   /// Prepare user reports search model
   /// </summary>
   /// <param name="searchModel">User reports search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user reports search model
   /// </returns>
   Task<UserReportsSearchModel> PrepareUserReportsSearchModelAsync(UserReportsSearchModel searchModel);

   /// <summary>
   /// Prepare paged registered users report list model
   /// </summary>
   /// <param name="searchModel">Registered users report search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the registered users report list model
   /// </returns>
   Task<RegisteredUsersReportListModel> PrepareRegisteredUsersReportListModelAsync(RegisteredUsersReportSearchModel searchModel);

   #endregion
}
