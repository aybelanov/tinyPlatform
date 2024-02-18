namespace Clients.Dash.Configuration;

/// <summary>
/// Represents an endpoint configuration 
/// </summary>
public class ServerConfig
{
   /// <summary>
   /// IoT hub host url
   /// </summary>
   public string HubHost { get; set; }

   /// <summary>
   /// Grpc host url
   /// </summary>
   public string GrpcHost { get; set; }

   /// <summary>
   /// SignalR endpoint url
   /// </summary>
   public string SignalrEndpoint { get; set; }

   /// <summary>
   /// Webapi endpoint url
   /// </summary>
   public string WebapiEndpoint { get; set; }   
}