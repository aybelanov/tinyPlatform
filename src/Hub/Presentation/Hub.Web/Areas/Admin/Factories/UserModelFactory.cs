using Hub.Core.Domain.Common;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Gdpr;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Authentication.External;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Directory;
using Hub.Services.Gdpr;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Messages;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Areas.Admin.Models.Gdpr;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Clients.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the user model factory implementation
/// </summary>
public partial class UserModelFactory : IUserModelFactory
{
   #region Fields

   private readonly AddressSettings _addressSettings;
   private readonly UserSettings _userSettings;
   private readonly DateTimeSettings _dateTimeSettings;
   private readonly GdprSettings _gdprSettings;
   private readonly ForumSettings _forumSettings;
   private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
   private readonly IAddressAttributeFormatter _addressAttributeFormatter;
   private readonly IAddressModelFactory _addressModelFactory;
   private readonly IAuthenticationPluginManager _authenticationPluginManager;
   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly ICountryService _countryService;
   private readonly IUserActivityService _userActivityService;
   private readonly IUserAttributeParser _userAttributeParser;
   private readonly IUserAttributeService _userAttributeService;
   private readonly IUserService _userService;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IHubSensorRecordService _sensorDataService;
   private readonly Hub.Services.Devices.IHubSensorService _sensorService;
   private readonly Hub.Services.Devices.IHubDeviceService _deviceService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IExternalAuthenticationService _externalAuthenticationService;
   private readonly IGdprService _gdprService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IGeoLookupService _geoLookupService;
   private readonly ILocalizationService _localizationService;
   private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
   private readonly IPictureService _pictureService;
   private readonly IStateProvinceService _stateProvinceService;
   private readonly MediaSettings _mediaSettings;
   private readonly ICommunicator _communicator;


   #endregion

   #region Ctor

   public UserModelFactory(
      AddressSettings addressSettings,
      UserSettings userSettings,
      DateTimeSettings dateTimeSettings,
      GdprSettings gdprSettings,
      ForumSettings forumSettings,
      IAclSupportedModelFactory aclSupportedModelFactory,
      IAddressAttributeFormatter addressAttributeFormatter,
      IAddressModelFactory addressModelFactory,
      IAuthenticationPluginManager authenticationPluginManager,
      IBaseAdminModelFactory baseAdminModelFactory,
      ICountryService countryService,
      IUserActivityService userActivityService,
      IDeviceActivityService deviceActivityService,
      IUserAttributeParser userAttributeParser,
      IUserAttributeService userAttributeService,
      IUserService userService,
      Hub.Services.Devices.IHubDeviceService deviceService,
      Hub.Services.Devices.IHubSensorService sensorService,
      IHubSensorRecordService sensorDataService,
      IDateTimeHelper dateTimeHelper,
      IExternalAuthenticationService externalAuthenticationService,
      IGdprService gdprService,
      IGenericAttributeService genericAttributeService,
      IGeoLookupService geoLookupService,
      ILocalizationService localizationService,
      INewsLetterSubscriptionService newsLetterSubscriptionService,
      IPictureService pictureService,
      IStateProvinceService stateProvinceService,
      MediaSettings mediaSettings,
      ICommunicator communicator)
   {
      _addressSettings = addressSettings;
      _userSettings = userSettings;
      _dateTimeSettings = dateTimeSettings;
      _gdprSettings = gdprSettings;
      _forumSettings = forumSettings;
      _aclSupportedModelFactory = aclSupportedModelFactory;
      _addressAttributeFormatter = addressAttributeFormatter;
      _addressModelFactory = addressModelFactory;
      _authenticationPluginManager = authenticationPluginManager;
      _baseAdminModelFactory = baseAdminModelFactory;
      _countryService = countryService;
      _userActivityService = userActivityService;
      _deviceActivityService = deviceActivityService;
      _userAttributeParser = userAttributeParser;
      _userAttributeService = userAttributeService;
      _userService = userService;
      _deviceService = deviceService;
      _sensorDataService = sensorDataService;
      _sensorService = sensorService;
      _dateTimeHelper = dateTimeHelper;
      _externalAuthenticationService = externalAuthenticationService;
      _gdprService = gdprService;
      _genericAttributeService = genericAttributeService;
      _geoLookupService = geoLookupService;
      _localizationService = localizationService;
      _newsLetterSubscriptionService = newsLetterSubscriptionService;
      _pictureService = pictureService;
      _stateProvinceService = stateProvinceService;
      _mediaSettings = mediaSettings;
      _communicator = communicator;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare user associated external authorization models
   /// </summary>
   /// <param name="models">List of user associated external authorization models</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareAssociatedExternalAuthModelsAsync(IList<UserAssociatedExternalAuthModel> models, User user)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      foreach (var record in await _externalAuthenticationService.GetUserExternalAuthenticationRecordsAsync(user))
      {
         var method = await _authenticationPluginManager.LoadPluginBySystemNameAsync(record.ProviderSystemName);
         if (method == null)
            continue;

         models.Add(new UserAssociatedExternalAuthModel
         {
            Id = record.Id,
            Email = record.Email,
            ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                 ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
            AuthMethodName = method.PluginDescriptor.FriendlyName
         });
      }
   }

