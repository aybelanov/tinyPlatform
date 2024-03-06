using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Plugins
{
   /// <summary>
   /// Represents a plugin model
   /// </summary>
   public partial record PluginModel : BaseAppModel, IAclSupportedModel, ILocalizedModel<PluginLocalizedModel>, IPluginModel
   {
      #region Ctor

      public PluginModel()
      {
         Locales = new List<PluginLocalizedModel>();
         SelectedUserRoleIds = new List<long>();
         AvailableUserRoles = new List<SelectListItem>();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
      public string Group { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
      public string FriendlyName { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
      public string SystemName { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
      public string Version { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
      public string Author { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
      public string ConfigurationUrl { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
      public bool Installed { get; set; }

      public string Description { get; set; }

      public bool CanChangeEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.IsEnabled")]
      public bool IsEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.Logo")]
      public string LogoUrl { get; set; }

      public IList<PluginLocalizedModel> Locales { get; set; }

      //ACL (user roles)
      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.AclUserRoles")]
      public IList<long> SelectedUserRoleIds { get; set; }

      public IList<SelectListItem> AvailableUserRoles { get; set; }

      public bool IsActive { get; set; }

      #endregion
   }

   public partial record PluginLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
      public string FriendlyName { get; set; }
   }
}