using System;

namespace Hub.Core.Configuration;

/// <summary>
/// Represents security configuration parameters
/// </summary>
public class SecurityConfig : IConfig
{
   /// <summary>
   /// Whether CORS is enabled
   /// </summary>
   public bool CorsEnabled { get; private set; } = false;

   /// <summary>
   /// CORS origins of the blazor client app
   /// </summary>
   public string[] CorsOrigins { get; private set; } = Array.Empty<string>();

   /// <summary>
   /// If the security connection is on. For this solution it must set to on.
   /// </summary>
   public bool SslEnabled { get; private set; } = true;

   /// <summary>
   /// Reqire siganlr connection for client requests 
   /// and to control client per user count
   /// </summary>
   public bool RequireSignalrConnection { get; private set; } = true;
}
