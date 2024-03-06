using Hub.Core;
using Hub.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Hub.Services.Installation;

/// <summary>
/// Represents middleware that checks whether database is installed and redirects to installation URL in otherwise
/// </summary>
public class InstallUrlMiddleware
{
   #region Fields

   private readonly RequestDelegate _next;

   #endregion

   #region Ctor

   /// <summary>
   /// Param Ctor
   /// </summary>
   /// <param name="next"></param>
   public InstallUrlMiddleware(RequestDelegate next)
   {
      _next = next;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Invoke middleware actions
   /// </summary>
   /// <param name="context">HTTP context</param>
   /// <param name="webHelper">Web helper</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task InvokeAsync(HttpContext context, IWebHelper webHelper)
   {
      //whether database is installed
      if (!DataSettingsManager.IsDatabaseInstalled())
      {
         var installUrl = $"{webHelper.GetAppLocation().Trim('/')}/{AppInstallationDefaults.InstallPath.Trim('/')}";

         if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
         {
            //redirect
            context.Response.Redirect(installUrl);
            return;
         }
      }

      //or call the next middleware in the request pipeline
      await _next(context);
   }

   #endregion
}