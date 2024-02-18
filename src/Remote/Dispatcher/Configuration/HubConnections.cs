namespace Devices.Dispatcher.Configuration;

/// <summary>
/// Hub connection configuration
/// </summary>
public class HubConnections
{
   /// <summary>
   /// Client identifier
   /// </summary>
   public string ClientId { get; set; }

   /// <summary>
   /// Gets or sets the unique device name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the device secret credential
   /// </summary>
   public string SecretCredential { get; set; }

   /// <summary>
   /// Is communication (only data) enabled
   /// A token will be updating
   /// </summary>
   public bool Enabled { get; set; }

   /// <summary>
   /// Server webapi endpoint
   /// </summary>
   public string HubEndpoint { get; set; }

   /// <summary>
   /// Grpc service endpoint
   /// </summary>
   public string HubGrpcEndpoint { get; set; }
}
