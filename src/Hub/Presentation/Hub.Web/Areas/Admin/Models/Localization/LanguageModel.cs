using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Localization
{
   /// <summary>
   /// Represents a language model
   /// </summary>
   public partial record LanguageModel : BaseAppEntityModel
   {
      #region Ctor

      public LanguageModel()
      {
         AvailableCurrencies = new List<SelectListItem>();
         LocaleResourceSearchModel = new LocaleResourceSearchModel();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
      public string LanguageCulture { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
      public string UniqueSeoCode { get; set; }

      //flags
      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
      public string FlagImageFileName { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
      public bool Rtl { get; set; }

      //default currency
      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.DefaultCurrency")]
      public long DefaultCurrencyId { get; set; }

      public IList<SelectListItem> AvailableCurrencies { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
      public bool Published { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      // search
      public LocaleResourceSearchModel LocaleResourceSearchModel { get; set; }

      #endregion
   }
}