using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Directory
{
   /// <summary>
   /// Represents a country model
   /// </summary>
   public partial record CountryModel : BaseAppEntityModel, ILocalizedModel<CountryLocalizedModel>
   {
      #region Ctor

      public CountryModel()
      {
         Locales = new List<CountryLocalizedModel>();
         StateProvinceSearchModel = new StateProvinceSearchModel();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsBilling")]
      public bool AllowsBilling { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.TwoLetterIsoCode")]
      public string TwoLetterIsoCode { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode")]
      public string ThreeLetterIsoCode { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.NumericIsoCode")]
      public int NumericIsoCode { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.Published")]
      public bool Published { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.NumberOfStates")]
      public int NumberOfStates { get; set; }

      public IList<CountryLocalizedModel> Locales { get; set; }

      public StateProvinceSearchModel StateProvinceSearchModel { get; set; }

      #endregion
   }

   public partial record CountryLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
      public string Name { get; set; }
   }
}