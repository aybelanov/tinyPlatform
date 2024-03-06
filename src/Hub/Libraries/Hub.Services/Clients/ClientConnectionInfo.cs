using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Net;

namespace Hub.Services.Clients;

/// <summary>
/// Represents a SignalR hub connection info
/// </summary>
public class ClientConnectionInfo
{
   #region Ctors

   /// <summary>
   /// Params ctor
   /// </summary>
   /// <param name="hubLifetimeContext"></param>
   /// <param name="userId">User identifier</param>
   public ClientConnectionInfo(long userId, HubLifetimeContext hubLifetimeContext)
   {
      UserId = userId;
      ConnectionId = hubLifetimeContext.Context.ConnectionId;
      IpAddressV4 = hubLifetimeContext.Context.GetHttpContext().Connection?.RemoteIpAddress.MapToIPv4();
      SubcribedGroups = new HashSet<string>();
   }

   #endregion

   #region Properties

   /// <summary>
   /// User identifier
   /// </summary>
   public long UserId { get; }

   ///// <summary>
   ///// Connection http context
   ///// </summary>
   //public HttpContext HttpContext { get; }

   /// <summary>
   /// Subscrided groups
   /// </summary>
   public HashSet<string> SubcribedGroups { get; }

   /// <summary>
   /// Connection idenifier
   /// </summary>
   public string ConnectionId { get; }

   /// <summary>
   /// Connection IP addres V4
   /// </summary>
   public IPAddress IpAddressV4 { get; }

   #endregion
}
