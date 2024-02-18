using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.MultiFactorAuthentication;

/// <summary>
/// Represents an multi-factor authentication method model
/// </summary>
public partial record MultiFactorAuthenticationMethodModel : BaseAppModel, IPluginModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.FriendlyName")]
   public string FriendlyName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.SystemName")]
   public string SystemName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.IsActive")]
   public bool IsActive { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Configure")]
   public string ConfigurationUrl { get; set; }

   public string LogoUrl { get; set; }

   #endregion
}
