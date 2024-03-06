using Hub.Core.Domain.Common;
using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Models.User
{
   public partial record UserAttributeModel : BaseAppEntityModel
   {
      public UserAttributeModel()
      {
         Values = new List<UserAttributeValueModel>();
      }

      public string Name { get; set; }

      public bool IsRequired { get; set; }

      /// <summary>
      /// Default value for textboxes
      /// </summary>
      public string DefaultValue { get; set; }

      public AttributeControlType AttributeControlType { get; set; }

      public IList<UserAttributeValueModel> Values { get; set; }

   }

   public partial record UserAttributeValueModel : BaseAppEntityModel
   {
      public string Name { get; set; }

      public bool IsPreSelected { get; set; }
   }
}