   /// <summary>
   /// Prepare user attribute models
   /// </summary>
   /// <param name="models">List of user attribute models</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareUserAttributeModelsAsync(IList<UserModel.UserAttributeModel> models, User user)
   {
      if (models == null)
         throw new ArgumentNullException(nameof(models));

      //get available user attributes
      var userAttributes = await _userAttributeService.GetAllUserAttributesAsync();
      foreach (var attribute in userAttributes)
      {
         var attributeModel = new UserModel.UserAttributeModel
         {
            Id = attribute.Id,
            Name = attribute.Name,
            IsRequired = attribute.IsRequired,
            AttributeControlType = attribute.AttributeControlType
         };

         if (attribute.ShouldHaveValues())
         {
            //values
            var attributeValues = await _userAttributeService.GetUserAttributeValuesAsync(attribute.Id);
            foreach (var attributeValue in attributeValues)
            {
               var attributeValueModel = new UserModel.UserAttributeValueModel
               {
                  Id = attributeValue.Id,
                  Name = attributeValue.Name,
                  IsPreSelected = attributeValue.IsPreSelected
               };
               attributeModel.Values.Add(attributeValueModel);
            }
         }

         //set already selected attributes
         if (user != null)
         {
            var selectedUserAttributes = await _genericAttributeService
                .GetAttributeAsync<string>(user, AppUserDefaults.CustomUserAttributes);
            switch (attribute.AttributeControlType)
            {
               case AttributeControlType.DropdownList:
               case AttributeControlType.RadioList:
               case AttributeControlType.Checkboxes:
                  {
                     if (!string.IsNullOrEmpty(selectedUserAttributes))
                     {
                        //clear default selection
                        foreach (var item in attributeModel.Values)
                           item.IsPreSelected = false;

                        //select new values
                        var selectedValues = await _userAttributeParser.ParseUserAttributeValuesAsync(selectedUserAttributes);
                        foreach (var attributeValue in selectedValues)
                           foreach (var item in attributeModel.Values)
                              if (attributeValue.Id == item.Id)
                                 item.IsPreSelected = true;
                     }
                  }
                  break;
               case AttributeControlType.ReadonlyCheckboxes:
                  {
                     //do nothing
                     //values are already pre-set
                  }
                  break;
               case AttributeControlType.TextBox:
               case AttributeControlType.MultilineTextbox:
                  {
                     if (!string.IsNullOrEmpty(selectedUserAttributes))
                     {
                        var enteredText = _userAttributeParser.ParseValues(selectedUserAttributes, attribute.Id);
                        if (enteredText.Any())
                           attributeModel.DefaultValue = enteredText[0];
                     }
                  }
                  break;
               case AttributeControlType.Datepicker:
               case AttributeControlType.ColorSquares:
               case AttributeControlType.ImageSquares:
               case AttributeControlType.FileUpload:
               default:
                  //not supported attribute control types
                  break;
            }
         }

         models.Add(attributeModel);
      }
   }

