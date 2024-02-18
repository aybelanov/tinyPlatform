using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Newsletter
{
   public partial record SubscriptionActivationModel : BaseAppModel
   {
      public string Result { get; set; }
   }
}