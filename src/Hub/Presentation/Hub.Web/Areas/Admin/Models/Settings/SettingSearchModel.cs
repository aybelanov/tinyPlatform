using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a setting search model
   /// </summary>
   public partial record SettingSearchModel : BaseSearchModel
   {
      #region Ctor

      public SettingSearchModel()
      {
         AddSetting = new SettingModel();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingName")]
      public string SearchSettingName { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingValue")]
      public string SearchSettingValue { get; set; }

      public SettingModel AddSetting { get; set; }

      #endregion
   }
}