using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a hosting configuration model
/// </summary>
public partial record HostingConfigModel : BaseAppModel, IConfigModel
{
   #region Properties
   
   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.HubHostUrl")]
   public string HubHostUrl { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ClientHostUrl")]
   public string ClientHostUrl { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ListeningUrls")]
   public string Urls { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.AllowedHosts")]
   public string AllowedHosts { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseProxy")]
   public bool UseProxy { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName")]
   public string ForwardedForHeaderName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName")]
   public string ForwardedProtoHeaderName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.KnownProxies")]
   public string KnownProxies { get; set; }

   #endregion
}