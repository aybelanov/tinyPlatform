using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record AccountActivationModel : BaseAppModel
   {
      public string Result { get; set; }

      public string ReturnUrl { get; set; }
   }
}