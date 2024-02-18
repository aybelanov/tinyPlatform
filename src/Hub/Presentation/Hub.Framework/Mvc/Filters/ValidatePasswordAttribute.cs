using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Hub.Core;
using Hub.Data;
using Hub.Services.Users;

namespace Hub.Web.Framework.Mvc.Filters
{
   /// <summary>
   /// Represents filter attribute that validates user password expiration
   /// </summary>
   public sealed class ValidatePasswordAttribute : TypeFilterAttribute
   {
      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      public ValidatePasswordAttribute() : base(typeof(ValidatePasswordFilter))
      {
      }

      #endregion

      #region Nested filter

      /// <summary>
      /// Represents a filter that validates user password expiration
      /// </summary>
      private class ValidatePasswordFilter : IAsyncActionFilter
      {
         #region Fields

         private readonly IUserService _userService;
         private readonly IWebHelper _webHelper;
         private readonly IWorkContext _workContext;

         #endregion

         #region Ctor

         public ValidatePasswordFilter(IUserService userService,
             IWebHelper webHelper,
             IWorkContext workContext)
         {
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
         }

         #endregion

         #region Utilities

         /// <summary>
         /// Called asynchronously before the action, after model binding is complete.
         /// </summary>
         /// <param name="context">A context for action filters</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         private async Task ValidatePasswordAsync(ActionExecutingContext context)
         {
            if (context == null)
               throw new ArgumentNullException(nameof(context));

            if (context.HttpContext.Request == null)
               return;

            //ignore AJAX requests
            if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
               return;

            if (!DataSettingsManager.IsDatabaseInstalled())
               return;

            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
               return;

            //don't validate on the 'Change Password' page
            if (controllerName.Equals("User", StringComparison.InvariantCultureIgnoreCase) &&
                actionName.Equals("ChangePassword", StringComparison.InvariantCultureIgnoreCase))
               return;

            //check password expiration
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsPasswordExpiredAsync(user))
               return;

            var returnUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
            //redirect to ChangePassword page if expires
            context.Result = new RedirectToRouteResult("UserChangePassword", new { returnUrl });
         }

         #endregion

         #region Methods

         /// <summary>
         /// Called asynchronously before the action, after model binding is complete.
         /// </summary>
         /// <param name="context">A context for action filters</param>
         /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
         {
            await ValidatePasswordAsync(context);
            if (context.Result == null)
               await next();
         }

         #endregion
      }

      #endregion
   }
}