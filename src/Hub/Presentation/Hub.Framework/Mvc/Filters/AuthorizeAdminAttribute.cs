﻿using Hub.Data;
using Hub.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to the admin panel
/// </summary>
public sealed class AuthorizeAdminAttribute : TypeFilterAttribute
{
   #region Ctor

   /// <summary>
   /// Create instance of the filter attribute
   /// </summary>
   /// <param name="ignore">Whether to ignore the execution of filter actions</param>
   public AuthorizeAdminAttribute(bool ignore = false) : base(typeof(AuthorizeAdminFilter))
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
   /// Represents a filter that confirms access to the admin panel
   /// </summary>
   private class AuthorizeAdminFilter : IAsyncAuthorizationFilter
   {
      #region Fields

      private readonly bool _ignoreFilter;
      private readonly IPermissionService _permissionService;

      #endregion

      #region Ctor

      public AuthorizeAdminFilter(bool ignoreFilter, IPermissionService permissionService)
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
      private async Task AuthorizeAdminAsync(AuthorizationFilterContext context)
      {
         if (context == null)
            throw new ArgumentNullException(nameof(context));

         if (!DataSettingsManager.IsDatabaseInstalled())
            return;

         //check whether this filter has been overridden for the action
         var actionFilter = context.ActionDescriptor.FilterDescriptors
             .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
             .Select(filterDescriptor => filterDescriptor.Filter)
             .OfType<AuthorizeAdminAttribute>()
             .FirstOrDefault();

         //ignore filter (the action is available even if a user hasn't access to the admin area)
         if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
            return;

         //there is AdminAuthorizeFilter, so check access
         if (context.Filters.Any(filter => filter is AuthorizeAdminFilter))
            //authorize permission of access to the admin area
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
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
         await AuthorizeAdminAsync(context);
      }

      #endregion
   }

   #endregion
}