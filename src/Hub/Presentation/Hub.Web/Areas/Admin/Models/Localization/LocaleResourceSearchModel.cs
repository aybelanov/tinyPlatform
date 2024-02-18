using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Localization
{
   /// <summary>
   /// Represents a locale resource search model
   /// </summary>
   public partial record LocaleResourceSearchModel : BaseSearchModel
   {
      #region Ctor

      public LocaleResourceSearchModel()
      {
         AddResourceString = new LocaleResourceModel();
      }

      #endregion

      #region Properties

      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceName")]
      public string SearchResourceName { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceValue")]
      public string SearchResourceValue { get; set; }

      public LocaleResourceModel AddResourceString { get; set; }

      #endregion
   }
}