using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Services.Clients;
using Hub.Services.Logging;
using Hub.Services.Users;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Hubs.Filters;

/// <summary>
/// Represents a hub filter with default subscriptions
/// </summary>
public class SubscribeGroupsFilter(IUserService userService, ICommunicator communicator, IWorkContext workContext, ILogger logger) : IHubFilter
{
   private readonly IUserService _userService = userService;
   private readonly IWorkContext _workContext = workContext;
   private readonly ILogger _logger = logger;
   private readonly ICommunicator _communicator = communicator;

   // <summary>
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
      try
      {
         var user = await _workContext.GetCurrentUserAsync();
         await _communicator.AddUserToGroupsAsync(user.Id, [$"{nameof(User)}_{user.Id}"]);
         await next(context);
      }
      catch (Exception ex)
      {
         await _logger.ErrorAsync("Default subscriptions are failed", ex);
      }
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
      try
      {
         var user = await _workContext.GetCurrentUserAsync();
         await _communicator.RemoveUserFromGroupsAsync(user.Id, [$"{nameof(User)}_{user.Id}"]);
         await next(context, exception);
      }
      catch (Exception ex)
      {
         await _logger.ErrorAsync("Default unsubscriptions are failed", ex);
      }
   }
}
