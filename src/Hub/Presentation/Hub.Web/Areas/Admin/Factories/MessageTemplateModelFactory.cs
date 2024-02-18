using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Messages;
using Hub.Data.Extensions;
using Hub.Services.Localization;
using Hub.Services.Messages;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Extensions;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the message template model factory implementation
   /// </summary>
   public partial class MessageTemplateModelFactory : IMessageTemplateModelFactory
   {
      #region Fields

      private readonly IBaseAdminModelFactory _baseAdminModelFactory;
      private readonly ILocalizationService _localizationService;
      private readonly ILocalizedModelFactory _localizedModelFactory;
      private readonly IMessageTemplateService _messageTemplateService;
      private readonly IMessageTokenProvider _messageTokenProvider;

      #endregion

      #region Ctor

      public MessageTemplateModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
          ILocalizationService localizationService,
          ILocalizedModelFactory localizedModelFactory,
          IMessageTemplateService messageTemplateService,
          IMessageTokenProvider messageTokenProvider)
      {
         _baseAdminModelFactory = baseAdminModelFactory;
         _localizationService = localizationService;
         _localizedModelFactory = localizedModelFactory;
         _messageTemplateService = messageTemplateService;
         _messageTokenProvider = messageTokenProvider;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare message template search model
      /// </summary>
      /// <param name="searchModel">Message template search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the message template search model
      /// </returns>
      public virtual Task<MessageTemplateSearchModel> PrepareMessageTemplateSearchModelAsync(MessageTemplateSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare page parameters
         searchModel.SetGridPageSize();

         return Task.FromResult(searchModel);
      }

      /// <summary>
      /// Prepare paged message template list model
      /// </summary>
      /// <param name="searchModel">Message template search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the message template list model
      /// </returns>
      public virtual async Task<MessageTemplateListModel> PrepareMessageTemplateListModelAsync(MessageTemplateSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get message templates
         var messageTemplates = (await _messageTemplateService.GetAllMessageTemplatesAsync(searchModel.SearchKeywords)).ToPagedList(searchModel);

         //prepare list model
         var model = await new MessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
         {
            return messageTemplates.SelectAwait(async messageTemplate =>
               {
                  //fill in model values from the entity
                  var messageTemplateModel = messageTemplate.ToModel<MessageTemplateModel>();

                  //TODO some section

                  return await Task.FromResult(messageTemplateModel);
               });
         });

         return model;
      }

      /// <summary>
      /// Prepare message template model
      /// </summary>
      /// <param name="model">Message template model</param>
      /// <param name="messageTemplate">Message template</param>
      /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the message template model
      /// </returns>
      public virtual async Task<MessageTemplateModel> PrepareMessageTemplateModelAsync(MessageTemplateModel model,
          MessageTemplate messageTemplate, bool excludeProperties = false)
      {
         Func<MessageTemplateLocalizedModel, long, Task> localizedModelConfiguration = null;

         if (messageTemplate != null)
         {
            //fill in model values from the entity
            model ??= messageTemplate.ToModel<MessageTemplateModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
               locale.BccEmailAddresses = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.BccEmailAddresses, languageId, false, false);
               locale.Subject = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId, false, false);
               locale.Body = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId, false, false);
               locale.EmailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.EmailAccountId, languageId, false, false);

               //prepare available email accounts
               await _baseAdminModelFactory.PrepareEmailAccountsAsync(locale.AvailableEmailAccounts,
                       defaultItemText: await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard"));

               //PrepareEmailAccounts only gets available accounts, we need to set the item as selected manually
               if (locale.AvailableEmailAccounts?.FirstOrDefault(x => x.Value == locale.EmailAccountId.ToString()) is SelectListItem emailAccountListItem)
                  emailAccountListItem.Selected = true;

            };
         }

         model.SendImmediately = !model.DelayBeforeSend.HasValue;
         model.HasAttachedDownload = model.AttachedDownloadId > 0;

         var allowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokensAsync(_messageTokenProvider.GetTokenGroups(messageTemplate)));
         model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}" +
             $"{await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";

         //prepare localized models
         if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

         //prepare available email accounts
         await _baseAdminModelFactory.PrepareEmailAccountsAsync(model.AvailableEmailAccounts);

         
         return model;
      }

      /// <summary>
      /// Prepare test message template model
      /// </summary>
      /// <param name="model">Test message template model</param>
      /// <param name="messageTemplate">Message template</param>
      /// <param name="languageId">Language identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the st message template model
      /// </returns>
      public virtual async Task<TestMessageTemplateModel> PrepareTestMessageTemplateModelAsync(TestMessageTemplateModel model,
          MessageTemplate messageTemplate, long languageId)
      {
         if (model == null)
            throw new ArgumentNullException(nameof(model));

         if (messageTemplate == null)
            throw new ArgumentNullException(nameof(messageTemplate));

         model.Id = messageTemplate.Id;
         model.LanguageId = languageId;

         //filter tokens to the current template
         var subject = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId);
         var body = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId);
         model.Tokens = (await _messageTokenProvider.GetListOfAllowedTokensAsync())
             .Where(token => subject.Contains(token) || body.Contains(token)).ToList();

         return model;
      }

      #endregion
   }
}