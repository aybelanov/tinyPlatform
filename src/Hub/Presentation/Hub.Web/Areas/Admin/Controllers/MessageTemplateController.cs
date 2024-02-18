﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Messages;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class MessageTemplateController : BaseAdminController
{
   #region Fields

   private readonly IUserActivityService _userActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly ILocalizedEntityService _localizedEntityService;
   private readonly IMessageTemplateModelFactory _messageTemplateModelFactory;
   private readonly IMessageTemplateService _messageTemplateService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IWorkflowMessageService _workflowMessageService;

   #endregion Fields

   #region Ctor

   public MessageTemplateController(IUserActivityService userActivityService,
       ILocalizationService localizationService,
       ILocalizedEntityService localizedEntityService,
       IMessageTemplateModelFactory messageTemplateModelFactory,
       IMessageTemplateService messageTemplateService,
       INotificationService notificationService,
       IPermissionService permissionService,       
       IWorkflowMessageService workflowMessageService)
   {
      _userActivityService = userActivityService;
      _localizationService = localizationService;
      _localizedEntityService = localizedEntityService;
      _messageTemplateModelFactory = messageTemplateModelFactory;
      _messageTemplateService = messageTemplateService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _workflowMessageService = workflowMessageService;
   }

   #endregion

   #region Utilities

   protected virtual async Task UpdateLocalesAsync(MessageTemplate mt, MessageTemplateModel model)
   {
      foreach (var localized in model.Locales)
      {
         await _localizedEntityService.SaveLocalizedValueAsync(mt,
             x => x.BccEmailAddresses,
             localized.BccEmailAddresses,
             localized.LanguageId);

         await _localizedEntityService.SaveLocalizedValueAsync(mt,
             x => x.Subject,
             localized.Subject,
             localized.LanguageId);

         await _localizedEntityService.SaveLocalizedValueAsync(mt,
             x => x.Body,
             localized.Body,
             localized.LanguageId);

         await _localizedEntityService.SaveLocalizedValueAsync(mt,
             x => x.EmailAccountId,
             localized.EmailAccountId,
             localized.LanguageId);
      }
   }

   #endregion

   #region Methods

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //prepare model
      var model = await _messageTemplateModelFactory.PrepareMessageTemplateSearchModelAsync(new MessageTemplateSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> List(MessageTemplateSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _messageTemplateModelFactory.PrepareMessageTemplateListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> Edit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //try to get a message template with the specified id
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
      if (messageTemplate == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _messageTemplateModelFactory.PrepareMessageTemplateModelAsync(null, messageTemplate);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   [FormValueRequired("save", "save-continue")]
   public virtual async Task<IActionResult> Edit(MessageTemplateModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //try to get a message template with the specified id
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
      if (messageTemplate == null)
         return RedirectToAction("List");

      if (ModelState.IsValid)
      {
         messageTemplate = model.ToEntity(messageTemplate);

         //attached file
         if (!model.HasAttachedDownload)
            messageTemplate.AttachedDownloadId = 0;
         if (model.SendImmediately)
            messageTemplate.DelayBeforeSend = null;
         await _messageTemplateService.UpdateMessageTemplateAsync(messageTemplate);

         //activity log
         await _userActivityService.InsertActivityAsync("EditMessageTemplate",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMessageTemplate"), messageTemplate.Id), messageTemplate);

         //locales
         await UpdateLocalesAsync(messageTemplate, model);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Updated"));

         if (!continueEditing)
            return RedirectToAction("List");

         return RedirectToAction("Edit", new { id = messageTemplate.Id });
      }

      //prepare model
      model = await _messageTemplateModelFactory.PrepareMessageTemplateModelAsync(model, messageTemplate, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //try to get a message template with the specified id
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
      if (messageTemplate == null)
         return RedirectToAction("List");

      await _messageTemplateService.DeleteMessageTemplateAsync(messageTemplate);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteMessageTemplate",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMessageTemplate"), messageTemplate.Id), messageTemplate);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Deleted"));

      return RedirectToAction("List");
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("message-template-copy")]
   public virtual async Task<IActionResult> CopyTemplate(MessageTemplateModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //try to get a message template with the specified id
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
      if (messageTemplate == null)
         return RedirectToAction("List");

      try
      {
         var newMessageTemplate = await _messageTemplateService.CopyMessageTemplateAsync(messageTemplate);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Copied"));

         return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
         return RedirectToAction("Edit", new { id = model.Id });
      }
   }

   public virtual async Task<IActionResult> TestTemplate(long id, long languageId = 0)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //try to get a message template with the specified id
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
      if (messageTemplate == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _messageTemplateModelFactory
          .PrepareTestMessageTemplateModelAsync(new TestMessageTemplateModel(), messageTemplate, languageId);

      return View(model);
   }

   [HttpPost, ActionName("TestTemplate")]
   [FormValueRequired("send-test")]
   public virtual async Task<IActionResult> TestTemplate(TestMessageTemplateModel model, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
         return AccessDeniedView();

      //try to get a message template with the specified id
      var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
      if (messageTemplate == null)
         return RedirectToAction("List");

      var tokens = new List<Token>();
      foreach (var formKey in form.Keys)
         if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
         {
            var tokenKey = formKey["token_".Length..].Replace("%", string.Empty);
            var stringValue = form[formKey].ToString();

            //try get non-string value
            object tokenValue;
            if (bool.TryParse(stringValue, out var boolValue))
               tokenValue = boolValue;
            else if (int.TryParse(stringValue, out var intValue))
               tokenValue = intValue;
            else if (decimal.TryParse(stringValue, out var decimalValue))
               tokenValue = decimalValue;
            else
               tokenValue = stringValue;

            tokens.Add(new Token(tokenKey, tokenValue));
         }

      await _workflowMessageService.SendTestEmailAsync(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

      if (ModelState.IsValid)
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Test.Success"));

      return RedirectToAction("Edit", new { id = messageTemplate.Id });
   }

   #endregion
}