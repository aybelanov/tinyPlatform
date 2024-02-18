using System;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class EmailAccountController : BaseAdminController
{
   #region Fields

   private readonly EmailAccountSettings _emailAccountSettings;
   private readonly IUserActivityService _userActivityService;
   private readonly IEmailAccountModelFactory _emailAccountModelFactory;
   private readonly IEmailAccountService _emailAccountService;
   private readonly IEmailSender _emailSender;
   private readonly ILocalizationService _localizationService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly ISettingService _settingService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public EmailAccountController(EmailAccountSettings emailAccountSettings,
       IUserActivityService userActivityService,
       IEmailAccountModelFactory emailAccountModelFactory,
       IEmailAccountService emailAccountService,
       IEmailSender emailSender,
       ILocalizationService localizationService,
       INotificationService notificationService,
       IPermissionService permissionService,
       ISettingService settingService,
       IGenericAttributeService genericAttributeService,
       IWorkContext workContext)
   {
      _emailAccountSettings = emailAccountSettings;
      _userActivityService = userActivityService;
      _emailAccountModelFactory = emailAccountModelFactory;
      _emailAccountService = emailAccountService;
      _emailSender = emailSender;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _settingService = settingService;
      _genericAttributeService = genericAttributeService;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List(bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //prepare model
      var model = await _emailAccountModelFactory.PrepareEmailAccountSearchModelAsync(new EmailAccountSearchModel());

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> List(EmailAccountSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _emailAccountModelFactory.PrepareEmailAccountListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> MarkAsDefaultEmail(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
      if (defaultEmailAccount == null)
         return RedirectToAction("List");

      _emailAccountSettings.DefaultEmailAccountId = defaultEmailAccount.Id;
      await _settingService.SaveSettingAsync(_emailAccountSettings);

      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> Create()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //prepare model
      var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(new EmailAccountModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> Create(EmailAccountModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var emailAccount = model.ToEntity<EmailAccount>();

         //set password manually
         emailAccount.Password = model.Password;
         await _emailAccountService.InsertEmailAccountAsync(emailAccount);

         //activity log
         await _userActivityService.InsertActivityAsync("AddNewEmailAccount",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewEmailAccount"), emailAccount.Id), emailAccount);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Added"));

         return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
      }

      //prepare model
      model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> Edit(long id, bool showtour = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //try to get an email account with the specified id
      var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
      if (emailAccount == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(null, emailAccount);

      //show configuration tour
      if (showtour)
      {
         var user = await _workContext.GetCurrentUserAsync();
         var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
         var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

         if (!hideCard && !closeCard)
            ViewBag.ShowTour = true;
      }

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   [FormValueRequired("save", "save-continue")]
   public virtual async Task<IActionResult> Edit(EmailAccountModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //try to get an email account with the specified id
      var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
      if (emailAccount == null)
         return RedirectToAction("List");

      if (ModelState.IsValid)
      {
         emailAccount = model.ToEntity(emailAccount);
         await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

         //activity log
         await _userActivityService.InsertActivityAsync("EditEmailAccount",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditEmailAccount"), emailAccount.Id), emailAccount);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Updated"));

         return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
      }

      //prepare model
      model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("changepassword")]
   public virtual async Task<IActionResult> ChangePassword(EmailAccountModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //try to get an email account with the specified id
      var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
      if (emailAccount == null)
         return RedirectToAction("List");

      //do not validate model
      emailAccount.Password = model.Password;
      await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.Password.PasswordChanged"));

      return RedirectToAction("Edit", new { id = emailAccount.Id });
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("sendtestemail")]
   public virtual async Task<IActionResult> SendTestEmail(EmailAccountModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //try to get an email account with the specified id
      var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
      if (emailAccount == null)
         return RedirectToAction("List");

      if (!CommonHelper.IsValidEmail(model.SendTestEmailTo))
      {
         _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
         return View(model);
      }

      try
      {
         if (string.IsNullOrWhiteSpace(model.SendTestEmailTo))
            throw new AppException("Enter test email address");
         var subject = "Testing email functionality.";
         var body = "Email works fine.";
         await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, model.SendTestEmailTo, null);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.SendTestEmail.Success"));
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
      }

      //prepare model
      model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
         return AccessDeniedView();

      //try to get an email account with the specified id
      var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
      if (emailAccount == null)
         return RedirectToAction("List");

      try
      {
         await _emailAccountService.DeleteEmailAccountAsync(emailAccount);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteEmailAccount",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteEmailAccount"), emailAccount.Id), emailAccount);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Deleted"));

         return RedirectToAction("List");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("Edit", new { id = emailAccount.Id });
      }
   }

   #endregion
}