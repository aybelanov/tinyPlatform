namespace Hub.Core.Configuration;

/// <summary>
/// Represents hosting configuration parameters
/// </summary>
public partial class HostingConfig : IConfig
{
   /// <summary>
   /// Hub application host url
   /// </summary>
   public string HubHostUrl { get; private set; } = "https://localhost:5001";

   /// <summary>
   /// Client application ("Telemetry dashboard") host url
   /// </summary>
   public string ClientHostUrl { get; private set; } = "https://localhost:5003/dashboard";

   /// <summary>
   /// Gets or sets a value indicating whether to use proxy servers and load balancers
   /// </summary>
   public bool UseProxy { get; private set; }

   /// <summary>
   /// Gets or sets the header used to retrieve the value for the originating scheme (HTTP/HTTPS)
   /// </summary>
   /// <remarks>For example Nginx "X-Forwarded-Proto"</remarks>
   public string ForwardedProtoHeaderName { get; private set; } = "X-Forwarded-Proto";

   /// <summary>
   /// Gets or sets the header used to retrieve the originating client IP
   /// </summary>
   /// <remarks>e.g. Nginx "X-Forwarded-For"</remarks>
   public string ForwardedForHeaderName { get; private set; } = "X-Forwarded-For";

   /// <summary>
   /// Gets or sets addresses of known proxies to accept forwarded headers from
   /// </summary>
   /// <remarks>e.g. IP4 like 1.1.1.1 (private or public)</remarks>
   public string KnownProxies { get; private set; } = string.Empty;
}