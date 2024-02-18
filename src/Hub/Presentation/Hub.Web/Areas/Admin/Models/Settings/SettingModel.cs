using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a setting model
   /// </summary>
   public partial record SettingModel : BaseAppEntityModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
      public string Value { get; set; }

      #endregion
   }
}