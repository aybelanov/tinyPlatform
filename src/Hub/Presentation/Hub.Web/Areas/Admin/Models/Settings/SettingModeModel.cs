using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a setting mode model
   /// </summary>
   public partial record SettingModeModel : BaseAppModel
   {
      #region Properties

      public string ModeName { get; set; }

      public bool Enabled { get; set; }

      #endregion
   }
}