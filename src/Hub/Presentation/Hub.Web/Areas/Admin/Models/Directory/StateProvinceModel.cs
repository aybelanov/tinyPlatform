using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Directory
{
   /// <summary>
   /// Represents a state and province model
   /// </summary>
   public partial record StateProvinceModel : BaseAppEntityModel, ILocalizedModel<StateProvinceLocalizedModel>
   {
      #region Ctor

      public StateProvinceModel()
      {
         Locales = new List<StateProvinceLocalizedModel>();
      }

      #endregion

      #region Properties

      public long CountryId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.States.Fields.Abbreviation")]
      public string Abbreviation { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
      public bool Published { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      public IList<StateProvinceLocalizedModel> Locales { get; set; }

      #endregion
   }

   public partial record StateProvinceLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
      public string Name { get; set; }
   }
}