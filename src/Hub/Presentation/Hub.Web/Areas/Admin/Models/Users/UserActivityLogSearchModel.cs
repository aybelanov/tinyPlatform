using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Users
{
   /// <summary>
   /// Represents a user activity log search model
   /// </summary>
   public partial record UserActivityLogSearchModel : BaseSearchModel
   {
      #region Properties

      public long UserId { get; set; }

      #endregion
   }
}