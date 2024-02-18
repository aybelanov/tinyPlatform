using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Services.Clients.Devices;
using Hub.Services.Common;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Services.Users;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Represents a device registration service interface implementation
/// </summary>
public class DeviceRegistrationService : IDeviceRegistrationService
{
   #region Fields

   private readonly DeviceSettings _deviceSettings;
   private readonly UserSettings _userSettings;
   private readonly LocalizationSettings _localizationSettings;
   private readonly IHubDeviceService _hubDeviceService;
   private readonly IDeviceService _deviceService;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IUserService _userService;
   private readonly IEncryptionService _encryptionService;
   private readonly IPermissionService _permissionService;
   private readonly IEventPublisher _eventPublisher;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly ILocalizationService _localizationService;
   private readonly IWorkContext _workContext;
   private readonly IWorkflowMessageService _workflowMessageService;

   #endregion

   #region Ctor

   /// <summary> 
   /// IoC Ctor
   /// </summary>
   public DeviceRegistrationService(DeviceSettings deviceSettings,
       UserSettings userSettings,
       IUserService userService,
       IHubDeviceService hubDeviceService,
       IDeviceService clientDeviceService,
       IDeviceActivityService deviceActivityService,
       IEncryptionService encryptionService,
       IPermissionService permissionService,
       IEventPublisher eventPublisher,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       LocalizationSettings localizationSettings,
       IWorkContext workContext,
       IWorkflowMessageService workflowMessageService)
   {
      _deviceSettings = deviceSettings;
      _userSettings = userSettings;
      _userService = userService;
      _hubDeviceService = hubDeviceService;
      _deviceService = clientDeviceService;
      _deviceActivityService = deviceActivityService;
      _encryptionService = encryptionService;
      _permissionService = permissionService;
      _eventPublisher = eventPublisher;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _localizationSettings = localizationSettings;
      _workContext = workContext;
      _workflowMessageService = workflowMessageService;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Checks whether the entered password matches with a saved one
   /// </summary>
   /// <param name="deviceCredential">Device password</param>
   /// <param name="enteredPassword">The entered password</param>
   /// <returns>True if passwords match; otherwise false</returns>
   protected virtual bool PasswordsMatch(DeviceCredential deviceCredential, string enteredPassword)
   {
      if (deviceCredential == null || string.IsNullOrEmpty(enteredPassword))
         return false;

      var savedPassword = string.Empty;
      switch (deviceCredential.PasswordFormat)
      {
         case PasswordFormat.Clear:
            savedPassword = enteredPassword;
            break;
         case PasswordFormat.Encrypted:
            savedPassword = _encryptionService.EncryptText(enteredPassword);
            break;
         case PasswordFormat.Hashed:
            savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, deviceCredential.PasswordSalt, _deviceSettings.HashedPasswordFormat);
            break;
      }

      if (deviceCredential.Password == null)
         return false;

      return deviceCredential.Password.Equals(savedPassword);
   }

   #endregion

   #region Methods

   /// <summary>
   /// validates device systemname
   /// </summary>
   /// <param name="systemName">Validating systemname</param>
   /// <returns>Validating result</returns>
   public virtual async Task<DeviceRegistrationResult> ValidateSystemNameAsync(string systemName)
   {
      var result = new DeviceRegistrationResult();

      // check register info
      if (string.IsNullOrEmpty(systemName))
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.SystemNameIsNotProvided"));

      else if (_deviceSettings.SystemNameMinLength > 0 && systemName.Length < _deviceSettings.SystemNameMinLength)
         result.AddError(string.Format(await _localizationService.GetResourceAsync("Account.Register.Errors.SystemNameIsShort"), _deviceSettings.SystemNameMinLength));

      else if (_deviceSettings.SystemNameMaxLength > 0 && systemName.Length > _deviceSettings.SystemNameMaxLength)
         result.AddError(string.Format(await _localizationService.GetResourceAsync("Account.Register.Errors.SystemNameIsLong"), _deviceSettings.SystemNameMaxLength));

      //validate unique system name
      else if (await _hubDeviceService.GetDeviceBySystemNameAsync(systemName) != null)
         result.AddError(string.Format(await _localizationService.GetResourceAsync("Account.Register.Errors.SystemNameAlreadyExists"), systemName));

      return result; 
   }

