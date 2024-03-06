using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a measure weight model
/// </summary>
public partial record MeasureWeightModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword")]
   public string SystemKeyword { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio")]
   public decimal Ratio { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.IsPrimaryWeight")]
   public bool IsPrimaryWeight { get; set; }

   #endregion
}