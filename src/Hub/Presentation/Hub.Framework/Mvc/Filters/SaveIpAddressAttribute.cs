using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters
{
   /// <summary>
   /// Represents filter attribute that saves last IP address of user
   /// </summary>
   public sealed class SaveIpAddressAttribute : TypeFilterAttribute
   {
      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      public SaveIpAddressAttribute() : base(typeof(SaveIpAddressFilter))
      {
      }

      #endregion

      #region Nested filter

      /// <summary>
      /// Represents a filter that saves last IP address of user
      /// </summary>
      private class SaveIpAddressFilter : IAsyncActionFilter
      {
         #region Fields

         private readonly UserSettings _userSettings;
         private readonly IRepository<User> _userRepository;
         private readonly IWebHelper _webHelper;
         private readonly IWorkContext _workContext;

         #endregion

         #region Ctor

         public SaveIpAddressFilter(UserSettings userSettings,
             IRepository<User> userRepository,
             IWebHelper webHelper,
             IWorkContext workContext)
         {
            _userSettings = userSettings;
            _userRepository = userRepository;
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
         private async Task SaveIpAddressAsync(ActionExecutingContext context)
         {
            if (context == null)
               throw new ArgumentNullException(nameof(context));

            if (context.HttpContext.Request == null)
               return;

            //only in GET requests
            if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
               return;

            if (!DataSettingsManager.IsDatabaseInstalled())
               return;

            //check whether we store IP addresses
            if (!_userSettings.StoreIpAddresses)
               return;

            //get current IP address
            var currentIpAddress = _webHelper.GetCurrentIpAddress();

            if (string.IsNullOrEmpty(currentIpAddress))
               return;

            //update user's IP address
            var user = await _workContext.GetCurrentUserAsync();
            if (_workContext.OriginalUserIfImpersonated == null &&
                 !currentIpAddress.Equals(user.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
            {
               user.LastIpAddress = currentIpAddress;

               //update user without event notification
               await _userRepository.UpdateAsync(user, false);
            }
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
            await SaveIpAddressAsync(context);
            if (context.Result == null)
               await next();
         }

         #endregion
      }

      #endregion
   }
}