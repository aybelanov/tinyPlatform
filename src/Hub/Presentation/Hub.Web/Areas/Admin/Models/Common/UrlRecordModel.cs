using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an URL record model
/// </summary>
public partial record UrlRecordModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.System.SeNames.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.EntityId")]
   public long EntityId { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.EntityName")]
   public string EntityName { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.IsActive")]
   public bool IsActive { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.Language")]
   public string Language { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.Details")]
   public string DetailsUrl { get; set; }

   #endregion
}