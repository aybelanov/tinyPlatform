using Clients.Dash.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Shared.Clients;
using Clients.Dash.Configuration;

namespace Clients.Dash.Services.Authentication;

/// <summary>
/// Represent a custom implementaion of the authorization message handler
/// </summary>
public class DashAuthorizationMessageHandler : AuthorizationMessageHandler
{
   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="provider"></param>
   /// <param name="navigationManager"></param>
   public DashAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager) : base(provider, navigationManager)
   {
      ConfigureHandler(authorizedUrls: new[]
      {
         Singleton<ServerConfig>.Instance.WebapiEndpoint,
         Singleton<ServerConfig>.Instance.GrpcHost
      });
   }
}
