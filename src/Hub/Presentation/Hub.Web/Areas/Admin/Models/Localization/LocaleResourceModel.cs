using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Localization;

/// <summary>
/// Represents a locale resource model
/// </summary>
public partial record LocaleResourceModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
   public string ResourceName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
   public string ResourceValue { get; set; }

   public long LanguageId { get; set; }

   #endregion
}