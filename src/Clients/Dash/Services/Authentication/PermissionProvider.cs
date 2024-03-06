using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Clients.Dash.Services.Authentication;

/// <summary>
/// Represents standard permission provider
/// </summary>
public class PermissionProvider
{
   private static readonly IEnumerable<string> _permissions = new[]
   {
      "AccessToClient",
      "ManageDevices",
      "ManageMonitors",
      "ManageHMIWidgets",
      "ManageReports",
      "ShareDevices",
      "ShareMonitors",
      "ShareWidgets",
      "ShareReports",
      "ViewData",
      "ViewReports"
   };


   /// <summary>
   /// Prepare authorization policies by permissions
   /// </summary>
   /// <param name="options">Authorization options</param>
   public static void PreparePolicies(AuthorizationOptions options)
   {
      foreach (var permission in _permissions)
      {
         options.AddPolicy(permission, policy =>
         {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", permission);
         });
      }
   }
}
