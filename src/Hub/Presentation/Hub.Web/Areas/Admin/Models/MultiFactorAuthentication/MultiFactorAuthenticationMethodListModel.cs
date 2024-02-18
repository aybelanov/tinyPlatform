using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.MultiFactorAuthentication
{
   /// <summary>
   /// Represents an multi-factor authentication method list model
   /// </summary>
   public partial record MultiFactorAuthenticationMethodListModel : BasePagedListModel<MultiFactorAuthenticationMethodModel>
   {
   }
}
