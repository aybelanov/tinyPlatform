using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Hub.Core;
using Hub.Data;

namespace Hub.Services.Common;

/// <summary>
/// Represents middleware that checks whether request is for keep alive
/// </summary>
public class KeepAliveMiddleware
{
   #region Fields

   private readonly RequestDelegate _next;

   #endregion

   #region Ctor
   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="next"></param>
   public KeepAliveMiddleware(RequestDelegate next)
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
      if (DataSettingsManager.IsDatabaseInstalled())
      {
         //keep alive page requested (we ignore it to prevent creating a guest user records)
         var keepAliveUrl = $"{webHelper.GetAppLocation()}{HubCommonDefaults.KeepAlivePath}";
         if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
            return;
      }

      //or call the next middleware in the request pipeline
      await _next(context);
   }

   #endregion
}