using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Devices.Dispatcher.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// Acces is allowed only from local IP's and
/// if webapi is enabled in device settings
/// </summary>
public class OnlyLocalIPAttribute : TypeFilterAttribute
{
   #region Ctor

   /// <summary>
   /// Create instance of the filter attribute
   /// </summary>
   public OnlyLocalIPAttribute(bool ignore = false) : base(typeof(OnlyLocalIPFilter))
   {
      IgnoreFilter = ignore;
      Arguments = new object[] { ignore };
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets a value indicating whether to ignore the execution of filter actions
   /// </summary>
   public bool IgnoreFilter { get; }

   #endregion

   #region Nested filter

   private class OnlyLocalIPFilter : IAsyncResourceFilter
   {
      #region Fields

      private readonly bool _ignoreFilter;
      private readonly HubConnections _hubConnection;
      #endregion

      #region Ctor

      public OnlyLocalIPFilter(bool ignoreFilter, HubConnections hubConnection)
      {

         _ignoreFilter = ignoreFilter;
         _hubConnection = hubConnection;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Called early in the filter pipeline to confirm request is authorized
      /// </summary>
      /// <param name="context">Authorization filter context</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      private void ValidateIPAddress(ResourceExecutingContext context)
      {
         if (context == null)
            throw new ArgumentNullException(nameof(context));

         //check whether this filter has been overridden for the action
         var actionFilter = context.ActionDescriptor.FilterDescriptors
             .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
             .Select(filterDescriptor => filterDescriptor.Filter)
             .OfType<OnlyLocalIPAttribute>()
             .FirstOrDefault();

         //ignore filter (the action is available even if a user hasn't access to the admin area)
         if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
            return;

         if (context.HttpContext.Request == null)
            return;

         if (context.HttpContext.Connection?.RemoteIpAddress is not IPAddress remoteIp)
            return;

         if (remoteIp.IsIP4InternalIP() && _hubConnection.Enabled)
            return;

         // Forbidden
         context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
         context.Result = new EmptyResult();
      }

      #endregion

      #region Methods

      /// <summary>
      /// Called asynchronously before the action, after model binding is complete.
      /// </summary>
      /// <param name="context">A context for action filters</param>
      /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
      {
         ValidateIPAddress(context);
         if (context.Result == null)
            await next();
      }
      #endregion
   }

   #endregion
}
