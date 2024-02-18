using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// Represents signalr connection recoonent delay 
/// </summary>
public class ConnectionRetryPolicy : IRetryPolicy
{
   /// <summary>
   /// Next retry delay
   /// </summary>
   /// <param name="retryContext">Retry context</param>
   /// <returns>Delay as timespan</returns>
   public TimeSpan? NextRetryDelay(RetryContext retryContext)
   {
      return TimeSpan.FromSeconds(10);
   }
}
