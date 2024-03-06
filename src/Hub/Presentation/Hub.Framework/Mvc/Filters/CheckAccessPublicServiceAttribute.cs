using Hub.Data;
using Hub.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters
{
   /// <summary>
   /// Represents a filter attribute that confirms access to public service part
   /// </summary>
   public sealed class CheckAccessPublicPlatformAttribute : TypeFilterAttribute
   {
      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      /// <param name="ignore">Whether to ignore the execution of filter actions</param>
      public CheckAccessPublicPlatformAttribute(bool ignore = false) : base(typeof(CheckAccessPublicPlatformFilter))
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

      /// <summary>
      /// Represents a filter that confirms access to public platform
      /// </summary>
      private class CheckAccessPublicPlatformFilter : IAsyncAuthorizationFilter
      {
         #region Fields

         private readonly bool _ignoreFilter;
         private readonly IPermissionService _permissionService;

         #endregion

         #region Ctor

         public CheckAccessPublicPlatformFilter(bool ignoreFilter, IPermissionService permissionService)
         {
            _ignoreFilter = ignoreFilter;
            _permissionService = permissionService;
         }

         #endregion

         #region Utilities

         /// <summary>
         /// Called early in the filter pipeline to confirm request is authorized
         /// </summary>
         /// <param name="context">Authorization filter context</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         private async Task CheckAccessPublicPlatformAsync(AuthorizationFilterContext context)
         {
            if (context == null)
               throw new ArgumentNullException(nameof(context));

            if (!DataSettingsManager.IsDatabaseInstalled())
               return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckAccessPublicPlatformAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if navigation is not allowed)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
               return;

            //check whether current user has access to a public platform
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.PublicPagesAllowNavigation))
               return;

            //user hasn't access to a public platform
            context.Result = new ChallengeResult();
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
            await CheckAccessPublicPlatformAsync(context);
         }

         #endregion
      }

      #endregion
   }
}