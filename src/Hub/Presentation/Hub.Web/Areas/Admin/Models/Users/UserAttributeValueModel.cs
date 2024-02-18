using System.Collections.Generic;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user attribute value model
/// </summary>
public partial record UserAttributeValueModel : BaseAppEntityModel, ILocalizedModel<UserAttributeValueLocalizedModel>
{
   #region Ctor

   public UserAttributeValueModel()
   {
      Locales = new List<UserAttributeValueLocalizedModel>();
   }

   #endregion

   #region Properties

   public long UserAttributeId { get; set; }

   [AppResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.IsPreSelected")]
   public bool IsPreSelected { get; set; }

   [AppResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   public IList<UserAttributeValueLocalizedModel> Locales { get; set; }

   #endregion
}

public partial record UserAttributeValueLocalizedModel : ILocalizedLocaleModel
{
   public long LanguageId { get; set; }

   [AppResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
   public string Name { get; set; }
}