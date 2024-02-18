using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Reports
{
   /// <summary>
   /// Represents a user reports search model
   /// </summary>
   public partial record UserReportsSearchModel : BaseSearchModel
   {
      #region Ctor

      public UserReportsSearchModel()
      {
         RegisteredUsers = new RegisteredUsersReportSearchModel();
      }

      #endregion

      #region Properties

      public RegisteredUsersReportSearchModel RegisteredUsers { get; set; }

      #endregion
   }
}