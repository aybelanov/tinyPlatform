using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record RegisterResultModel : BaseAppModel
   {
      public string Result { get; set; }

      public string ReturnUrl { get; set; }
   }
}