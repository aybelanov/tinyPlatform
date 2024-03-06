﻿using Hub.Core.Domain.Messages;
using Hub.Services.Messages;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the email account model factory implementation
   /// </summary>
   public partial class EmailAccountModelFactory : IEmailAccountModelFactory
   {
      #region Fields

      private readonly EmailAccountSettings _emailAccountSettings;
      private readonly IEmailAccountService _emailAccountService;

      #endregion

      #region Ctor

      public EmailAccountModelFactory(EmailAccountSettings emailAccountSettings,
          IEmailAccountService emailAccountService)
      {
         _emailAccountSettings = emailAccountSettings;
         _emailAccountService = emailAccountService;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare email account search model
      /// </summary>
      /// <param name="searchModel">Email account search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the email account search model
      /// </returns>
      public virtual Task<EmailAccountSearchModel> PrepareEmailAccountSearchModelAsync(EmailAccountSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare page parameters
         searchModel.SetGridPageSize();

         return Task.FromResult(searchModel);
      }

      /// <summary>
      /// Prepare paged email account list model
      /// </summary>
      /// <param name="searchModel">Email account search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the email account list model
      /// </returns>
      public virtual async Task<EmailAccountListModel> PrepareEmailAccountListModelAsync(EmailAccountSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get email accounts
         var emailAccounts = (await _emailAccountService.GetAllEmailAccountsAsync()).ToPagedList(searchModel);

         //prepare grid model
         var model = new EmailAccountListModel().PrepareToGrid(searchModel, emailAccounts, () =>
         {
            return emailAccounts.Select(emailAccount =>
               {
                  //fill in model values from the entity
                  var emailAccountModel = emailAccount.ToModel<EmailAccountModel>();

                  //fill in additional values (not existing in the entity)
                  emailAccountModel.IsDefaultEmailAccount = emailAccount.Id == _emailAccountSettings.DefaultEmailAccountId;

                  return emailAccountModel;
               });
         });

         return model;
      }

      /// <summary>
      /// Prepare email account model
      /// </summary>
      /// <param name="model">Email account model</param>
      /// <param name="emailAccount">Email account</param>
      /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the email account model
      /// </returns>
      public virtual Task<EmailAccountModel> PrepareEmailAccountModelAsync(EmailAccountModel model,
          EmailAccount emailAccount, bool excludeProperties = false)
      {
         //fill in model values from the entity
         if (emailAccount != null)
            model ??= emailAccount.ToModel<EmailAccountModel>();

         //set default values for the new model
         if (emailAccount == null)
            model.Port = 25;

         return Task.FromResult(model);
      }

      #endregion
   }
}