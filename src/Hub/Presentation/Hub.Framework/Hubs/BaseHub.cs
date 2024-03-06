using Hub.Services.Authentication;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Shared.Clients.Configuration;

namespace Hub.Web.Framework.Hubs;

/// <summary>
/// Represents a base signalr hub 
/// </summary>
[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(Roles = UserDefaults.TelemetryRoles, Policy = AuthDefaults.AuthClientPolicy)]
public abstract class BaseHub : Microsoft.AspNetCore.SignalR.Hub
{

}