using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents a newsletter subscription model
   /// </summary>
   public partial record NewsletterSubscriptionModel : BaseAppEntityModel
   {
      #region Properties

      [DataType(DataType.EmailAddress)]
      [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.Fields.Email")]
      public string Email { get; set; }

      [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.Fields.Active")]
      public bool Active { get; set; }

      [AppResourceDisplayName("Admin.Milticast.NewsLetterSubscriptions.Fields.CreatedOn")]
      public string CreatedOn { get; set; }

      #endregion
   }
}