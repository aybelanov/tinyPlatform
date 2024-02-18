using Hub.Web.Areas.Admin.Models.Plugins.Marketplace;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Plugins;

/// <summary>
/// Represents a plugins configuration model
/// </summary>
public partial record PluginsConfigurationModel : BaseAppModel
{
   #region Ctor

   public PluginsConfigurationModel()
   {
      PluginsLocal = new PluginSearchModel();
      AllPluginsAndThemes = new OfficialFeedPluginSearchModel();
   }

   #endregion

   #region Properties

   public PluginSearchModel PluginsLocal { get; set; }

   public OfficialFeedPluginSearchModel AllPluginsAndThemes { get; set; }

   #endregion
}
