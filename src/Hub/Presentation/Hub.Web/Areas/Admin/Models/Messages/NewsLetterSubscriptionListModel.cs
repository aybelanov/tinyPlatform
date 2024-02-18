using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents a newsletter subscription list model
   /// </summary>
   public partial record NewsletterSubscriptionListModel : BasePagedListModel<NewsletterSubscriptionModel>
   {
   }
}