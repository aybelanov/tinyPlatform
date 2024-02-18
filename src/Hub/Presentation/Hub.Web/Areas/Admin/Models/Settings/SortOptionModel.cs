using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a sort option model
   /// </summary>
   public partial record SortOptionModel : BaseAppEntityModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.IsActive")]
      public bool IsActive { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.DisplayOrder")]
      public int DisplayOrder { get; set; }

      #endregion
   }
}