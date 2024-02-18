using Hub.Core;
using Hub.Core.Configuration;
using Hub.Services.Clients;
using Hub.Services.Logging;
using Hub.Services.Users;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Hubs.Filters;

/// <summary>
/// Represents a signalr filter 
/// that register connection with the communicator
/// </summary>
public class RegisterConnectionFilter(ICommunicator communicator, IWorkContext workContext,  IUserService userService,
   IUserActivityService userActivityService, AppSettings appSetiing) : IHubFilter
{
   #region fields

   private readonly ICommunicator _communicator = communicator;
   private readonly IWorkContext _workContext = workContext;
   private readonly IUserActivityService _userActivityService = userActivityService;
   private readonly IUserService _userService = userService;
   private readonly CommonConfig _commonConfig = appSetiing.Get<CommonConfig>();

   #endregion

   #region Methods

   /// <summary>
   /// Allows handling of all Hub method invocations.
   /// </summary>
   /// <param name="invocationContext">The context for the method invocation that holds all the important information about the invoke.</param>
   /// <param name="next">The next filter to run, and for the final one, the Hub invocation.</param>
   /// <returns>Returns the result of the Hub method invoke.</returns>
   public ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next) => next(invocationContext);

   /// <summary>
   /// Allows handling of the <see cref="Hub.OnConnectedAsync"/> method.
   /// </summary>
   /// <param name="context">The context for OnConnectedAsync.</param>
   /// <param name="next">The next filter to run, and for the final one, the Hub invocation.</param>
   /// <returns></returns>
   public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var userConnections = await _communicator.GetUserConnectionsInfoAsync(user.Id);
      if (!await _userService.IsAdminAsync(user) && userConnections.Count + 1 > _commonConfig.ConnectionPerUser)
      {
         var ips = string.Join(", ", userConnections.Select(x=>x.IpAddressV4.ToString()));
         throw new AppException($"Connection error. User {user.Email} has already connected from IP(s): {ips}.");
         //context.Hub.Context.Abort();
         //return;
      }

      await _communicator.RegisterClientConnectionAsync(user.Id, context);
      await _userActivityService.InsertActivityAsync("DashBoard.Connected", "User has connected.");

      await next(context);
   }

   /// <summary>
   /// Allows handling of the <see cref="Hub.OnDisconnectedAsync(Exception)"/> method.
   /// </summary>
   /// <param name="context">The context for OnDisconnectedAsync.</param>
   /// <param name="exception">The exception, if any, for the connection closing.</param>
   /// <param name="next">The next filter to run, and for the final one, the Hub invocation.</param>
   /// <returns></returns>
   public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception, Func<HubLifetimeContext, Exception, Task> next)
   {
      if (exception is AppException ex && ex.Message.StartsWith("Connection error"))
         return;

      await _communicator.UnregisterUserConnectionAsync(context.Context.ConnectionId);
      await _userActivityService.InsertActivityAsync("DashBoard.Disconnected", "User has disconnected.");
      await next(context, exception);
   }

   #endregion
}
