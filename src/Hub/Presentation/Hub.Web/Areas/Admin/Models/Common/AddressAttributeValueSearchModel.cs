using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute value search model
/// </summary>
public partial record AddressAttributeValueSearchModel : BaseSearchModel
{
   #region Properties

   public long AddressAttributeId { get; set; }

   #endregion
}