using Hub.Core;
using Hub.Core.Configuration;
using Hub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that checks whether current connection is secured and properly redirect if necessary
/// </summary>
public sealed class HttpsRequirementAttribute : TypeFilterAttribute
{
   #region Ctor

   /// <summary>
   /// Create instance of the filter attribute
   /// </summary>
   public HttpsRequirementAttribute() : base(typeof(HttpsRequirementFilter))
   {
   }

   #endregion

   #region Nested filter

   /// <summary>
   /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
   /// </summary>
   private class HttpsRequirementFilter(IWebHelper webHelper, AppSettings appSettings) : IAsyncAuthorizationFilter
   {
      #region Fields
    
      private readonly SecurityConfig _securityConfig = appSettings.Get<SecurityConfig>();
      private readonly HostingConfig _hostingConfig = appSettings.Get<HostingConfig>();
      
      #endregion

      #region Utilities

      /// <summary>
      /// Called early in the filter pipeline to confirm request is authorized
      /// </summary>
      /// <param name="context">Authorization filter context</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      private async Task CheckHttpsRequirementAsync(AuthorizationFilterContext context)
      {
         ArgumentNullException.ThrowIfNull(context);

         // Proxy server must implement the redirect logic,
         // so we switch off our https redirect logic
         if(_hostingConfig.UseProxy)
            return;

         if (context.HttpContext.Request == null)
            return;

         //only in GET requests, otherwise the browser might not propagate the verb and request body correctly
         if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
            return;

         if (!DataSettingsManager.IsDatabaseInstalled())
            return;

         //whether current connection is secured
         var currentConnectionSecured = webHelper.IsCurrentConnectionSecured();

         //page should be secured, so redirect (permanent) to HTTPS version of page
         if (_securityConfig.SslEnabled && !currentConnectionSecured)
            context.Result = new RedirectResult(webHelper.GetThisPageUrl(true, true), true);

         //page shouldn't be secured, so redirect (permanent) to HTTP version of page
         if (!_securityConfig.SslEnabled && currentConnectionSecured)
            context.Result = new RedirectResult(webHelper.GetThisPageUrl(true, false), true);

         await Task.CompletedTask;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Called early in the filter pipeline to confirm request is authorized
      /// </summary>
      /// <param name="context">Authorization filter context</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
      {
         await CheckHttpsRequirementAsync(context);
      }

      #endregion
   }

   #endregion
}