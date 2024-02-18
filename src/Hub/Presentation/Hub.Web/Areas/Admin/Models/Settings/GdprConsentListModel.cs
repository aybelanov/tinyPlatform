using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a GDPR consent list model
/// </summary>
public partial record GdprConsentListModel : BasePagedListModel<GdprConsentModel>
{
}