   /// <summary>
   /// Validates device password
   /// </summary>
   /// <param name="newPassword">Validated password</param>
   /// <returns>Validating result</returns>
   public virtual async Task<DeviceRegistrationResult> ValidatePasswordFormatAsync(string newPassword)
   {
      var result = new DeviceRegistrationResult();

      var regExp = "^";
      //Passwords must be at least X characters and contain the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*-)
      regExp += _deviceSettings.PasswordRequireUppercase ? "(?=.*?[A-Z])" : "";
      regExp += _deviceSettings.PasswordRequireLowercase ? "(?=.*?[a-z])" : "";
      regExp += _deviceSettings.PasswordRequireDigit ? "(?=.*?[0-9])" : "";
      regExp += _deviceSettings.PasswordRequireNonAlphanumeric ? "(?=.*?[#?!@$%^&*-])" : "";
      regExp += $".{{{_deviceSettings.PasswordMinLength},}}$";

      var validateFormat = Regex.IsMatch(newPassword, regExp);
      if (!validateFormat)
      {
         result.AddError(string.Format(await _localizationService.GetResourceAsync("Validation.Password.Rule"),
          string.Format(await _localizationService.GetResourceAsync("Validation.Password.LengthValidation"), _deviceSettings.PasswordMinLength),
          _deviceSettings.PasswordRequireUppercase ? await _localizationService.GetResourceAsync("Validation.Password.RequireUppercase") : "",
          _deviceSettings.PasswordRequireLowercase ? await _localizationService.GetResourceAsync("Validation.Password.RequireLowercase") : "",
          _deviceSettings.PasswordRequireDigit ? await _localizationService.GetResourceAsync("Validation.Password.RequireDigit") : "",
          _deviceSettings.PasswordRequireNonAlphanumeric ? await _localizationService.GetResourceAsync("Validation.Password.RequireNonAlphanumeric") : ""));
      }
      
      return result;
   }

   /// <summary>
   /// Validate device
   /// </summary>
   /// <param name="systemName">Device unique identifier</param>
   /// <param name="password">Password</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<DeviceLoginResults> ValidateDeviceCredentialsAsync(string systemName, string password)
   {
      var device = await _hubDeviceService.GetDeviceBySystemNameAsync(systemName);

      // check device
      if (device == null)
         return DeviceLoginResults.DeviceNotExist;
      if (device.IsDeleted)
         return DeviceLoginResults.DeviceDeleted;
      if (!device.IsActive)
         return DeviceLoginResults.DeviceNotActive;
      //check whether a device is locked out
      if (device.CannotLoginUntilDateUtc.HasValue && device.CannotLoginUntilDateUtc.Value > DateTime.UtcNow)
         return DeviceLoginResults.DeviceLockedOut;

      // check user-owner
      var owner = await _userService.GetUserByIdAsync(device.OwnerId);
      if (owner == null)
         return DeviceLoginResults.UserNotExist;
      if (owner.IsDeleted)
         return DeviceLoginResults.UserDeleted;
      if (!owner.IsActive && _deviceSettings.BlockDevicesIfOwnerNotActive)
         return DeviceLoginResults.UserNotActive;
      //only registered user's device can login
      if (!await _userService.IsRegisteredAsync(owner))
         return DeviceLoginResults.UserNotRegistered;
      //check whether a user is locked out
      if (owner.CannotLoginUntilDateUtc.HasValue && owner.CannotLoginUntilDateUtc.Value > DateTime.UtcNow && _deviceSettings.BlockDevicesIfOwnerNotActive)
         return DeviceLoginResults.UserLockedOut;

      if (!PasswordsMatch(await _hubDeviceService.GetCurrentDeviceCredentialAsync(device.Id), password))
      {
         //wrong password
         device.FailedLoginAttempts++;
         if (_deviceSettings.FailedPasswordAllowedAttempts > 0 &&
             device.FailedLoginAttempts >= _deviceSettings.FailedPasswordAllowedAttempts)
         {
            //lock out
            device.CannotLoginUntilDateUtc = DateTime.UtcNow.AddMinutes(_deviceSettings.FailedPasswordLockoutMinutes);
            //reset the counter
            device.FailedLoginAttempts = 0;
         }

         await _hubDeviceService.UpdateDeviceAsync(device);

         return DeviceLoginResults.WrongPassword;
      }

      //update login details
      device.FailedLoginAttempts = 0;
      device.CannotLoginUntilDateUtc = null;
      await _hubDeviceService.UpdateDeviceAsync(device);

      await _deviceActivityService.InsertActivityAsync(device, "Device.Login", $"Device {device.SystemName} has logged in.");
      return DeviceLoginResults.Successful;
   }

