using Hub.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Models.Newsletter
{
   public partial record NewsletterBoxModel : BaseAppModel
   {
      [DataType(DataType.EmailAddress)]
      public string NewsletterEmail { get; set; }
      public bool AllowToUnsubscribe { get; set; }
   }
}