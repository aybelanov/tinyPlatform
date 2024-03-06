using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a platform information settings model
   /// </summary>
   public partial record AppInformationSettingsModel : BaseAppModel, ISettingsModel
   {
      #region Ctor

      public AppInformationSettingsModel()
      {
         AvailableApplicationThemes = new List<ThemeModel>();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PlatformClosed")]
      public bool PlatformClosed { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultPlatformTheme")]
      public string DefaultPlatformTheme { get; set; }
      public IList<ThemeModel> AvailableApplicationThemes { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowUserToSelectTheme")]
      public bool AllowUserToSelectTheme { get; set; }

      [UIHint("Picture")]
      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Logo")]
      public long LogoPictureId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning")]
      public bool DisplayEuCookieLawWarning { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookLink")]
      public string FacebookLink { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterLink")]
      public string TwitterLink { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.YoutubeLink")]
      public string YoutubeLink { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
      public bool SubjectFieldOnContactUsForm { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
      public bool UseSystemEmailForContactUsForm { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PopupForTermsOfServiceLinks")]
      public bool PopupForTermsOfServiceLinks { get; set; }

      #endregion

      #region Nested classes

      public partial record ThemeModel
      {
         public string SystemName { get; set; }
         public string FriendlyName { get; set; }
         public string PreviewImageUrl { get; set; }
         public string PreviewText { get; set; }
         public bool SupportRtl { get; set; }
         public bool Selected { get; set; }
      }

      #endregion
   }
}