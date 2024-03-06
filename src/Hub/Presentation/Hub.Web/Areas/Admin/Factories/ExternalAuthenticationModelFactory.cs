﻿using Hub.Services.Authentication.External;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.ExternalAuthentication;
using Hub.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the external authentication method model factory implementation
   /// </summary>
   public partial class ExternalAuthenticationMethodModelFactory : IExternalAuthenticationMethodModelFactory
   {
      #region Fields

      private readonly IAuthenticationPluginManager _authenticationPluginManager;

      #endregion

      #region Ctor

      public ExternalAuthenticationMethodModelFactory(IAuthenticationPluginManager authenticationPluginManager)
      {
         _authenticationPluginManager = authenticationPluginManager;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare external authentication method search model
      /// </summary>
      /// <param name="searchModel">External authentication method search model</param>
      /// <returns>External authentication method search model</returns>
      public virtual ExternalAuthenticationMethodSearchModel PrepareExternalAuthenticationMethodSearchModel(
          ExternalAuthenticationMethodSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare page parameters
         searchModel.SetGridPageSize();

         return searchModel;
      }

      /// <summary>
      /// Prepare paged external authentication method list model
      /// </summary>
      /// <param name="searchModel">External authentication method search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the external authentication method list model
      /// </returns>
      public virtual async Task<ExternalAuthenticationMethodListModel> PrepareExternalAuthenticationMethodListModelAsync(
          ExternalAuthenticationMethodSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get external authentication methods
         var externalAuthenticationMethods = (await _authenticationPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

         //prepare grid model
         var model = new ExternalAuthenticationMethodListModel().PrepareToGrid(searchModel, externalAuthenticationMethods, () =>
         {
            //fill in model values from the entity
            return externalAuthenticationMethods.Select(method =>
               {
                  //fill in model values from the entity
                  var externalAuthenticationMethodModel = method.ToPluginModel<ExternalAuthenticationMethodModel>();

                  //fill in additional values (not existing in the entity)
                  externalAuthenticationMethodModel.IsActive = _authenticationPluginManager.IsPluginActive(method);
                  externalAuthenticationMethodModel.ConfigurationUrl = method.GetConfigurationPageUrl();

                  return externalAuthenticationMethodModel;
               });
         });

         return model;
      }

      #endregion
   }
}