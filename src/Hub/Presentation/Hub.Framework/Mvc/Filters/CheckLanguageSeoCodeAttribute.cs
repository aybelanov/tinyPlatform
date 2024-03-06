﻿using Hub.Core;
using Hub.Core.Domain.Localization;
using Hub.Data;
using Hub.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters
{
   /// <summary>
   /// Represents filter attribute that checks SEO friendly URLs for multiple languages and properly redirect if necessary
   /// </summary>
   public sealed class CheckLanguageSeoCodeAttribute : TypeFilterAttribute
   {
      #region Ctor

      /// <summary>
      /// Create instance of the filter attribute
      /// </summary>
      /// <param name="ignore">Whether to ignore the execution of filter actions</param>
      public CheckLanguageSeoCodeAttribute(bool ignore = false) : base(typeof(CheckLanguageSeoCodeFilter))
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
      /// Represents a filter that checks SEO friendly URLs for multiple languages and properly redirect if necessary
      /// </summary>
      private class CheckLanguageSeoCodeFilter : IAsyncActionFilter
      {
         #region Fields

         private readonly bool _ignoreFilter;
         private readonly IWebHelper _webHelper;
         private readonly IWorkContext _workContext;
         private readonly LocalizationSettings _localizationSettings;

         #endregion

         #region Ctor

         public CheckLanguageSeoCodeFilter(bool ignoreFilter,
             IWebHelper webHelper,
             IWorkContext workContext,
             LocalizationSettings localizationSettings)
         {
            _ignoreFilter = ignoreFilter;
            _webHelper = webHelper;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
         }

         #endregion

         #region Utilities

         /// <summary>
         /// Called asynchronously before the action, after model binding is complete.
         /// </summary>
         /// <param name="context">A context for action filters</param>
         /// <returns>A task that represents the asynchronous operation</returns>
         private async Task CheckLanguageSeoCodeAsync(ActionExecutingContext context)
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

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckLanguageSeoCodeAttribute>()
                .FirstOrDefault();

            //ignore filter (an action doesn't need to be checked)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
               return;

            //whether SEO friendly URLs are enabled
            if (!_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
               return;

            //check whether current page URL is already localized URL
            var pageUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
            var result = await pageUrl.IsLocalizedUrlAsync(context.HttpContext.Request.PathBase, true);
            if (result.IsLocalized)
               return;

            //not localized yet, so redirect to the page with working language SEO code
            var language = await _workContext.GetWorkingLanguageAsync();
            pageUrl = pageUrl.AddLanguageSeoCodeToUrl(context.HttpContext.Request.PathBase, true, language);

            context.Result = new LocalRedirectResult(pageUrl, false);
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
            await CheckLanguageSeoCodeAsync(context);
            if (context.Result == null)
               await next();
         }

         #endregion
      }

      #endregion
   }
}