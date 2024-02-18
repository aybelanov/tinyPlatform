using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Shared.Common;

namespace Hub.Services.Authentication;

/// <summary>
/// Represents default values related to authentication services
/// </summary>
public static partial class AuthDefaults
{
   /// <summary>
   /// Mixed (cookies and bearer) value for authentication scheme 
   /// </summary>
   public const string MixedScheme = "cookies_and_jwt";

   /// <summary>
   /// The default value used for authentication scheme
   /// </summary>
   public const string CookieAuthenticationScheme = "Authentication";

   /// <summary>
   /// The default value used for external authentication scheme
   /// </summary>
   public const string ExternalAuthenticationScheme = "ExternalAuthentication";

   /// <summary>
   /// The issuer that should be used for any claims that are created
   /// </summary>
   public static string ClaimsIssuer => "hubapplication";

   /// <summary>
   /// The default value for the login path
   /// </summary>
   public static PathString LoginPath => new("/login");

   /// <summary>
   /// The default value for the access denied path
   /// </summary>
   public static PathString AccessDeniedPath => new("/page-not-found");

   /// <summary>
   /// The default value of the return URL parameter
   /// </summary>
   public static string ReturnUrlParameter => "returnUrl";

   /// <summary>
   /// Gets a key to store external authentication errors to session
   /// </summary>
   public static string ExternalAuthenticationErrorsSessionKey => "app.externalauth.errors";

   /// <summary>
   /// Atentication scheme for custom OAuth implementation
   /// </summary>
   public static string OAuth2Scheme => "OAuthAthentication";

   /// <summary>
   /// Auth code life time in seconds
   /// </summary>
   public static int OAuthCodeLifetime => 300;

   /// <summary>
   /// Json web token life time in seconds for clients
   /// </summary>
   public static int ClientJWTlifetime => 3600;

   /// <summary>
   /// Json web token life time in seconds for devices
   /// </summary>
   public static int DeviceJWTlifetime => 3600;

   /// <summary>
   /// Client application
   /// </summary>
   public static string ClientApp => "Clients.Dash";

   /// <summary>
   /// Device service
   /// </summary>
   public static string DispatcherApp => "Remote.Dispatcher";

   /// <summary>
   /// Client policy name for authorization
   /// </summary>
   public const string AuthClientPolicy = "Clients";

   /// <summary>
   /// Client policy name for authorization
   /// </summary>
   public const string AuthDevicePolicy = "Devices";

   /// <summary>
   /// Clients roles for authorization
   /// </summary>
   public const string AuthDeviceRoles = "Devices";

   /// <summary>
   /// JWT audiences
   /// </summary>
   public static IEnumerable<string> Audiences => new[]
   {
      ClientApp,
      DispatcherApp
   };
}