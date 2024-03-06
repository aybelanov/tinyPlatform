using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Users
{
   /// <summary>
   /// Represents a user attribute model
   /// </summary>
   public partial record UserAttributeModel : BaseAppEntityModel, ILocalizedModel<UserAttributeLocalizedModel>
   {
      #region Ctor

      public UserAttributeModel()
      {
         Locales = new List<UserAttributeLocalizedModel>();
         UserAttributeValueSearchModel = new UserAttributeValueSearchModel();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.Users.UserAttributes.Fields.IsRequired")]
      public bool IsRequired { get; set; }

      [AppResourceDisplayName("Admin.Users.UserAttributes.Fields.AttributeControlType")]
      public int AttributeControlTypeId { get; set; }

      [AppResourceDisplayName("Admin.Users.UserAttributes.Fields.AttributeControlType")]
      public string AttributeControlTypeName { get; set; }

      [AppResourceDisplayName("Admin.Users.UserAttributes.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      public IList<UserAttributeLocalizedModel> Locales { get; set; }

      public UserAttributeValueSearchModel UserAttributeValueSearchModel { get; set; }

      #endregion
   }

   public partial record UserAttributeLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
      public string Name { get; set; }
   }
}