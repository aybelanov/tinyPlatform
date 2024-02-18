using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Forums
{
   /// <summary>
   /// Represents a forum group search model
   /// </summary>
   public partial record ForumGroupSearchModel : BaseSearchModel
   {
      #region Ctor

      public ForumGroupSearchModel()
      {
         ForumSearch = new ForumSearchModel();
      }

      #endregion

      #region Properties

      public ForumSearchModel ForumSearch { get; set; }

      #endregion
   }
}