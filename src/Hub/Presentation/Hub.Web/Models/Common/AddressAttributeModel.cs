using System.Collections.Generic;
using Hub.Core.Domain.Common;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record AddressAttributeModel : BaseAppEntityModel
{
   public AddressAttributeModel()
   {
      Values = new List<AddressAttributeValueModel>();
   }

   public string ControlId { get; set; }

   public string Name { get; set; }

   public bool IsRequired { get; set; }

   /// <summary>
   /// Default value for textboxes
   /// </summary>
   public string DefaultValue { get; set; }

   public AttributeControlType AttributeControlType { get; set; }

   public IList<AddressAttributeValueModel> Values { get; set; }
}

public partial record AddressAttributeValueModel : BaseAppEntityModel
{
   public string Name { get; set; }

   public bool IsPreSelected { get; set; }
}