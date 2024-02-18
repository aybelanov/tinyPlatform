using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Common;

namespace Hub.Web.Framework.Mvc.Filters
{
   /// <summary>
   /// Represents filter attribute that saves last visited page by user
   /// </summary>
   public sealed class SaveLastVisitedPageAttribute : TypeFilterAttribute
   {

      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      /// <param name="ignore">Whether to ignore the execution of filter actions</param>
      public SaveLastVisitedPageAttribute(bool ignore = false) : base(typeof(SaveLastVisitedPageFilter))
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
      /// Represents a filter that saves last visited page by user
      /// </summary>
      private class SaveLastVisitedPageFilter : IAsyncActionFilter
      {
         #region Fields

         private readonly UserSettings _userSettings;
         private readonly IGenericAttributeService _genericAttributeService;
         private readonly IRepository<GenericAttribute> _genericAttributeRepository;
         private readonly IWebHelper _webHelper;
         private readonly IWorkContext _workContext;
         private readonly bool _ignoreFilter;

         #endregion

         #region Ctor

         public SaveLastVisitedPageFilter(UserSettings userSettings,
             IGenericAttributeService genericAttributeService,
             IRepository<GenericAttribute> genericAttributeRepository,
             IWebHelper webHelper,
             IWorkContext workContext,
             bool ignoreFilter)
         {
            _userSettings = userSettings;
            _genericAttributeService = genericAttributeService;
            _genericAttributeRepository = genericAttributeRepository;
            _webHelper = webHelper;
            _workContext = workContext;
            _ignoreFilter = ignoreFilter;
         }

         #endregion

         #region Utilities

         /// <summary>
         /// Called asynchronously before the action, after model binding is complete.
         /// </summary>
         /// <param name="context">A context for action filters</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         private async Task SaveLastVisitedPageAsync(ActionExecutingContext context)
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

            //check whether this filter has been overridden for the action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<SaveLastVisitedPageAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if a user hasn't access to the admin area)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
               return;

            //check whether we store last visited page URL
            if (!_userSettings.PlatformLastVisitedPage)
               return;

            //get current page
            var pageUrl = _webHelper.GetThisPageUrl(true);

            if (string.IsNullOrEmpty(pageUrl))
               return;

            //get previous last page
            var user = await _workContext.GetCurrentUserAsync();

            //var attr = _genericAttributeRepository.Table.FirstOrDefault(x=>x.KeyGroup == nameof(User) && x.EntityId == user.Id && x.Key == AppUserDefaults.LastVisitedPageAttribute);
            //attr.Value = pageUrl;   
            //await _genericAttributeRepository.UpdateAsync(attr);
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.LastVisitedPageAttribute, pageUrl);
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
            await SaveLastVisitedPageAsync(context);
            if (context.Result == null)
               await next();
         }

         #endregion
      }

      #endregion
   }
}