   /// <summary>
   /// Prepare HTML string address
   /// </summary>
   /// <param name="model">Address model</param>
   /// <param name="address">Address</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task PrepareModelAddressHtmlAsync(AddressModel model, Address address)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      var addressHtmlSb = new StringBuilder("<div>");

      if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(model.Company))
         addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Company));

      if (_addressSettings.StreetAddressEnabled && !string.IsNullOrEmpty(model.Address1))
         addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address1));

      if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(model.Address2))
         addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address2));

      if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(model.City))
         addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.City));

      if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(model.County))
         addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.County));

      if (_addressSettings.StateProvinceEnabled && !string.IsNullOrEmpty(model.StateProvinceName))
         addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.StateProvinceName));

      if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(model.ZipPostalCode))
         addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.ZipPostalCode));

      if (_addressSettings.CountryEnabled && !string.IsNullOrEmpty(model.CountryName))
         addressHtmlSb.AppendFormat("{0}", WebUtility.HtmlEncode(model.CountryName));

      var customAttributesFormatted = await _addressAttributeFormatter.FormatAttributesAsync(address?.CustomAttributes);
      if (!string.IsNullOrEmpty(customAttributesFormatted))
         //already encoded
         addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);

      addressHtmlSb.Append("</div>");

      model.AddressHtml = addressHtmlSb.ToString();
   }

   /// <summary>
   /// Prepare user address search model
   /// </summary>
   /// <param name="searchModel">User address search model</param>
   /// <param name="user">User</param>
   /// <returns>User address search model</returns>
   protected virtual UserDeviceSearchModel PrepareUserDeviceSearchModel(UserDeviceSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      searchModel.UserId = user.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare user address search model
   /// </summary>
   /// <param name="searchModel">User address search model</param>
   /// <param name="user">User</param>
   /// <returns>User address search model</returns>
   protected virtual UserAddressSearchModel PrepareUserAddressSearchModel(UserAddressSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      searchModel.UserId = user.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare user activity log search model
   /// </summary>
   /// <param name="searchModel">User activity log search model</param>
   /// <param name="user">User</param>
   /// <returns>User activity log search model</returns>
   protected virtual UserActivityLogSearchModel PrepareUserActivityLogSearchModel(UserActivityLogSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      searchModel.UserId = user.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare user back in stock subscriptions search model
   /// </summary>
   /// <param name="searchModel">User back in stock subscriptions search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user back in stock subscriptions search model
   /// </returns>
   protected virtual async Task<UserAssociatedExternalAuthRecordsSearchModel> PrepareUserAssociatedExternalAuthRecordsSearchModelAsync(
       UserAssociatedExternalAuthRecordsSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      searchModel.UserId = user.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();
      //prepare external authentication records
      await PrepareAssociatedExternalAuthModelsAsync(searchModel.AssociatedExternalAuthRecords, user);

      return searchModel;
   }

   protected virtual async Task<UserStatisticsModel> PrepareUserStatisticsModel(User user)
   {
      var model = new UserStatisticsModel();
      if (user == null)
         return model;

      var now = DateTime.UtcNow;
      var onlineUserIds = await _communicator.GetOnlineUserIdsAsync();

      // online status
      if (onlineUserIds.Contains(user.Id))
      {
         model.Status = "online";
         model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Online");
      }
      else
      {
         if (user.LastActivityUtc >= now.AddMinutes(-_userSettings.BeenRecentlyMinutes))
         {
            model.Status = "beenrecently";
            model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.BeenRecently");
         }
         else if (user.LastActivityUtc < now.AddMinutes(-_userSettings.BeenRecentlyMinutes))
         {
            model.Status = "offline";
            model.StatusValue = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Offline");
         }
      }

      model.NumberOfSensors = (await _sensorService.GetAllPagedSensorsAsync(
         ownerIds: new[] { user.Id }, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

      model.NumberOfDataRecords = (await _sensorDataService.GetAllPagedSensorDataAsync(
         userIds: new[] { user.Id }, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

      var ownDevices = await _deviceService.GetAllPagedDevicesAsync(ownerIds: [user.Id], pageIndex: 0, pageSize: int.MaxValue);

      model.NumberOfTotalOwnDevices = ownDevices.TotalCount;

      var onlineDeviceIds = await _communicator.GetOnlineDeviceIdsAsync();

      model.NumberOfOnlineOwnDevices = onlineDeviceIds.Intersect(ownDevices.Select(x => x.Id)).Count();

      return model;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare user search model
   /// </summary>
   /// <param name="searchModel">User search model</param>
   /// <param name="popup">For popup tables</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user search model
   /// </returns>
   public virtual async Task<UserSearchModel> PrepareUserSearchModelAsync(UserSearchModel searchModel, bool popup = false)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      searchModel.UsernamesEnabled = _userSettings.UsernamesEnabled;
      searchModel.AvatarEnabled = _userSettings.AllowUsersToUploadAvatars;
      searchModel.FirstNameEnabled = _userSettings.FirstNameEnabled;
      searchModel.LastNameEnabled = _userSettings.LastNameEnabled;
      searchModel.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
      searchModel.CompanyEnabled = _userSettings.CompanyEnabled;
      searchModel.PhoneEnabled = _userSettings.PhoneEnabled;
      searchModel.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;

      //search registered users by default
      var registeredRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName);
      if (registeredRole != null)
         searchModel.SelectedUserRoleIds.Add(registeredRole.Id);

      //prepare available user roles
      await _aclSupportedModelFactory.PrepareModelUserRolesAsync(searchModel);

      //prepare page parameters
      if (!popup)
         searchModel.SetGridPageSize();
      else
         searchModel.SetPopupGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged user list model
   /// </summary>
   /// <param name="searchModel">User search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user list model
   /// </returns>
   public virtual async Task<UserListModel> PrepareUserListModelAsync(UserSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get parameters to filter users
      _ = int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
      _ = int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);

      //get users
      var users = await _userService.GetAllUsersAsync(userRoleIds: searchModel.SelectedUserRoleIds.ToArray(),
          email: searchModel.SearchEmail,
          username: searchModel.SearchUsername,
          firstName: searchModel.SearchFirstName,
          lastName: searchModel.SearchLastName,
          dayOfBirth: dayOfBirth,
          monthOfBirth: monthOfBirth,
          company: searchModel.SearchCompany,
          phone: searchModel.SearchPhone,
          zipPostalCode: searchModel.SearchZipPostalCode,
          ipAddress: searchModel.SearchIpAddress,
          pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //prepare list model
      var model = await new UserListModel().PrepareToGridAsync(searchModel, users, () =>
      {
         return users.SelectAwait(async user =>
            {
               //fill in model values from the entity
               var userModel = user.ToModel<UserModel>();

               //convert dates to the user time
               userModel.Email = await _userService.IsRegisteredAsync(user)
                      ? user.Email
                      : await _localizationService.GetResourceAsync("Admin.Users.Guest");
               userModel.FullName = await _userService.GetUserFullNameAsync(user);
               userModel.Company = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CompanyAttribute);
               userModel.Phone = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.PhoneAttribute);
               userModel.ZipPostalCode = await _genericAttributeService
                      .GetAttributeAsync<string>(user, AppUserDefaults.ZipPostalCodeAttribute);

               userModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(user.CreatedOnUtc, DateTimeKind.Utc);
               userModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(user.LastActivityUtc, DateTimeKind.Utc);

               //fill in additional values (not existing in the entity)
               userModel.UserRoleNames = string.Join(", ",
                      (await _userService.GetUserRolesAsync(user)).Select(role => role.Name));
               if (_userSettings.AllowUsersToUploadAvatars)
               {
                  //var avatarPictureId = await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.AvatarPictureIdAttribute);
                  userModel.AvatarUrl = await _pictureService
                         .GetPictureUrlAsync(user.AvatarPictureId, _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
               }

               return userModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare user model
   /// </summary>
   /// <param name="model">User model</param>
   /// <param name="user">User</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user model
   /// </returns>
   public virtual async Task<UserModel> PrepareUserModelAsync(UserModel model, User user, bool excludeProperties = false)
   {
      if (user != null)
      {
         //fill in model values from the entity
         model ??= new UserModel();

         model.Id = user.Id;
         model.AllowSendingOfPrivateMessage = await _userService.IsRegisteredAsync(user) &&
             _forumSettings.AllowPrivateMessages;
         model.AllowSendingOfWelcomeMessage = await _userService.IsRegisteredAsync(user) &&
             _userSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
         model.AllowReSendingOfActivationMessage = await _userService.IsRegisteredAsync(user) && !user.IsActive &&
             _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
         model.GdprEnabled = _gdprSettings.GdprEnabled;

         model.MultiFactorAuthenticationProvider = await _genericAttributeService
             .GetAttributeAsync<string>(user, AppUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);

         //whether to fill in some of properties
         if (!excludeProperties)
         {
            model.Email = user.Email;
            model.Username = user.Username;
            model.AdminComment = user.AdminComment;
            model.IsActive = user.IsActive;
            model.FirstName = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FirstNameAttribute);
            model.LastName = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastNameAttribute);
            model.Gender = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.GenderAttribute);
            model.DateOfBirth = await _genericAttributeService.GetAttributeAsync<DateTime?>(user, AppUserDefaults.DateOfBirthAttribute);
            model.Company = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CompanyAttribute);
            model.StreetAddress = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.StreetAddressAttribute);
            model.StreetAddress2 = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.StreetAddress2Attribute);
            model.ZipPostalCode = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.ZipPostalCodeAttribute);
            model.City = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CityAttribute);
            model.County = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CountyAttribute);
            model.CountryId = await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.CountryIdAttribute);
            model.StateProvinceId = await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.StateProvinceIdAttribute);
            model.Phone = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.PhoneAttribute);
            model.Fax = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FaxAttribute);
            model.TimeZoneId = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.TimeZoneIdAttribute);
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(user.CreatedOnUtc, DateTimeKind.Utc);
            model.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(user.LastActivityUtc, DateTimeKind.Utc);
            model.LastIpAddress = _userSettings.StoreIpAddresses ? user.LastIpAddress : string.Empty;
            model.LastVisitedPage = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastVisitedPageAttribute);
            model.SelectedUserRoleIds = (await _userService.GetUserRoleIdsAsync(user)).ToList();

            //prepare model newsletter subscriptions
            if (!string.IsNullOrEmpty(user.Email))
            {
               var subscriptoion = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAsync(user.Email);
               model.NewsletterSubscribed = subscriptoion == null ? false : true;
            }
         }

         PrepareUserDeviceSearchModel(model.UserDeviceSearchModel, user);
         PrepareUserAddressSearchModel(model.UserAddressSearchModel, user);
         PrepareUserActivityLogSearchModel(model.UserActivityLogSearchModel, user);
         await PrepareUserAssociatedExternalAuthRecordsSearchModelAsync(model.UserAssociatedExternalAuthRecordsSearchModel, user);
      }
      else
         //whether to fill in some of properties
         if (!excludeProperties)
      {
         //precheck Registered Role as a default role while creating a new user through admin
         var registeredRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName);
         if (registeredRole != null)
            model.SelectedUserRoleIds.Add(registeredRole.Id);
      }

      model.UsernamesEnabled = _userSettings.UsernamesEnabled;
      model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
      model.FirstNameEnabled = _userSettings.FirstNameEnabled;
      model.LastNameEnabled = _userSettings.LastNameEnabled;
      model.GenderEnabled = _userSettings.GenderEnabled;
      model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
      model.CompanyEnabled = _userSettings.CompanyEnabled;
      model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
      model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
      model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
      model.CityEnabled = _userSettings.CityEnabled;
      model.CountyEnabled = _userSettings.CountyEnabled;
      model.CountryEnabled = _userSettings.CountryEnabled;
      model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
      model.PhoneEnabled = _userSettings.PhoneEnabled;
      model.FaxEnabled = _userSettings.FaxEnabled;

      //set default values for the new model
      if (user == null)
      {
         model.IsActive = true;
      }

      //prepare model user attributes
      await PrepareUserAttributeModelsAsync(model.UserAttributes, user);



      //prepare model user roles
      await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model);

      //prepare available time zones
      await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

      // user statistics summary
      model.UserStatisticsModel = await PrepareUserStatisticsModel(user);

      //prepare available countries and states
      if (_userSettings.CountryEnabled)
      {
         await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);
         if (_userSettings.StateProvinceEnabled)
            await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId == 0 ? null : model.CountryId);
      }

      return model;
   }

   /// <summary>
   /// Prepare paged user own device list model
   /// </summary>
   /// <param name="searchModel">User device search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user own device list model
   /// </returns>
   public async Task<UserOwnDeviceListModel> PrepareUserOwnDeviceListModelAsync(UserDeviceSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var devices = (await _deviceService.GetOwnDevicesByUserIdAsync(user.Id))
        .OrderByDescending(x => x.CreatedOnUtc).ThenByDescending(x => x.Id)
        .ToList().ToPagedList(searchModel);

      var model = await new UserOwnDeviceListModel().PrepareToGridAsync(searchModel, devices, () =>
      {
         return devices.SelectAwait(async device =>
         {
            //fill in model values from the entity        
            var deviceModel = device.ToModel<DeviceModel>();
            deviceModel.LastLoginDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device, "Device.Login"))?.CreatedOnUtc;
            deviceModel.LastActivityDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device))?.CreatedOnUtc;

            return deviceModel;
         });
      });

      return model;
   }


   /// <summary>
   /// Prepare paged user shared device list model
   /// </summary>
   /// <param name="searchModel">User device search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the shared user device list model
   /// </returns>
   public async Task<UserSharedDeviceListModel> PrepareUserSharedDeviceListModelAsync(UserDeviceSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var devices = (await _deviceService.GetSharedDeviceByUserIdAsync(user.Id))
        .OrderByDescending(x => x.CreatedOnUtc).ThenByDescending(x => x.Id)
        .ToList().ToPagedList(searchModel);

      var model = await new UserSharedDeviceListModel().PrepareToGridAsync(searchModel, devices, () =>
      {
         return devices.SelectAwait(async device =>
         {
            //fill in model values from the entity        
            var deviceModel = device.ToModel<DeviceModel>();

            var owner = await _userService.GetUserByIdAsync(device.OwnerId);
            deviceModel.OwnerName = _userSettings.UsernamesEnabled ? owner.Username : owner.Email;

            deviceModel.LastLoginDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device, "Device.Login"))?.CreatedOnUtc;
            deviceModel.LastActivityDateUtc = (await _deviceActivityService.GetLastActivityRecordAsync(device))?.CreatedOnUtc;

            return deviceModel;
         });
      });

      return model;
   }

   /// <summary>
   /// Prepare paged user address list model
   /// </summary>
   /// <param name="searchModel">User address search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user address list model
   /// </returns>
   public virtual async Task<UserAddressListModel> PrepareUserAddressListModelAsync(UserAddressSearchModel searchModel, User user)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (user == null)
         throw new ArgumentNullException(nameof(user));

      //get user addresses
      var addresses = (await _userService.GetAddressesByUserIdAsync(user.Id))
          .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
          .ToPagedList(searchModel);

      //prepare list model
      var model = await new UserAddressListModel().PrepareToGridAsync(searchModel, addresses, () =>
      {
         return addresses.SelectAwait(async address =>
            {
               //fill in model values from the entity        
               var addressModel = address.ToModel<AddressModel>();

               addressModel.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
               addressModel.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

               //fill in additional values (not existing in the entity)
               await PrepareModelAddressHtmlAsync(addressModel, address);

               return addressModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare user address model
   /// </summary>
   /// <param name="model">User address model</param>
   /// <param name="user">User</param>
   /// <param name="address">Address</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user address model
   /// </returns>
   public virtual async Task<UserAddressModel> PrepareUserAddressModelAsync(UserAddressModel model,
       User user, Address address, bool excludeProperties = false)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (address != null)
      {
         //fill in model values from the entity
         model ??= new UserAddressModel();

         //whether to fill in some of properties
         if (!excludeProperties)
            model.Address = address.ToModel(model.Address);
      }

      model.UserId = user.Id;

      //prepare address model
      await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
      model.Address.FirstNameRequired = true;
      model.Address.LastNameRequired = true;
      model.Address.EmailRequired = true;
      model.Address.CompanyRequired = _addressSettings.CompanyRequired;
      model.Address.CityRequired = _addressSettings.CityRequired;
      model.Address.CountyRequired = _addressSettings.CountyRequired;
      model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
      model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
      model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
      model.Address.PhoneRequired = _addressSettings.PhoneRequired;
      model.Address.FaxRequired = _addressSettings.FaxRequired;

      return model;
   }

   /// <summary>
   /// Prepare paged user activity log list model
   /// </summary>
   /// <param name="searchModel">User activity log search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user activity log list model
   /// </returns>
   public virtual async Task<UserActivityLogListModel> PrepareUserActivityLogListModelAsync(UserActivityLogSearchModel searchModel, User user)
   {
      ArgumentNullException.ThrowIfNull(searchModel);
      ArgumentNullException.ThrowIfNull(user);

      //get user activity log
      var activityLog = await _userActivityService.GetAllActivitiesAsync(userId: user.Id,
          pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //prepare list model
      var model = await new UserActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
      {
         return activityLog.SelectAwait(async logItem =>
            {
               //fill in model values from the entity
               var userActivityLogModel = logItem.ToModel<UserActivityLogModel>();

               //fill in additional values (not existing in the entity)
               userActivityLogModel.ActivityLogTypeName = (await _userActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;
               userActivityLogModel.IpAddress = _userSettings.StoreIpAddresses ? logItem.IpAddress : HubCommonDefaults.SpoofedIp;
               //convert dates to the user time
               userActivityLogModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

               return userActivityLogModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare online user search model
   /// </summary>
   /// <param name="searchModel">Online user search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online user search model
   /// </returns>
   public virtual async Task<OnlineUserSearchModel> PrepareOnlineUserSearchModelAsync(OnlineUserSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //search registered users by default
      var registeredRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName);
      if (registeredRole != null)
         searchModel.SelectedUserRoleIds.Add(registeredRole.Id);

      //prepare available user roles
      var availableRoles = await _userService.GetAllUserRolesAsync(showHidden: true);
      searchModel.AvailableUserRoles = availableRoles.Where(x => x.SystemName != UserDefaults.DevicesRoleName).Select(role => new SelectListItem
      {
         Text = role.Name,
         Value = role.Id.ToString(),
         Selected = searchModel.SelectedUserRoleIds.Contains(role.Id)

      }).ToList();

      searchModel.SearchOnline = true;
      searchModel.SearchBeenRecently = true;
      searchModel.SearchOffline = false;

      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged online user list model
   /// </summary>
   /// <param name="searchModel">Online user search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online user list model
   /// </returns>
   public virtual async Task<OnlineUserListModel> PrepareOnlineUserListModelAsync(OnlineUserSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      var now = DateTime.UtcNow;

      var activityStart = searchModel.SearchLastActivityFrom.HasValue
         ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
         : null;

      var activityEnd = searchModel.SearchLastActivityTo.HasValue
         ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
         : null;

      var onlineUserIds = await _communicator.GetOnlineUserIdsAsync();

      var users = await _userService.GetOnlineUsersAsync(
         utcNow: now,
         online: searchModel.SearchOnline,
         onlineIds: onlineUserIds.ToArray(),
         beenRecently: searchModel.SearchBeenRecently,
         offline: searchModel.SearchOffline,
         userRoleIds: searchModel.SelectedUserRoleIds.ToArray(),
         email: searchModel.SearchEmail,
         ipAddress: searchModel.SearchIpAddress,
         company: searchModel.SearchCompany,
         lastActivityFromUtc: activityStart,
         lastActivityToUtc: activityEnd);

      //prepare list model
      var model = await new OnlineUserListModel().PrepareToGridAsync(searchModel, users, () =>
      {
         return users.SelectAwait(async user =>
         {
            //fill in model values from the entity
            var userModel = new OnlineUserModel();

            userModel.Id = user.Id;

            //fill in additional values (not existing in the entity)
            userModel.UserInfo = await _userService.IsRegisteredAsync(user)
                     ? user.Email
                     : await _localizationService.GetResourceAsync("Admin.Users.Guest");

            userModel.LastIpAddress = _userSettings.StoreIpAddresses
                     ? user.LastIpAddress
                     : await _localizationService.GetResourceAsync("Admin.Users.OnlineUsers.Fields.IPAddress.Disabled");

            userModel.Location = _geoLookupService.LookupCountryName(user.LastIpAddress);

            if (onlineUserIds.Contains(userModel.Id))
            {
               userModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Online");
            }
            else
            {
               userModel.LastVisitedPage = _userSettings.PlatformLastVisitedPage
                    ? await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastVisitedPageAttribute)
                    : await _localizationService.GetResourceAsync("Admin.Users.OnlineUsers.Fields.LastVisitedPage.Disabled");

               //convert dates to the user time
               userModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(user.LastActivityUtc, DateTimeKind.Utc);

               if (user.LastActivityUtc >= now.AddMinutes(-_userSettings.BeenRecentlyMinutes))
                  userModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.BeenRecently");
               else
                  userModel.Status = await _localizationService.GetResourceAsync("Admin.Common.Online.Status.Offline");
            }
            return userModel;
         });
      });

      return model;
   }

   /// <summary>
   /// Prepare GDPR request (log) search model
   /// </summary>
   /// <param name="searchModel">GDPR request search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR request search model
   /// </returns>
   public virtual async Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare request types
      await _baseAdminModelFactory.PrepareGdprRequestTypesAsync(searchModel.AvailableRequestTypes);

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged GDPR request list model
   /// </summary>
   /// <param name="searchModel">GDPR request search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR request list model
   /// </returns>
   public virtual async Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      var userId = 0L;
      var userInfo = "";
      if (!string.IsNullOrEmpty(searchModel.SearchEmail))
      {
         var user = await _userService.GetUserByEmailAsync(searchModel.SearchEmail);
         if (user != null)
            userId = user.Id;
         else
            userInfo = searchModel.SearchEmail;
      }
      //get requests
      var gdprLog = await _gdprService.GetAllLogAsync(
          userId: userId,
          userInfo: userInfo,
          requestType: searchModel.SearchRequestTypeId > 0 ? (GdprRequestType?)searchModel.SearchRequestTypeId : null,
          pageIndex: searchModel.Page - 1,
          pageSize: searchModel.PageSize);

      //prepare list model
      var model = await new GdprLogListModel().PrepareToGridAsync(searchModel, gdprLog, () =>
      {
         return gdprLog.SelectAwait(async log =>
            {
               //fill in model values from the entity
               var user = await _userService.GetUserByIdAsync(log.UserId);

               var requestModel = log.ToModel<GdprLogModel>();

               //fill in additional values (not existing in the entity)
               requestModel.UserInfo = user != null && !user.IsDeleted && !string.IsNullOrEmpty(user.Email)
                      ? user.Email
                      : log.UserInfo;
               requestModel.RequestType = await _localizationService.GetLocalizedEnumAsync(log.RequestType);

               requestModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);

               return requestModel;
            });
      });

      return model;
   }

   #endregion
}