using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents a campaign list model
   /// </summary>
   public partial record CampaignListModel : BasePagedListModel<CampaignModel>
   {
   }
}