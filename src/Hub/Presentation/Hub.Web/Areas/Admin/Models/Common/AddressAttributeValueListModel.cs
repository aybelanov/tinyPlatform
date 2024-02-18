using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute value list model
/// </summary>
public record AddressAttributeValueListModel : BasePagedListModel<AddressAttributeValueModel>
{
}