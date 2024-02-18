using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record ExternalAuthenticationMethodModel : BaseAppModel
   {
      public string ViewComponentName { get; set; }
   }
}