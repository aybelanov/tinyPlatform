using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Plugins.Marketplace
{
   /// <summary>
   /// Represents a list model of plugins of the official feed
   /// </summary>
   public partial record OfficialFeedPluginListModel : BasePagedListModel<OfficialFeedPluginModel>
   {
   }
}