using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Common;
using Hub.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters
{
   /// <summary>
   /// Represents filter attribute that validates force of the multi-factor authentication
   /// </summary>
   public sealed class ForceMultiFactorAuthenticationAttribute : TypeFilterAttribute
   {

      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      public ForceMultiFactorAuthenticationAttribute() : base(typeof(ForceMultiFactorAuthenticationFilter))
      {
      }

      #endregion

      #region Nested filter

      private class ForceMultiFactorAuthenticationFilter : IAsyncActionFilter
      {
         #region Fields

         private readonly IUserService _userService;
         private readonly IGenericAttributeService _genericAttributeService;
         private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
         private readonly IWorkContext _workContext;
         private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

         #endregion

         #region Ctor

         public ForceMultiFactorAuthenticationFilter(IUserService userService,
             IGenericAttributeService genericAttributeService,
             IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
             IWorkContext workContext,
             MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
         {
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _workContext = workContext;
            _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
         }

         #endregion

         #region Utilities

         /// <summary>
         /// Called asynchronously before the action, after model binding is complete.
         /// </summary>
         /// <param name="context">A context for action filters</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         private async Task ValidateAuthenticationForceAsync(ActionExecutingContext context)
         {
            if (context == null)
               throw new ArgumentNullException(nameof(context));

            if (context.HttpContext.Request == null)
               return;

            if (!DataSettingsManager.IsDatabaseInstalled())
               return;

            //validate only for registered users
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
               return;

            //don't validate on the 'Multi-factor authentication settings' page
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
               return;

            if (controllerName.Equals("User", StringComparison.InvariantCultureIgnoreCase) &&
                actionName.Equals("MultiFactorAuthentication", StringComparison.InvariantCultureIgnoreCase))
               return;

            //whether the feature is enabled
            if (!_multiFactorAuthenticationSettings.ForceMultifactorAuthentication ||
                !await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
               return;

            //check selected provider of MFA
            var selectedProvider = await _genericAttributeService
                .GetAttributeAsync<string>(user, AppUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            if (!string.IsNullOrEmpty(selectedProvider))
               return;

            //redirect to MultiFactorAuthenticationSettings page if force is enabled
            context.Result = new RedirectToRouteResult("MultiFactorAuthenticationSettings", null);
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
            await ValidateAuthenticationForceAsync(context);
            if (context.Result == null)
               await next();
         }

         #endregion
      }

      #endregion
   }
}
