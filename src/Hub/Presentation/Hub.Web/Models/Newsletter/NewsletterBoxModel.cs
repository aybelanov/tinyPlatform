using System.ComponentModel.DataAnnotations;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Newsletter
{
   public partial record NewsletterBoxModel : BaseAppModel
   {
      [DataType(DataType.EmailAddress)]
      public string NewsletterEmail { get; set; }
      public bool AllowToUnsubscribe { get; set; }
   }
}