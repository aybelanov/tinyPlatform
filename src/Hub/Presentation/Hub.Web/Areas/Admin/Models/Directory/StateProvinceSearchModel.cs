using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a state and province search model
/// </summary>
public partial record StateProvinceSearchModel : BaseSearchModel
{
   #region Properties

   public long CountryId { get; set; }

   #endregion
}