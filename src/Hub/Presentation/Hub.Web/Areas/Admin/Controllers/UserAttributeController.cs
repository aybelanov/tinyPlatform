using Hub.Core.Domain.Users;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers
{
   public partial class UserAttributeController : BaseAdminController
   {
      #region Fields

      private readonly IUserActivityService _userActivityService;
      private readonly IUserAttributeModelFactory _userAttributeModelFactory;
      private readonly IUserAttributeService _userAttributeService;
      private readonly ILocalizationService _localizationService;
      private readonly ILocalizedEntityService _localizedEntityService;
      private readonly INotificationService _notificationService;
      private readonly IPermissionService _permissionService;

      #endregion

      #region Ctor

      public UserAttributeController(IUserActivityService userActivityService,
          IUserAttributeModelFactory userAttributeModelFactory,
          IUserAttributeService userAttributeService,
          ILocalizationService localizationService,
          ILocalizedEntityService localizedEntityService,
          INotificationService notificationService,
          IPermissionService permissionService)
      {
         _userActivityService = userActivityService;
         _userAttributeModelFactory = userAttributeModelFactory;
         _userAttributeService = userAttributeService;
         _localizationService = localizationService;
         _localizedEntityService = localizedEntityService;
         _notificationService = notificationService;
         _permissionService = permissionService;
      }

      #endregion

      #region Utilities

      protected virtual async Task UpdateAttributeLocalesAsync(UserAttribute userAttribute, UserAttributeModel model)
      {
         foreach (var localized in model.Locales)
            await _localizedEntityService.SaveLocalizedValueAsync(userAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
      }

      protected virtual async Task UpdateValueLocalesAsync(UserAttributeValue userAttributeValue, UserAttributeValueModel model)
      {
         foreach (var localized in model.Locales)
            await _localizedEntityService.SaveLocalizedValueAsync(userAttributeValue,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
      }

      #endregion

      #region User attributes

      public virtual IActionResult Index()
      {
         return RedirectToAction("List");
      }

      public virtual async Task<IActionResult> List()
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //select an appropriate card
         SaveSelectedCardName("usersettings-userformfields");

         //we just redirect a user to the user settings page
         return RedirectToAction("UserUser", "Setting");
      }

      [HttpPost]
      public virtual async Task<IActionResult> List(UserAttributeSearchModel searchModel)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return await AccessDeniedDataTablesJson();

         //prepare model
         var model = await _userAttributeModelFactory.PrepareUserAttributeListModelAsync(searchModel);

         return Json(model);
      }

      public virtual async Task<IActionResult> Create()
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //prepare model
         var model = await _userAttributeModelFactory.PrepareUserAttributeModelAsync(new UserAttributeModel(), null);

         return View(model);
      }

      [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
      public virtual async Task<IActionResult> Create(UserAttributeModel model, bool continueEditing)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         if (ModelState.IsValid)
         {
            var userAttribute = model.ToEntity<UserAttribute>();
            await _userAttributeService.InsertUserAttributeAsync(userAttribute);

            //activity log
            await _userActivityService.InsertActivityAsync("AddNewUserAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewUserAttribute"), userAttribute.Id),
                userAttribute);

            //locales
            await UpdateAttributeLocalesAsync(userAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserAttributes.Added"));

            if (!continueEditing)
               return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = userAttribute.Id });
         }

         //prepare model
         model = await _userAttributeModelFactory.PrepareUserAttributeModelAsync(model, null, true);

         //if we got this far, something failed, redisplay form
         return View(model);
      }

      public virtual async Task<IActionResult> Edit(long id)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //try to get a user attribute with the specified id
         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(id);
         if (userAttribute == null)
            return RedirectToAction("List");

         //prepare model
         var model = await _userAttributeModelFactory.PrepareUserAttributeModelAsync(null, userAttribute);

         return View(model);
      }

      [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
      public virtual async Task<IActionResult> Edit(UserAttributeModel model, bool continueEditing)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(model.Id);
         if (userAttribute == null)
            //no user attribute found with the specified id
            return RedirectToAction("List");

         if (!ModelState.IsValid)
            //if we got this far, something failed, redisplay form
            return View(model);

         userAttribute = model.ToEntity(userAttribute);
         await _userAttributeService.UpdateUserAttributeAsync(userAttribute);

         //activity log
         await _userActivityService.InsertActivityAsync("EditUserAttribute",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditUserAttribute"), userAttribute.Id),
             userAttribute);

         //locales
         await UpdateAttributeLocalesAsync(userAttribute, model);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserAttributes.Updated"));

         if (!continueEditing)
            return RedirectToAction("List");

         return RedirectToAction("Edit", new { id = userAttribute.Id });
      }

      [HttpPost]
      public virtual async Task<IActionResult> Delete(long id)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(id);
         await _userAttributeService.DeleteUserAttributeAsync(userAttribute);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteUserAttribute",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteUserAttribute"), userAttribute.Id),
             userAttribute);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserAttributes.Deleted"));
         return RedirectToAction("List");
      }

      #endregion

      #region User attribute values

      [HttpPost]
      public virtual async Task<IActionResult> ValueList(UserAttributeValueSearchModel searchModel)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return await AccessDeniedDataTablesJson();

         //try to get a user attribute with the specified id
         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(searchModel.UserAttributeId)
             ?? throw new ArgumentException("No user attribute found with the specified id");

         //prepare model
         var model = await _userAttributeModelFactory.PrepareUserAttributeValueListModelAsync(searchModel, userAttribute);

         return Json(model);
      }

      public virtual async Task<IActionResult> ValueCreatePopup(long userAttributeId)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //try to get a user attribute with the specified id
         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(userAttributeId);
         if (userAttribute == null)
            return RedirectToAction("List");

         //prepare model
         var model = await _userAttributeModelFactory
             .PrepareUserAttributeValueModelAsync(new UserAttributeValueModel(), userAttribute, null);

         return View(model);
      }

      [HttpPost]
      public virtual async Task<IActionResult> ValueCreatePopup(UserAttributeValueModel model)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //try to get a user attribute with the specified id
         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(model.UserAttributeId);
         if (userAttribute == null)
            return RedirectToAction("List");

         if (ModelState.IsValid)
         {
            var cav = model.ToEntity<UserAttributeValue>();
            await _userAttributeService.InsertUserAttributeValueAsync(cav);

            //activity log
            await _userActivityService.InsertActivityAsync("AddNewUserAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewUserAttributeValue"), cav.Id), cav);

            await UpdateValueLocalesAsync(cav, model);

            ViewBag.RefreshPage = true;

            return View(model);
         }

         //prepare model
         model = await _userAttributeModelFactory.PrepareUserAttributeValueModelAsync(model, userAttribute, null, true);

         //if we got this far, something failed, redisplay form
         return View(model);
      }

      public virtual async Task<IActionResult> ValueEditPopup(long id)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //try to get a user attribute value with the specified id
         var userAttributeValue = await _userAttributeService.GetUserAttributeValueByIdAsync(id);
         if (userAttributeValue == null)
            return RedirectToAction("List");

         //try to get a user attribute with the specified id
         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(userAttributeValue.UserAttributeId);
         if (userAttribute == null)
            return RedirectToAction("List");

         //prepare model
         var model = await _userAttributeModelFactory.PrepareUserAttributeValueModelAsync(null, userAttribute, userAttributeValue);

         return View(model);
      }

      [HttpPost]
      public virtual async Task<IActionResult> ValueEditPopup(UserAttributeValueModel model)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //try to get a user attribute value with the specified id
         var userAttributeValue = await _userAttributeService.GetUserAttributeValueByIdAsync(model.Id);
         if (userAttributeValue == null)
            return RedirectToAction("List");

         //try to get a user attribute with the specified id
         var userAttribute = await _userAttributeService.GetUserAttributeByIdAsync(userAttributeValue.UserAttributeId);
         if (userAttribute == null)
            return RedirectToAction("List");

         if (ModelState.IsValid)
         {
            userAttributeValue = model.ToEntity(userAttributeValue);
            await _userAttributeService.UpdateUserAttributeValueAsync(userAttributeValue);

            //activity log
            await _userActivityService.InsertActivityAsync("EditUserAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditUserAttributeValue"), userAttributeValue.Id),
                userAttributeValue);

            await UpdateValueLocalesAsync(userAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
         }

         //prepare model
         model = await _userAttributeModelFactory.PrepareUserAttributeValueModelAsync(model, userAttribute, userAttributeValue, true);

         //if we got this far, something failed, redisplay form
         return View(model);
      }

      [HttpPost]
      public virtual async Task<IActionResult> ValueDelete(long id)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            return AccessDeniedView();

         //try to get a user attribute value with the specified id
         var userAttributeValue = await _userAttributeService.GetUserAttributeValueByIdAsync(id)
             ?? throw new ArgumentException("No user attribute value found with the specified id", nameof(id));

         await _userAttributeService.DeleteUserAttributeValueAsync(userAttributeValue);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteUserAttributeValue",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteUserAttributeValue"), userAttributeValue.Id),
             userAttributeValue);

         return new NullJsonResult();
      }

      #endregion
   }
}