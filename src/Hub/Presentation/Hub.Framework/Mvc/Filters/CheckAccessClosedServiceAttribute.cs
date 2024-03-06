using Hub.Core.Domain;
using Hub.Data;
using Hub.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to a closed service
/// </summary>
public sealed class CheckAccessClosedPlatformAttribute : TypeFilterAttribute
{
   #region Ctor

   /// <summary>
   /// Create instance of the filter attribute
   /// </summary>
   /// <param name="ignore">Whether to ignore the execution of filter actions</param>
   public CheckAccessClosedPlatformAttribute(bool ignore = false) : base(typeof(CheckAccessClosedPlatformFilter))
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
   /// Represents a filter that confirms access to closed platform
   /// </summary>
   private class CheckAccessClosedPlatformFilter : IAsyncActionFilter
   {
      #region Fields

      private readonly bool _ignoreFilter;
      private readonly IPermissionService _permissionService;
      private readonly AppInfoSettings _appInformationSettings;

      #endregion

      #region Ctor

      public CheckAccessClosedPlatformFilter(bool ignoreFilter,
          IPermissionService permissionService,
          AppInfoSettings appInformationSettings)
      {
         _ignoreFilter = ignoreFilter;
         _permissionService = permissionService;
         _appInformationSettings = appInformationSettings;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Called asynchronously before the action, after model binding is complete.
      /// </summary>
      /// <param name="context">A context for action filters</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      private async Task CheckAccessClosedPlatformAsync(ActionExecutingContext context)
      {
         if (context == null)
            throw new ArgumentNullException(nameof(context));

         if (!DataSettingsManager.IsDatabaseInstalled())
            return;

         //check whether this filter has been overridden for the Action
         var actionFilter = context.ActionDescriptor.FilterDescriptors
             .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
             .Select(filterDescriptor => filterDescriptor.Filter)
             .OfType<CheckAccessClosedPlatformAttribute>()
             .FirstOrDefault();

         //ignore filter (the action is available even if a platform is closed)
         if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
            return;

         // the microservice isn't closed
         if (!_appInformationSettings.PlatformClosed)
            return;

         //get action and controller names
         var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
         var actionName = actionDescriptor?.ActionName;
         var controllerName = actionDescriptor?.ControllerName;

         if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
            return;

         //two factor verification accessible when a platform is closed
         if (controllerName.Equals("User", StringComparison.InvariantCultureIgnoreCase) &&
             actionName.Equals("MultiFactorVerification", StringComparison.InvariantCultureIgnoreCase))
            return;

         // TODO implementation a page 'Platform is closed'
         ////topics accessible when a platform is closed
         //if (controllerName.Equals("ForumPost", StringComparison.InvariantCultureIgnoreCase) &&
         //    actionName.Equals("TopicDetails", StringComparison.InvariantCultureIgnoreCase))
         //{
         //   //get identifiers of topics are accessible when a platform is closed

         //   var platform = await _storeContext.GetCurrentPlatformAsync();
         //   var allowedTopicIds = (await _topicService.GetAllTopicsAsync(platform.Id))
         //       .Where(topic => topic.AccessibleWhenPlatformClosed)
         //       .Select(topic => topic.Id);

         //   //check whether requested topic is allowed
         //   var requestedTopicId = context.RouteData.Values["topicId"] as int?;
         //   if (requestedTopicId.HasValue && allowedTopicIds.Contains(requestedTopicId.Value))
         //      return;
         //}

         //check whether current user has access to a closed platform
         if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessClosedService))
            return;

         //platform is closed and no access, so redirect to 'PlatformClosed' page
         context.Result = new RedirectToRouteResult("PlatformClosed", null);
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
         await CheckAccessClosedPlatformAsync(context);
         if (context.Result == null)
            await next();
      }

      #endregion
   }

   #endregion
}