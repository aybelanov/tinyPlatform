using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum search model
/// </summary>
public partial record ForumSearchModel : BaseSearchModel
{
   #region Properties

   public long ForumGroupId { get; set; }

   #endregion
}