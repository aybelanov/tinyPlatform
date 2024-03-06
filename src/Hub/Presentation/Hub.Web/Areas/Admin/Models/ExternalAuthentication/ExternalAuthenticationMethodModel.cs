using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.ExternalAuthentication;

/// <summary>
/// Represents an external authentication method model
/// </summary>
public partial record ExternalAuthenticationMethodModel : BaseAppModel, IPluginModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Authentication.ExternalMethods.Fields.FriendlyName")]
   public string FriendlyName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.ExternalMethods.Fields.SystemName")]
   public string SystemName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.ExternalMethods.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.ExternalMethods.Fields.IsActive")]
   public bool IsActive { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.ExternalMethods.Configure")]
   public string ConfigurationUrl { get; set; }

   public string LogoUrl { get; set; }

   #endregion
}