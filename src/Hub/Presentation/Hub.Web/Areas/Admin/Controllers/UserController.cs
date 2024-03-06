using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Gdpr;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.ExportImport;
using Hub.Services.Forums;
using Hub.Services.Gdpr;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Areas.Admin.Models.Gdpr;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Shared.Clients.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class UserController : BaseAdminController
{
   #region Fields

   private readonly UserSettings _userSettings;
   private readonly DateTimeSettings _dateTimeSettings;
   private readonly EmailAccountSettings _emailAccountSettings;
   private readonly ForumSettings _forumSettings;
   private readonly GdprSettings _gdprSettings;
   private readonly IAddressAttributeParser _addressAttributeParser;
   private readonly IAddressService _addressService;
   private readonly IUserActivityService _userActivityService;
   private readonly IUserAttributeParser _userAttributeParser;
   private readonly IUserAttributeService _userAttributeService;
   private readonly IUserModelFactory _userModelFactory;
   private readonly IHubDeviceService _deviceService;
   private readonly IDeviceModelFactory _deviceModelFactory;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IUserRegistrationService _userRegistrationService;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IEmailAccountService _emailAccountService;
   private readonly IEventPublisher _eventPublisher;
   private readonly IExportManager _exportManager;
   private readonly IForumService _forumService;
   private readonly IGdprService _gdprService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly ILocalizationService _localizationService;
   private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IQueuedEmailService _queuedEmailService;
   private readonly IWorkContext _workContext;
   private readonly IWorkflowMessageService _workflowMessageService;

   #endregion

   #region Ctor

   public UserController(UserSettings userSettings,
       DateTimeSettings dateTimeSettings,
       EmailAccountSettings emailAccountSettings,
       ForumSettings forumSettings,
       GdprSettings gdprSettings,
       IAddressAttributeParser addressAttributeParser,
       IAddressService addressService,
       IUserActivityService userActivityService,
       IUserAttributeParser userAttributeParser,
       IUserAttributeService userAttributeService,
       IUserModelFactory userModelFactory,
       IUserRegistrationService userRegistrationService,
       IUserService userService,
       IHubDeviceService deviceService,
       IDeviceModelFactory deviceModelFactory,
       IDeviceActivityService deviceActivityService,
       IDateTimeHelper dateTimeHelper,
       IEmailAccountService emailAccountService,
       IEventPublisher eventPublisher,
       IExportManager exportManager,
       IForumService forumService,
       IGdprService gdprService,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       INewsLetterSubscriptionService newsLetterSubscriptionService,
       INotificationService notificationService,
       IPermissionService permissionService,
       IQueuedEmailService queuedEmailService,
       IWorkContext workContext,
       IWorkflowMessageService workflowMessageService)
   {
      _userSettings = userSettings;
      _dateTimeSettings = dateTimeSettings;
      _emailAccountSettings = emailAccountSettings;
      _forumSettings = forumSettings;
      _gdprSettings = gdprSettings;
      _addressAttributeParser = addressAttributeParser;
      _addressService = addressService;
      _userActivityService = userActivityService;
      _userAttributeParser = userAttributeParser;
      _userAttributeService = userAttributeService;
      _userModelFactory = userModelFactory;
      _deviceService = deviceService;
      _deviceModelFactory = deviceModelFactory;
      _deviceActivityService = deviceActivityService;
      _userRegistrationService = userRegistrationService;
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _emailAccountService = emailAccountService;
      _eventPublisher = eventPublisher;
      _exportManager = exportManager;
      _forumService = forumService;
      _gdprService = gdprService;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _newsLetterSubscriptionService = newsLetterSubscriptionService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _queuedEmailService = queuedEmailService;
      _workContext = workContext;
      _workflowMessageService = workflowMessageService;
   }

   #endregion

   #region Utilities

   protected virtual async Task<string> ValidateUserRolesAsync(IList<UserRole> userRoles, IList<UserRole> existingUserRoles)
   {
      if (userRoles == null)
         throw new ArgumentNullException(nameof(userRoles));

      if (existingUserRoles == null)
         throw new ArgumentNullException(nameof(existingUserRoles));

      //check ACL permission to manage user roles
      var rolesToAdd = userRoles.Except(existingUserRoles, new UserRoleComparerByName());
      var rolesToDelete = existingUserRoles.Except(userRoles, new UserRoleComparerByName());
      if (rolesToAdd.Any(role => role.SystemName != UserDefaults.RegisteredRoleName) || rolesToDelete.Any())
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
            return await _localizationService.GetResourceAsync("Admin.Users.Users.UserRolesManagingError");

      //ensure a user is not added to both 'Guests' and 'Registered' user roles
      //ensure that a user is in at least one required role ('Guests' and 'Registered')
      var isInGuestsRole = userRoles.FirstOrDefault(cr => cr.SystemName == UserDefaults.GuestsRoleName) != null;
      var isInRegisteredRole = userRoles.FirstOrDefault(cr => cr.SystemName == UserDefaults.RegisteredRoleName) != null;
      if (isInGuestsRole && isInRegisteredRole)
         return await _localizationService.GetResourceAsync("Admin.Users.Users.GuestsAndRegisteredRolesError");
      if (!isInGuestsRole && !isInRegisteredRole)
         return await _localizationService.GetResourceAsync("Admin.Users.Users.AddUserToGuestsOrRegisteredRoleError");

      //no errors
      return string.Empty;
   }

   protected virtual async Task<string> ParseCustomUserAttributesAsync(IFormCollection form)
   {
      if (form == null)
         throw new ArgumentNullException(nameof(form));

      var attributesXml = string.Empty;
      var userAttributes = await _userAttributeService.GetAllUserAttributesAsync();
      foreach (var attribute in userAttributes)
      {
         var controlId = $"{AppUserServicesDefaults.UserAttributePrefix}{attribute.Id}";
         StringValues ctrlAttributes;

         switch (attribute.AttributeControlType)
         {
            case AttributeControlType.DropdownList:
            case AttributeControlType.RadioList:
               ctrlAttributes = form[controlId];
               if (!StringValues.IsNullOrEmpty(ctrlAttributes))
               {
                  var selectedAttributeId = int.Parse(ctrlAttributes);
                  if (selectedAttributeId > 0)
                     attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
                         attribute, selectedAttributeId.ToString());
               }

               break;
            case AttributeControlType.Checkboxes:
               var cblAttributes = form[controlId];
               if (!StringValues.IsNullOrEmpty(cblAttributes))
                  foreach (var item in cblAttributes.ToString()
                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                  {
                     var selectedAttributeId = int.Parse(item);
                     if (selectedAttributeId > 0)
                        attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                  }

               break;
            case AttributeControlType.ReadonlyCheckboxes:
               //load read-only (already server-side selected) values
               var attributeValues = await _userAttributeService.GetUserAttributeValuesAsync(attribute.Id);
               foreach (var selectedAttributeId in attributeValues
                   .Where(v => v.IsPreSelected)
                   .Select(v => v.Id)
                   .ToList())
                  attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
                      attribute, selectedAttributeId.ToString());

               break;
            case AttributeControlType.TextBox:
            case AttributeControlType.MultilineTextbox:
               ctrlAttributes = form[controlId];
               if (!StringValues.IsNullOrEmpty(ctrlAttributes))
               {
                  var enteredText = ctrlAttributes.ToString().Trim();
                  attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
                      attribute, enteredText);
               }

               break;
            case AttributeControlType.Datepicker:
            case AttributeControlType.ColorSquares:
            case AttributeControlType.ImageSquares:
            case AttributeControlType.FileUpload:
            //not supported user attributes
            default:
               break;
         }
      }

      return attributesXml;
   }

   private async Task<bool> SecondAdminAccountExistsAsync(User user)
   {
      var users = await _userService.GetAllUsersAsync(userRoleIds: new[] { (await _userService.GetUserRoleBySystemNameAsync(UserDefaults.AdministratorsRoleName)).Id });

      return users.Any(c => c.IsActive && c.Id != user.Id);
   }

   #endregion

   #region Users

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _userModelFactory.PrepareUserSearchModelAsync(new UserSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> UserList(UserSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _userModelFactory.PrepareUserListModelAsync(searchModel);

      return Json(model);
   }

   public virtual async Task<IActionResult> Create()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _userModelFactory.PrepareUserModelAsync(new UserModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   [FormValueRequired("save", "save-continue")]
   public virtual async Task<IActionResult> Create(UserModel model, bool continueEditing, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      if (!string.IsNullOrWhiteSpace(model.Email) && await _userService.GetUserByEmailAsync(model.Email) != null)
         ModelState.AddModelError(string.Empty, "Email is already registered");

      if (!string.IsNullOrWhiteSpace(model.Username) && _userSettings.UsernamesEnabled &&
          await _userService.GetUserByUsernameAsync(model.Username) != null)
         ModelState.AddModelError(string.Empty, "Username is already registered");

      //validate user roles
      var allUserRoles = await _userService.GetAllUserRolesAsync(true);
      var newUserRoles = new List<UserRole>();
      foreach (var userRole in allUserRoles)
         if (model.SelectedUserRoleIds.Contains(userRole.Id))
            newUserRoles.Add(userRole);
      var userRolesError = await ValidateUserRolesAsync(newUserRoles, new List<UserRole>());
      if (!string.IsNullOrEmpty(userRolesError))
      {
         ModelState.AddModelError(string.Empty, userRolesError);
         _notificationService.ErrorNotification(userRolesError);
      }

      // Ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
      if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == UserDefaults.RegisteredRoleName) != null &&
          !CommonHelper.IsValidEmail(model.Email))
      {
         ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));

         _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
      }

      //custom user attributes
      var userAttributesXml = await ParseCustomUserAttributesAsync(form);
      if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == UserDefaults.RegisteredRoleName) != null)
      {
         var userAttributeWarnings = await _userAttributeParser.GetAttributeWarningsAsync(userAttributesXml);
         foreach (var error in userAttributeWarnings)
            ModelState.AddModelError(string.Empty, error);
      }

      if (ModelState.IsValid)
      {
         //fill entity from model
         var user = model.ToEntity<User>();

         user.UserGuid = Guid.NewGuid();
         user.CreatedOnUtc = DateTime.UtcNow;
         user.LastActivityUtc = DateTime.UtcNow;

         await _userService.InsertUserAsync(user);

         //form fields
         if (_dateTimeSettings.AllowUsersToSetTimeZone)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
         if (_userSettings.GenderEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.GenderAttribute, model.Gender);
         if (_userSettings.FirstNameEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.FirstNameAttribute, model.FirstName);
         if (_userSettings.LastNameEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.LastNameAttribute, model.LastName);
         if (_userSettings.DateOfBirthEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.DateOfBirthAttribute, model.DateOfBirth);
         if (_userSettings.CompanyEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CompanyAttribute, model.Company);
         if (_userSettings.StreetAddressEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.StreetAddressAttribute, model.StreetAddress);
         if (_userSettings.StreetAddress2Enabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.StreetAddress2Attribute, model.StreetAddress2);
         if (_userSettings.ZipPostalCodeEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
         if (_userSettings.CityEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CityAttribute, model.City);
         if (_userSettings.CountyEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CountyAttribute, model.County);
         if (_userSettings.CountryEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CountryIdAttribute, model.CountryId);
         if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.StateProvinceIdAttribute, model.StateProvinceId);
         if (_userSettings.PhoneEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.PhoneAttribute, model.Phone);
         if (_userSettings.FaxEnabled)
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.FaxAttribute, model.Fax);

         //custom user attributes
         await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CustomUserAttributes, userAttributesXml);

         //newsletter subscriptions
         if (!string.IsNullOrEmpty(user.Email))
         {
            var newsletterSubscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAsync(user.Email);

            //subscribed
            if (newsletterSubscription == null)
               await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
               {
                  NewsLetterSubscriptionGuid = Guid.NewGuid(),
                  Email = user.Email,
                  Active = true,
                  CreatedOnUtc = DateTime.UtcNow
               });

            //not subscribed
            else
               await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletterSubscription);
         }

         //password
         if (!string.IsNullOrWhiteSpace(model.Password))
         {
            var changePassRequest = new ChangePasswordRequest(model.Email, false, _userSettings.DefaultPasswordFormat, model.Password);
            var changePassResult = await _userRegistrationService.ChangePasswordAsync(changePassRequest);
            if (!changePassResult.Success)
               foreach (var changePassError in changePassResult.Errors)
                  _notificationService.ErrorNotification(changePassError);
         }

         //user roles
         foreach (var userRole in newUserRoles)
         {
            //ensure that the current user cannot add to "Administrators" system role if he's not an admin himself
            if (userRole.SystemName == UserDefaults.AdministratorsRoleName && !await _userService.IsAdminAsync(await _workContext.GetCurrentUserAsync()))
               continue;

            await _userService.AddUserRoleMappingAsync(new UserUserRole { UserId = user.Id, UserRoleId = userRole.Id });
         }

         await _userService.UpdateUserAsync(user);

         //activity log
         await _userActivityService.InsertActivityAsync("AddNewUser",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewUser"), user.Id), user);
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.Added"));

         if (!continueEditing)
            return RedirectToAction("List");

         return RedirectToAction("Edit", new { id = user.Id });
      }

      //prepare model
      model = await _userModelFactory.PrepareUserModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> Edit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null || user.IsDeleted)
         return RedirectToAction("List");

      //prepare model
      var model = await _userModelFactory.PrepareUserModelAsync(null, user);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   [FormValueRequired("save", "save-continue")]
   public virtual async Task<IActionResult> Edit(UserModel model, bool continueEditing, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null || user.IsDeleted)
         return RedirectToAction("List");

      //validate user roles
      var allUserRoles = await _userService.GetAllUserRolesAsync(true);
      var newUserRoles = new List<UserRole>();
      foreach (var userRole in allUserRoles)
         if (model.SelectedUserRoleIds.Contains(userRole.Id))
            newUserRoles.Add(userRole);

      var userRolesError = await ValidateUserRolesAsync(newUserRoles, await _userService.GetUserRolesAsync(user));

      if (!string.IsNullOrEmpty(userRolesError))
      {
         ModelState.AddModelError(string.Empty, userRolesError);
         _notificationService.ErrorNotification(userRolesError);
      }

      // Ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
      if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == UserDefaults.RegisteredRoleName) != null &&
          !CommonHelper.IsValidEmail(model.Email))
      {
         ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
         _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
      }

      //custom user attributes
      var userAttributesXml = await ParseCustomUserAttributesAsync(form);
      if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == UserDefaults.RegisteredRoleName) != null)
      {
         var userAttributeWarnings = await _userAttributeParser.GetAttributeWarningsAsync(userAttributesXml);
         foreach (var error in userAttributeWarnings)
            ModelState.AddModelError(string.Empty, error);
      }

      if (ModelState.IsValid)
      {
         try
         {
            user.AdminComment = model.AdminComment;

            //prevent deactivation of the last active administrator
            if (!await _userService.IsAdminAsync(user) || model.IsActive || await SecondAdminAccountExistsAsync(user))
               user.IsActive = model.IsActive;
            else
               _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.AdminAccountShouldExists.Deactivate"));

            //email
            if (!string.IsNullOrWhiteSpace(model.Email))
               await _userRegistrationService.SetEmailAsync(user, model.Email, false);
            else
               user.Email = model.Email;

            //username
            if (_userSettings.UsernamesEnabled)
               if (!string.IsNullOrWhiteSpace(model.Username))
                  await _userRegistrationService.SetUsernameAsync(user, model.Username);
               else
                  user.Username = model.Username;

            //form fields
            if (_dateTimeSettings.AllowUsersToSetTimeZone)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
            if (_userSettings.GenderEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.GenderAttribute, model.Gender);
            if (_userSettings.FirstNameEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.FirstNameAttribute, model.FirstName);
            if (_userSettings.LastNameEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.LastNameAttribute, model.LastName);
            if (_userSettings.DateOfBirthEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.DateOfBirthAttribute, model.DateOfBirth);
            if (_userSettings.CompanyEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CompanyAttribute, model.Company);
            if (_userSettings.StreetAddressEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.StreetAddressAttribute, model.StreetAddress);
            if (_userSettings.StreetAddress2Enabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.StreetAddress2Attribute, model.StreetAddress2);
            if (_userSettings.ZipPostalCodeEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
            if (_userSettings.CityEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CityAttribute, model.City);
            if (_userSettings.CountyEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CountyAttribute, model.County);
            if (_userSettings.CountryEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CountryIdAttribute, model.CountryId);
            if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.StateProvinceIdAttribute, model.StateProvinceId);
            if (_userSettings.PhoneEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.PhoneAttribute, model.Phone);
            if (_userSettings.FaxEnabled)
               await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.FaxAttribute, model.Fax);

            //custom user attributes
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.CustomUserAttributes, userAttributesXml);

            //newsletter subscriptions
            if (!string.IsNullOrEmpty(user.Email))
            {
               var newsletterSubscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAsync(user.Email);

               //subscribe
               if (model.NewsletterSubscribed)
               {
                  if (newsletterSubscription == null)
                     await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                     {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = user.Email,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow
                     });
               }
               else
               {
                  //unsubscribe
                  if (newsletterSubscription != null)
                     await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletterSubscription);
               }
            }

            var currentUserRoleIds = await _userService.GetUserRoleIdsAsync(user, true);

            //user roles
            foreach (var userRole in allUserRoles)
            {
               //ensure that the current user cannot add/remove to/from "Administrators" system role
               //if he's not an admin himself
               if (userRole.SystemName == UserDefaults.AdministratorsRoleName &&
                   !await _userService.IsAdminAsync(await _workContext.GetCurrentUserAsync()))
                  continue;

               if (model.SelectedUserRoleIds.Contains(userRole.Id))
               {
                  //new role
                  if (currentUserRoleIds.All(roleId => roleId != userRole.Id))
                     await _userService.AddUserRoleMappingAsync(new UserUserRole { UserId = user.Id, UserRoleId = userRole.Id });
               }
               else
               {
                  //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                  if (userRole.SystemName == UserDefaults.AdministratorsRoleName && !await SecondAdminAccountExistsAsync(user))
                  {
                     _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.AdminAccountShouldExists.DeleteRole"));
                     continue;
                  }

                  //remove role
                  if (currentUserRoleIds.Any(roleId => roleId == userRole.Id))
                     await _userService.RemoveUserRoleMappingAsync(user, userRole);
               }
            }

            await _userService.UpdateUserAsync(user);

            //activity log
            await _userActivityService.InsertActivityAsync("EditUser",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditUser"), user.Id), user);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.Updated"));

            if (!continueEditing)
               return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = user.Id });
         }
         catch (Exception exc)
         {
            _notificationService.ErrorNotification(exc.Message);
         }
      }
      //prepare model
      model = await _userModelFactory.PrepareUserModelAsync(model, user, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("changepassword")]
   public virtual async Task<IActionResult> ChangePassword(UserModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null)
         return RedirectToAction("List");

      //ensure that the current user cannot change passwords of "Administrators" if he's not an admin himself
      if (await _userService.IsAdminAsync(user) && !await _userService.IsAdminAsync(await _workContext.GetCurrentUserAsync()))
      {
         _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.OnlyAdminCanChangePassword"));
         return RedirectToAction("Edit", new { id = user.Id });
      }

      if (!ModelState.IsValid)
         return RedirectToAction("Edit", new { id = user.Id });

      var changePassRequest = new ChangePasswordRequest(model.Email,
          false, _userSettings.DefaultPasswordFormat, model.Password);
      var changePassResult = await _userRegistrationService.ChangePasswordAsync(changePassRequest);
      if (changePassResult.Success)
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.PasswordChanged"));
      else
         foreach (var error in changePassResult.Errors)
            _notificationService.ErrorNotification(error);

      return RedirectToAction("Edit", new { id = user.Id });
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("remove-affiliate")]
   public virtual async Task<IActionResult> RemoveAffiliate(UserModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null)
         return RedirectToAction("List");

      user.AffiliateId = 0;
      await _userService.UpdateUserAsync(user);

      return RedirectToAction("Edit", new { id = user.Id });
   }

   [HttpPost]
   public virtual async Task<IActionResult> RemoveBindMFA(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
         return RedirectToAction("List");

      await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

      //raise event       
      await _eventPublisher.PublishAsync(new UserChangeMultiFactorAuthenticationProviderEvent(user));

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.UnbindMFAProvider"));

      return RedirectToAction("Edit", new { id = user.Id });
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
         return RedirectToAction("List");

      try
      {
         //prevent attempts to delete the user, if it is the last active administrator
         if (await _userService.IsAdminAsync(user) && !await SecondAdminAccountExistsAsync(user))
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.AdminAccountShouldExists.DeleteAdministrator"));
            return RedirectToAction("Edit", new { id = user.Id });
         }

         //ensure that the current user cannot delete "Administrators" if he's not an admin himself
         if (await _userService.IsAdminAsync(user) && !await _userService.IsAdminAsync(await _workContext.GetCurrentUserAsync()))
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.OnlyAdminCanDeleteAdmin"));
            return RedirectToAction("Edit", new { id = user.Id });
         }

         //delete
         await _userService.DeleteUserAsync(user);


         var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAsync(user.Email);
         if (subscription != null)
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteUser",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteUser"), user.Id), user);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.Deleted"));

         return RedirectToAction("List");
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
         return RedirectToAction("Edit", new { id = user.Id });
      }
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("impersonate")]
   public virtual async Task<IActionResult> Impersonate(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowUserImpersonation))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
         return RedirectToAction("List");

      if (!user.IsActive)
      {
         _notificationService.WarningNotification(
             await _localizationService.GetResourceAsync("Admin.Users.Users.Impersonate.Inactive"));
         return RedirectToAction("Edit", user.Id);
      }

      //ensure that a non-admin user cannot impersonate as an administrator
      //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
      var currentUser = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsAdminAsync(currentUser) && await _userService.IsAdminAsync(user))
      {
         _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.NonAdminNotImpersonateAsAdminError"));
         return RedirectToAction("Edit", user.Id);
      }

      //activity log
      await _userActivityService.InsertActivityAsync("Impersonation.Started",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Started.PlatformOwner"), user.Email, user.Id), user);
      await _userActivityService.InsertActivityAsync(user, "Impersonation.Started",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Started.User"), currentUser.Email, currentUser.Id), currentUser);

      //ensure login is not required
      user.RequireReLogin = false;
      await _userService.UpdateUserAsync(user);
      await _genericAttributeService.SaveAttributeAsync<long?>(currentUser, AppUserDefaults.ImpersonatedUserIdAttribute, user.Id);

      return RedirectToAction("Index", "Home", new { area = string.Empty });
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("send-welcome-message")]
   public virtual async Task<IActionResult> SendWelcomeMessage(UserModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null)
         return RedirectToAction("List");

      await _workflowMessageService.SendUserWelcomeMessageAsync(user, (await _workContext.GetWorkingLanguageAsync()).Id);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.SendWelcomeMessage.Success"));

      return RedirectToAction("Edit", new { id = user.Id });
   }

   [HttpPost, ActionName("Edit")]
   [FormValueRequired("resend-activation-message")]
   public virtual async Task<IActionResult> ReSendActivationMessage(UserModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null)
         return RedirectToAction("List");

      //email validation message
      await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
      await _workflowMessageService.SendUserEmailValidationMessageAsync(user, (await _workContext.GetWorkingLanguageAsync()).Id);

      _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.ReSendActivationMessage.Success"));

      return RedirectToAction("Edit", new { id = user.Id });
   }

   public virtual async Task<IActionResult> SendEmail(UserModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null)
         return RedirectToAction("List");

      try
      {
         if (string.IsNullOrWhiteSpace(user.Email))
            throw new AppException("User email is empty");
         if (!CommonHelper.IsValidEmail(user.Email))
            throw new AppException("User email is not valid");
         if (string.IsNullOrWhiteSpace(model.SendEmail.Subject))
            throw new AppException("Email subject is empty");
         if (string.IsNullOrWhiteSpace(model.SendEmail.Body))
            throw new AppException("Email body is empty");

         var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId);
         if (emailAccount == null)
            emailAccount = (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
         if (emailAccount == null)
            throw new AppException("Email account can't be loaded");
         var email = new QueuedEmail
         {
            Priority = QueuedEmailPriority.High,
            EmailAccountId = emailAccount.Id,
            FromName = emailAccount.DisplayName,
            From = emailAccount.Email,
            ToName = await _userService.GetUserFullNameAsync(user),
            To = user.Email,
            Subject = model.SendEmail.Subject,
            Body = model.SendEmail.Body,
            CreatedOnUtc = DateTime.UtcNow,
            DontSendBeforeDateUtc = model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue ?
                 null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
         };
         await _queuedEmailService.InsertQueuedEmailAsync(email);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.SendEmail.Queued"));
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
      }

      return RedirectToAction("Edit", new { id = user.Id });
   }

   public virtual async Task<IActionResult> SendPm(UserModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.Id);
      if (user == null)
         return RedirectToAction("List");

      try
      {
         if (!_forumSettings.AllowPrivateMessages)
            throw new AppException("Private messages are disabled");
         if (await _userService.IsGuestAsync(user))
            throw new AppException("User should be registered");
         if (string.IsNullOrWhiteSpace(model.SendPm.Subject))
            throw new AppException(await _localizationService.GetResourceAsync("PrivateMessages.SubjectCannotBeEmpty"));
         if (string.IsNullOrWhiteSpace(model.SendPm.Message))
            throw new AppException(await _localizationService.GetResourceAsync("PrivateMessages.MessageCannotBeEmpty"));

         var privateMessage = new PrivateMessage
         {
            ToUserId = user.Id,
            FromUserId = user.Id,
            Subject = model.SendPm.Subject,
            Text = model.SendPm.Message,
            IsDeletedByAuthor = false,
            IsDeletedByRecipient = false,
            IsRead = false,
            CreatedOnUtc = DateTime.UtcNow
         };

         await _forumService.InsertPrivateMessageAsync(privateMessage);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.SendPM.Sent"));
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
      }

      return RedirectToAction("Edit", new { id = user.Id });
   }

   [SaveLastVisitedPage(true)]
   public virtual async Task<IActionResult> LoadUserStatistics(string period)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return Content(string.Empty);

      var result = new List<object>();

      var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
      var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();
      var searchUserRoleIds = new[] { (await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName)).Id };

      var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);

      // TODO refactor by single period query 
      switch (period)
      {
         case "year":
            //year statistics
            var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
            var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
            for (var i = 0; i <= 12; i++)
            {
               result.Add(new
               {
                  date = searchYearDateUser.Date.ToString("Y", culture),
                  value = (await _userService.GetAllUsersAsync(
                       //createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                       createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                       userRoleIds: searchUserRoleIds,
                       pageIndex: 0,
                       pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
               });

               searchYearDateUser = searchYearDateUser.AddMonths(1);
            }

            break;
         case "month":
            //month statistics
            var monthAgoDt = nowDt.AddDays(-30);
            var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
            for (var i = 0; i <= 30; i++)
            {
               result.Add(new
               {
                  date = searchMonthDateUser.Date.ToString("M", culture),
                  value = (await _userService.GetAllUsersAsync(
                       //createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                       createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                       userRoleIds: searchUserRoleIds,
                       pageIndex: 0,
                       pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
               });

               searchMonthDateUser = searchMonthDateUser.AddDays(1);
            }

            break;
         case "week":
         default:
            //week statistics
            var weekAgoDt = nowDt.AddDays(-7);
            var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
            for (var i = 0; i <= 7; i++)
            {
               result.Add(new
               {
                  date = searchWeekDateUser.Date.ToString("d dddd", culture),
                  value = (await _userService.GetAllUsersAsync(
                       //createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                       createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                       userRoleIds: searchUserRoleIds,
                       pageIndex: 0,
                       pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
               });

               searchWeekDateUser = searchWeekDateUser.AddDays(1);
            }

            break;
      }

      return Json(result);
   }


   #endregion

   #region Devices

   public virtual async Task<IActionResult> DevicesAddPopup(long unitId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceSearchModelAsync(new DeviceSearchModel(), true);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> DevicesAddPopupList(DeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _deviceModelFactory.PrepareDeviceListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   [FormValueRequired("save")]
   public virtual async Task<IActionResult> DevicesAddPopup(UserDevicesModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowAdminsDevices))
         return AccessDeniedView();

      if (Request.Query.TryGetValue("btnId", out var button))
      {
         var subject = await _workContext.GetCurrentUserAsync();
         var user = await _userService.GetUserByIdAsync(model.UserId);
         var devices = await _deviceService.GetDevicesByIdsAsync(model.SelectedDeviceIds);
         // except an existing owner
         devices = devices.Where(x => x.OwnerId != model.UserId).ToList();

         if (devices.Any())
         {
            // set new owner for pointed devices
            if (button.ToString().Equals("btnRefresUserOwnDevices"))
            {

               var prevOwners = await _userService.GetUsersByIdsAsync(devices.Select(x => x.OwnerId).ToArray());

               var activityRecords = new List<(Device, string)>();
               foreach (var device in devices)
                  activityRecords.Add((device, $"Device {device.SystemName} owner {prevOwners.FirstOrDefault(x => x.Id == device.OwnerId).Email ?? string.Empty} has been changed to new owner {user.Email} by {subject.Email}"));

               foreach (var device in devices)
                  device.OwnerId = model.UserId;

               await _deviceService.UpdateDeviceAsync(devices);

               // unmpap if map exists because user is owmer now
               foreach (var device in devices)
                  await _deviceService.UnmapDeviceFromUserAsync(device.Id, model.UserId);

               foreach (var record in activityRecords)
                  await _deviceActivityService.InsertActivityAsync(record.Item1, "Device.OwnerChanged", record.Item2);

            }
            // set new shared devices
            else if (button.ToString().Equals("btnRefresUserSharedDevices"))
            {
               var existingSharedDeviceIds = (await _deviceService.GetSharedDeviceByUserIdAsync(model.UserId)).Select(x => x.Id);
               var newSharedDeviceIds = model.SelectedDeviceIds.Where(x => !existingSharedDeviceIds.Contains(x));

               if (newSharedDeviceIds.Any())
               {
                  await _deviceService.MapDeviceToUserAsync(newSharedDeviceIds, model.UserId);

                  var newSharedDevices = await _deviceService.GetDevicesByIdsAsync(newSharedDeviceIds.ToList());

                  foreach (var device in newSharedDevices)
                     await _deviceActivityService.InsertActivityAsync(device, "Device.AddMap", $"Device {device.SystemName} has been mapped to user {user.Email} by {subject.Email}");
               }
            }
         }
      }

      ViewBag.RefreshPage = true;

      return View(new DeviceSearchModel());
   }

   public virtual async Task<IActionResult> OwnDevicesSelect(UserDeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return await AccessDeniedDataTablesJson();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(searchModel.UserId)
          ?? throw new ArgumentException("No user found with the specified id");

      var model = await _userModelFactory.PrepareUserOwnDeviceListModelAsync(searchModel, user);

      return Json(model);
   }

   public virtual async Task<IActionResult> SharedDevicesSelect(UserDeviceSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
         return await AccessDeniedDataTablesJson();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(searchModel.UserId)
          ?? throw new ArgumentException("No user found with the specified id");

      var model = await _userModelFactory.PrepareUserSharedDeviceListModelAsync(searchModel, user);

      return Json(model);
   }

   #endregion

   #region Addresses

   [HttpPost]
   public virtual async Task<IActionResult> AddressesSelect(UserAddressSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(searchModel.UserId)
          ?? throw new ArgumentException("No user found with the specified id");

      //prepare model
      var model = await _userModelFactory.PrepareUserAddressListModelAsync(searchModel, user);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> AddressDelete(long id, long userId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(userId)
          ?? throw new ArgumentException("No user found with the specified id", nameof(userId));

      //try to get an address with the specified id
      var address = await _userService.GetUserAddressAsync(user.Id, id);

      if (address == null)
         return Content("No address found with the specified id");

      await _userService.RemoveUserAddressAsync(user, address);
      await _userService.UpdateUserAsync(user);

      //now delete the address record
      await _addressService.DeleteAddressAsync(address);

      return new NullJsonResult();
   }

   public virtual async Task<IActionResult> AddressCreate(long userId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(userId);
      if (user == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _userModelFactory.PrepareUserAddressModelAsync(new UserAddressModel(), user, null);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> AddressCreate(UserAddressModel model, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.UserId);
      if (user == null)
         return RedirectToAction("List");

      //custom address attributes
      var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
      var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
      foreach (var error in customAttributeWarnings)
         ModelState.AddModelError(string.Empty, error);

      if (ModelState.IsValid)
      {
         var address = model.Address.ToEntity<Address>();
         address.CustomAttributes = customAttributes;
         address.CreatedOnUtc = DateTime.UtcNow;

         //some validation
         if (address.CountryId == 0)
            address.CountryId = null;
         if (address.StateProvinceId == 0)
            address.StateProvinceId = null;

         await _addressService.InsertAddressAsync(address);

         await _userService.InsertUserAddressAsync(user, address);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.Addresses.Added"));

         return RedirectToAction("AddressEdit", new { addressId = address.Id, userId = model.UserId });
      }

      //prepare model
      model = await _userModelFactory.PrepareUserAddressModelAsync(model, user, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> AddressEdit(long addressId, long userId)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(userId);
      if (user == null)
         return RedirectToAction("List");

      //try to get an address with the specified id
      var address = await _addressService.GetAddressByIdAsync(addressId);
      if (address == null)
         return RedirectToAction("Edit", new { id = user.Id });

      //prepare model
      var model = await _userModelFactory.PrepareUserAddressModelAsync(null, user, address);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> AddressEdit(UserAddressModel model, IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(model.UserId);
      if (user == null)
         return RedirectToAction("List");

      //try to get an address with the specified id
      var address = await _addressService.GetAddressByIdAsync(model.Address.Id);
      if (address == null)
         return RedirectToAction("Edit", new { id = user.Id });

      //custom address attributes
      var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
      var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
      foreach (var error in customAttributeWarnings)
         ModelState.AddModelError(string.Empty, error);

      if (ModelState.IsValid)
      {
         address = model.Address.ToEntity(address);
         address.CustomAttributes = customAttributes;
         await _addressService.UpdateAddressAsync(address);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.Addresses.Updated"));

         return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, userId = model.UserId });
      }

      //prepare model
      model = await _userModelFactory.PrepareUserAddressModelAsync(model, user, address, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   #endregion

   #region Activity log

   [HttpPost]
   public virtual async Task<IActionResult> ListActivityLog(UserActivityLogSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(searchModel.UserId)
          ?? throw new ArgumentException("No user found with the specified id");

      //prepare model
      var model = await _userModelFactory.PrepareUserActivityLogListModelAsync(searchModel, user);

      return Json(model);
   }

   #endregion

   #region GDPR

   public virtual async Task<IActionResult> GdprLog()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //prepare model
      var model = await _userModelFactory.PrepareGdprLogSearchModelAsync(new GdprLogSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> GdprLogList(GdprLogSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _userModelFactory.PrepareGdprLogListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> GdprDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
         return RedirectToAction("List");

      if (!_gdprSettings.GdprEnabled)
         return RedirectToAction("List");

      try
      {
         //prevent attempts to delete the user, if it is the last active administrator
         if (await _userService.IsAdminAsync(user) && !await SecondAdminAccountExistsAsync(user))
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.AdminAccountShouldExists.DeleteAdministrator"));
            return RedirectToAction("Edit", new { id = user.Id });
         }

         //ensure that the current user cannot delete "Administrators" if he's not an admin himself
         if (await _userService.IsAdminAsync(user) && !await _userService.IsAdminAsync(await _workContext.GetCurrentUserAsync()))
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.OnlyAdminCanDeleteAdmin"));
            return RedirectToAction("Edit", new { id = user.Id });
         }

         //delete
         await _gdprService.PermanentDeleteUserAsync(user);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteUser",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteUser"), user.Id), user);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.Users.Deleted"));

         return RedirectToAction("List");
      }
      catch (Exception exc)
      {
         _notificationService.ErrorNotification(exc.Message);
         return RedirectToAction("Edit", new { id = user.Id });
      }
   }

   public virtual async Task<IActionResult> GdprExport(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      //try to get a user with the specified id
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
         return RedirectToAction("List");

      try
      {
         //log
         //_gdprService.InsertLog(user, 0, GdprRequestType.ExportData, await _localizationService.GetResource("Gdpr.Exported"));
         //export
         //export
         var bytes = await _exportManager.ExportUserGdprInfoToXlsxAsync(user);

         return File(bytes, MimeTypes.TextXlsx, $"userdata-{user.Id}.xlsx");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("Edit", new { id = user.Id });
      }
   }
   #endregion

   #region Export / Import

   [HttpPost, ActionName("ExportExcel")]
   [FormValueRequired("exportexcel-all")]
   public virtual async Task<IActionResult> ExportExcelAll(UserSearchModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      var users = await _userService.GetAllUsersAsync(userRoleIds: model.SelectedUserRoleIds.ToArray(),
          email: model.SearchEmail,
          username: model.SearchUsername,
          firstName: model.SearchFirstName,
          lastName: model.SearchLastName,
          dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
          monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
          company: model.SearchCompany,
          phone: model.SearchPhone,
          zipPostalCode: model.SearchZipPostalCode);

      try
      {
         var bytes = await _exportManager.ExportUsersToXlsxAsync(users);
         return File(bytes, MimeTypes.TextXlsx, "users.xlsx");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   [HttpPost]
   public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      var users = new List<User>();
      if (selectedIds != null)
      {
         var ids = selectedIds
             .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(x => Convert.ToInt64(x))
             .ToArray();
         users.AddRange(await _userService.GetUsersByIdsAsync(ids));
      }

      try
      {
         var bytes = await _exportManager.ExportUsersToXlsxAsync(users);
         return File(bytes, MimeTypes.TextXlsx, "users.xlsx");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   [HttpPost, ActionName("ExportXML")]
   [FormValueRequired("exportxml-all")]
   public virtual async Task<IActionResult> ExportXmlAll(UserSearchModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      var users = await _userService.GetAllUsersAsync(userRoleIds: model.SelectedUserRoleIds.ToArray(),
          email: model.SearchEmail,
          username: model.SearchUsername,
          firstName: model.SearchFirstName,
          lastName: model.SearchLastName,
          dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
          monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
          company: model.SearchCompany,
          phone: model.SearchPhone,
          zipPostalCode: model.SearchZipPostalCode);

      try
      {
         var xml = await _exportManager.ExportUsersToXmlAsync(users);
         return File(Encoding.UTF8.GetBytes(xml), "application/xml", "users.xml");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   [HttpPost]
   public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return AccessDeniedView();

      var users = new List<User>();
      if (selectedIds != null)
      {
         var ids = selectedIds
             .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(x => Convert.ToInt64(x))
             .ToArray();
         users.AddRange(await _userService.GetUsersByIdsAsync(ids));
      }

      try
      {
         var xml = await _exportManager.ExportUsersToXmlAsync(users);
         return File(Encoding.UTF8.GetBytes(xml), "application/xml", "users.xml");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("List");
      }
   }

   #endregion
}