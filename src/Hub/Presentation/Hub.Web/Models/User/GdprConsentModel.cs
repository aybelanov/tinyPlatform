using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record GdprConsentModel : BaseAppEntityModel
   {
      public string Message { get; set; }

      public bool IsRequired { get; set; }

      public string RequiredMessage { get; set; }

      public bool Accepted { get; set; }
   }
}