using Clients.Dash.Services.Connection;
using Shared.Clients.SignalR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// Represent application common http message hadler
/// </summary>
public class CommonHttpHandler : DelegatingHandler, IDisposable
{
   #region fields

   private readonly HubService _hubService;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public CommonHttpHandler(HubService hubService)
   {
      _hubService = hubService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Data load error handler
   /// </summary>
   /// <param name="request"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   {
      // adds signalr connection id for grpc and webapi idedntification on the hub server
      var connectioId = await _hubService.GetConnectionIdAsync();
      request.Headers.Add(SignalRDefaults.SignalrConnectionIdHeader, connectioId);

      var response = await base.SendAsync(request, cancellationToken);
      return response;
   }

   #endregion
}
