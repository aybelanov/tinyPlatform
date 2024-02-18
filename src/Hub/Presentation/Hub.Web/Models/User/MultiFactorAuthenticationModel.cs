using System.Collections.Generic;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Models.User;

public partial record MultiFactorAuthenticationModel : BaseAppModel
{
   public MultiFactorAuthenticationModel()
   {
      Providers = new List<MultiFactorAuthenticationProviderModel>();
   }

   [AppResourceDisplayName("Account.MultiFactorAuthentication.Fields.IsEnabled")]
   public bool IsEnabled { get; set; }

   public List<MultiFactorAuthenticationProviderModel> Providers { get; set; }

   public string Message { get; set; }

}
