using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a country list model
/// </summary>
public partial record CountryListModel : BasePagedListModel<CountryModel>
{
}