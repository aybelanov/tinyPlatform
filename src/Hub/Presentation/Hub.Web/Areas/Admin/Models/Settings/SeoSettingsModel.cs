using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a SEO settings model
   /// </summary>
   public partial record SeoSettingsModel : BaseAppModel, ISettingsModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
      public string PageTitleSeparator { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
      public int PageTitleSeoAdjustment { get; set; }
      public SelectList PageTitleSeoAdjustmentValues { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HomepageTitle")]
      public string HomepageTitle { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HomepageDescription")]
      public string HomepageDescription { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
      public string DefaultTitle { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
      public string DefaultMetaKeywords { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
      public string DefaultMetaDescription { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GenerateMetaDescription")]
      public bool GenerateMetaDescription { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
      public bool ConvertNonWesternChars { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
      public bool CanonicalUrlsEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.WwwRequirement")]
      public int WwwRequirement { get; set; }
      public SelectList WwwRequirementValues { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags")]
      public bool TwitterMetaTags { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags")]
      public bool OpenGraphMetaTags { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CustomHeadTags")]
      public string CustomHeadTags { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Microdata")]
      public bool MicrodataEnabled { get; set; }

      #endregion
   }
}