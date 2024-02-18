using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Plugins;

/// <summary>
/// Represents a plugin search model
/// </summary>
public partial record PluginSearchModel : BaseSearchModel
{
   #region Ctor

   public PluginSearchModel()
   {
      AvailableLoadModes = new List<SelectListItem>();
      AvailableGroups = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
   public int SearchLoadModeId { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Plugins.Group")]
   public string SearchGroup { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Plugins.FriendlyName")]
   public string SearchFriendlyName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Plugins.Author")]
   public string SearchAuthor { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
   public IList<SelectListItem> AvailableLoadModes { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Plugins.Group")]
   public IList<SelectListItem> AvailableGroups { get; set; }

   public bool NeedToRestart { get; set; }

   #endregion
}