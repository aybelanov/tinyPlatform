using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Directory
{
   /// <summary>
   /// Represents a currency model
   /// </summary>
   public partial record CurrencyModel : BaseAppEntityModel, ILocalizedModel<CurrencyLocalizedModel>
   {
      #region Ctor

      public CurrencyModel()
      {
         Locales = new List<CurrencyLocalizedModel>();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyCode")]
      public string CurrencyCode { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayLocale")]
      public string DisplayLocale { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.Rate")]
      public decimal Rate { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.CustomFormatting")]
      public string CustomFormatting { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.Published")]
      public bool Published { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.CreatedOn")]
      public DateTime CreatedOn { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
      public bool IsPrimaryExchangeRateCurrency { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryPlatformCurrency")]
      public bool IsPrimaryPlatformCurrency { get; set; }

      public IList<CurrencyLocalizedModel> Locales { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.RoundingType")]
      public int RoundingTypeId { get; set; }

      #endregion
   }

   public partial record CurrencyLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
      public string Name { get; set; }
   }
}