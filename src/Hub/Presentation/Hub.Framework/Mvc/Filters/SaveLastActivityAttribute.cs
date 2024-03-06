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
   /// Represents filter attribute that saves last user activity date
   /// </summary>
   public sealed class SaveLastActivityAttribute : TypeFilterAttribute
   {
      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      public SaveLastActivityAttribute() : base(typeof(SaveLastActivityFilter))
      {
      }

      #endregion

      #region Nested filter

      /// <summary>
      /// Represents a filter that saves last user activity date
      /// </summary>
      private class SaveLastActivityFilter : IAsyncActionFilter
      {
         #region Fields

         private readonly UserSettings _userSettings;
         private readonly IRepository<User> _userRepository;
         private readonly IWorkContext _workContext;

         #endregion

         #region Ctor

         public SaveLastActivityFilter(UserSettings userSettings,
             IRepository<User> userRepository,
             IWorkContext workContext)
         {
            _userSettings = userSettings;
            _userRepository = userRepository;
            _workContext = workContext;
         }

         #endregion

         #region Utilities

         /// <summary>
         /// Called asynchronously before the action, after model binding is complete.
         /// </summary>
         /// <param name="context">A context for action filters</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         private async Task SaveLastActivityAsync(ActionExecutingContext context)
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

            //update last activity date
            var user = await _workContext.GetCurrentUserAsync();
            if (user.LastActivityUtc.AddMinutes(_userSettings.LastActivityMinutes) < DateTime.UtcNow)
            {
               user.LastActivityUtc = DateTime.UtcNow;

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
            await SaveLastActivityAsync(context);
            if (context.Result == null)
               await next();
         }

         #endregion
      }

      #endregion
   }
}