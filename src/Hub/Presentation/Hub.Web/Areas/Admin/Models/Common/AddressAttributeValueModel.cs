using System.Collections.Generic;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute value model
/// </summary>
public partial record AddressAttributeValueModel : BaseAppEntityModel, ILocalizedModel<AddressAttributeValueLocalizedModel>
{
   #region Ctor

   public AddressAttributeValueModel()
   {
      Locales = new List<AddressAttributeValueLocalizedModel>();
   }

   #endregion

   #region Properties

   public long AddressAttributeId { get; set; }

   [AppResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.IsPreSelected")]
   public bool IsPreSelected { get; set; }

   [AppResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   public IList<AddressAttributeValueLocalizedModel> Locales { get; set; }

   #endregion
}

public partial record AddressAttributeValueLocalizedModel : ILocalizedLocaleModel
{
   public long LanguageId { get; set; }

   [AppResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
   public string Name { get; set; }
}