   /// <summary>
   /// Register device
   /// </summary>
   /// <param name="request">Request</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<DeviceRegistrationResult> RegisterDeviceAsync(DeviceRegistrationRequest request)
   {
      if (request == null)
         throw new ArgumentNullException(nameof(request));

      if (request.Device == null)
         throw new ArgumentException("Can not load current device");

      var result = new DeviceRegistrationResult();

      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices) 
         && !await _permissionService.AuthorizeAsync(StandardPermissionProvider.AllowManageDevices))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.CannotRegisterDevice"));
         return result;
      }
      // check registration subject
      var user = await _workContext.GetCurrentUserAsync();
      if (user == null || user.IsSearchEngineAccount() || user.IsBackgroundTaskAccount())
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UserNotPointed"));
         return result;
      }
      if (!await _userService.IsRegisteredAsync(user))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UserNotRegister"));
         return result;
      }
      if (!await _userService.IsOwnerAsync(user) && !await _userService.IsAdminAsync(user))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.CannotRegisterDevice"));
         return result;
      }
      if (!user.IsActive && _deviceSettings.BlockDevicesIfOwnerNotActive)
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UserNotActive"));
         return result;
      }
      if (user.IsDeleted)
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UserIsDeleted"));
         return result;
      }

      // check owner
      var owner = await _userService.GetUserByIdAsync(request.Device.OwnerId);
      if (owner is null || !await _userService.IsRegisteredAsync(owner))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.OwnerIsNotRegistered"));
         return result;

      }

      if (user.DeviceCountLimit > 0 && ((await _hubDeviceService.GetAllPagedDevicesAsync(ownerIds: [owner.Id])).Count >= user.DeviceCountLimit))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.LimitIsReached"));
         return result;
      }

      var systenNameValidateResult = await ValidateSystemNameAsync(request.SystemName);
      if (!systenNameValidateResult.Success)
      {
         systenNameValidateResult.Errors.ToList().ForEach(result.AddError);
         return result; 
      }

      var passwordValidateResult = await ValidatePasswordFormatAsync(request.Password);
      if(!passwordValidateResult.Success)
      {
         passwordValidateResult.Errors.ToList().ForEach(result.AddError);
         return result;
      }

      //if (!CheckPasswordFormat(request.Password))
      //{
      //   result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordFormatIsWrong"));
      //   return result;
      //}

      //at this point request is valid
      request.Device.IsActive = request.IsApproved;
      request.Device.Guid = Guid.NewGuid();
      await _deviceService.InsertDeviceAsync(request.Device);

      var deviceCredential = new DeviceCredential
      {
         DeviceId = request.Device.Id,
         PasswordFormat = request.PasswordFormat,
         CreatedOnUtc = DateTime.UtcNow
      };
      switch (request.PasswordFormat)
      {
         case PasswordFormat.Clear:
            deviceCredential.Password = request.Password;
            break;
         case PasswordFormat.Encrypted:
            deviceCredential.Password = _encryptionService.EncryptText(request.Password);
            break;
         case PasswordFormat.Hashed:
            var saltKey = _encryptionService.CreateSaltKey(AppUserServicesDefaults.PasswordSaltKeySize);
            deviceCredential.PasswordSalt = saltKey;
            deviceCredential.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _deviceSettings.HashedPasswordFormat);
            break;
      }

      await _hubDeviceService.InsertDeviceCredentialsAsync(deviceCredential);

      //notifications
      if (_deviceSettings.NotifyNewDeviceRegistration)
         await _workflowMessageService.SendDeviceRegisteredNotificationMessageAsync(request.Device,
             _localizationSettings.DefaultAdminLanguageId);

      //raise event       
      await _eventPublisher.PublishAsync(new DeviceRegisteredEvent(request.Device));

      return result;
   }

   /// <summary>
   /// Change password
   /// </summary>
   /// <param name="request">Request</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
   {
      if (request == null)
         throw new ArgumentNullException(nameof(request));

      var result = new ChangePasswordResult();
      if (string.IsNullOrWhiteSpace(request.EmailOrSystemName))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.SystemNameIsNotProvided"));
         return result;
      }

      if (string.IsNullOrWhiteSpace(request.NewPassword))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordIsNotProvided"));
         return result;
      }

      var device = await _hubDeviceService.GetDeviceBySystemNameAsync(request.EmailOrSystemName);
      if (device == null)
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.SystemNameNotFound"));
         return result;
      }

      //request isn't valid
      if (request.ValidateRequest && !PasswordsMatch(await _hubDeviceService.GetCurrentDeviceCredentialAsync(device.Id), request.OldPassword))
      {
         result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
         return result;
      }

      var passwordValidateResult = await ValidatePasswordFormatAsync(request.NewPassword);
      if (!passwordValidateResult.Success)
      {
         passwordValidateResult.Errors.ToList().ForEach(result.AddError);
         return result;
      }

      //check for duplicates
      if (_deviceSettings.UnduplicatedPasswordNumber > 0)
      {
         var previousPasswords = await _hubDeviceService.GetDeviceCredentialsAsync(device.Id, credentialsToReturn: _deviceSettings.UnduplicatedPasswordNumber);
         
         var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
         if (newPasswordMatchesWithPrevious)
         {
            result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
            return result;
         }
      }

      //at this point request is valid
      var deviceCredentials = new DeviceCredential
      {
         DeviceId = device.Id,
         PasswordFormat = request.NewPasswordFormat,
         CreatedOnUtc = DateTime.UtcNow,
      };

      switch (request.NewPasswordFormat)
      {
         case PasswordFormat.Clear:
            deviceCredentials.Password = request.NewPassword;
            break;
         case PasswordFormat.Encrypted:
            deviceCredentials.Password = _encryptionService.EncryptText(request.NewPassword);
            break;
         case PasswordFormat.Hashed:
            var saltKey = _encryptionService.CreateSaltKey(AppUserServicesDefaults.PasswordSaltKeySize);
            deviceCredentials.PasswordSalt = saltKey;
            deviceCredentials.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey,
                request.HashedPasswordFormat ?? _deviceSettings.HashedPasswordFormat);
            break;
      }

      await _hubDeviceService.InsertDeviceCredentialsAsync(deviceCredentials);

      //publish event
      await _eventPublisher.PublishAsync(new DeviceCredentialsChangedEvent(deviceCredentials));

      return result;
   }

   #endregion
}
