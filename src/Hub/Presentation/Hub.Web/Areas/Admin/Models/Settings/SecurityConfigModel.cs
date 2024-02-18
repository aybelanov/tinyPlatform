using System.Linq;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using LinqToDB.Common;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a security configuration model
/// </summary>
public partial record SecurityConfigModel : BaseAppModel, IConfigModel
{
   /// <summary>
   /// Whether CORS is enabled
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.CorsEnabled")]
   public bool CorsEnabled { get; set; } = true;

   /// <summary>
   /// CORS origins of the blazor client app
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.CorsOrigins")]
   public string CorsOriginString
   {
      get => string.Join(';', CorsOrigins);
      set => CorsOrigins = string.IsNullOrWhiteSpace(value) ? Array<string>.Empty : value.Split(';').Select(x=>x.Trim().Trim('/')).ToArray();
   }

   /// <summary>
   /// Array of cors origins
   /// </summary>
   public string[] CorsOrigins { get; set; }

   /// <summary>
   /// If the security connection is on. For this solution it must set to on.
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.SslEnabled")]
   public bool SslEnabled { get; set; }

   /// <summary>
   /// Reqire siganlr connection for client requests 
   /// and to control client per user count
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.AppSettings.Hosting.RequireSignalrConnection")]
   public bool RequireSignalrConnection { get; set; }
}
