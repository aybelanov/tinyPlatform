using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Plugins
{
   /// <summary>
   /// Represents a plugin list model
   /// </summary>
   public partial record PluginListModel : BasePagedListModel<PluginModel>
   {
   }
}