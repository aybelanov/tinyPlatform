using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ClosedXML.Excel;
using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Services.ExportImport.Help;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.Messages;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Services.Logging;
using Hub.Services.Devices;
using Hub.Core.Domain.Clients;

namespace Hub.Services.ExportImport;

/// <summary>
/// Export manager
/// </summary>
public partial class ExportManager : IExportManager
{
   #region Fields

   private readonly AddressSettings _addressSettings;
   private readonly UserSettings _userSettings;
   private readonly DateTimeSettings _dateTimeSettings;
   private readonly ICountryService _countryService;
   private readonly IUserAttributeFormatter _userAttributeFormatter;
   private readonly IUserService _userService;
   private readonly IHubDeviceService _deviceService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly IPictureService _pictureService;
   private readonly IStateProvinceService _stateProvinceService;
   private readonly IWorkContext _workContext;
   private readonly CommonSettings _commonSettings;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ExportManager(AddressSettings addressSettings,
       UserSettings userSettings,
       DateTimeSettings dateTimeSettings,
       ICountryService countryService,
       IUserAttributeFormatter userAttributeFormatter,
       IUserService userService,
       IHubDeviceService deviceService,
       IDeviceActivityService deviceActivityService,
       IGenericAttributeService genericAttributeService,
       ILocalizationService localizationService,
       IPictureService pictureService,
       IStateProvinceService stateProvinceService,
       IWorkContext workContext,
       CommonSettings commonSettings)
   {
      _addressSettings = addressSettings;
      _userSettings = userSettings;
      _dateTimeSettings = dateTimeSettings;
      _countryService = countryService;
      _userAttributeFormatter = userAttributeFormatter;
      _userService = userService;
      _deviceActivityService = deviceActivityService;
      _deviceService = deviceService;
      _genericAttributeService = genericAttributeService;
      _localizationService = localizationService;
      _pictureService = pictureService;
      _stateProvinceService = stateProvinceService;
      _workContext = workContext;
      _commonSettings = commonSettings;
   }

   #endregion

   #region Utilities

   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual Task WriteCategoriesAsync(XmlWriter xmlWriter, int parentCategoryId)
   {
      // TODO implementation
      throw new NotImplementedException();
   }

   /// <summary>
   /// Returns the path to the image file by ID
   /// </summary>
   /// <param name="pictureId">Picture ID</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the path to the image file
   /// </returns>
   protected virtual async Task<string> GetPicturesAsync(long pictureId)
   {
      var picture = await _pictureService.GetPictureByIdAsync(pictureId);

      return await _pictureService.GetThumbLocalPathAsync(picture);
   }



   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task<bool> IgnoreExportCategoryPropertyAsync()
   {
      try
      {
         return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "category-advanced-mode");
      }
      catch (ArgumentNullException)
      {
         return false;
      }
   }

   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task<bool> IgnoreExportManufacturerPropertyAsync()
   {
      try
      {
         return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "manufacturer-advanced-mode");
      }
      catch (ArgumentNullException)
      {
         return false;
      }
   }

   /// <returns>A task that represents the asynchronous operation</returns>
   private async Task<object> GetCustomUserAttributesAsync(User user)
   {
      var selectedUserAttributes = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CustomUserAttributes);

      return await _userAttributeFormatter.FormatAttributesAsync(selectedUserAttributes, ";");
   }

   #endregion

   #region Methods


   /// <summary>
   /// Export user list to XLSX
   /// </summary>
   /// <param name="users">Users</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task<byte[]> ExportUsersToXlsxAsync(IList<User> users)
   {
      async Task<object> getPasswordFormat(User user)
      {
         var password = await _userService.GetCurrentPasswordAsync(user.Id);
         var passwordFormatId = password?.PasswordFormatId ?? 0;
         return CommonHelper.ConvertEnum(((PasswordFormat)passwordFormatId).ToString());
      }


      async Task<object> getCountry(User user)
      {
         var countryId = await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.CountryIdAttribute);
         var country = await _countryService.GetCountryByIdAsync(countryId);
         return country?.Name ?? string.Empty;
      }

      async Task<object> getStateProvince(User user)
      {
         var stateProvinceId = await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.StateProvinceIdAttribute);
         var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);
         return stateProvince?.Name ?? string.Empty;
      }


      //property manager 
      var manager = new PropertyManager<User>(new[]
      {
             new PropertyByName<User>("UserId", p => p.Id),
             new PropertyByName<User>("UserGuid", p => p.UserGuid),
             new PropertyByName<User>("Email", p => p.Email),
             new PropertyByName<User>("Username", p => p.Username),
             //new PropertyByName<User>("Password", async p => (await _userService.GetCurrentPasswordAsync(p.Id))?.Password),
             new PropertyByName<User>("PasswordFormat", getPasswordFormat),
             //new PropertyByName<User>("PasswordSalt", async p => (await _userService.GetCurrentPasswordAsync(p.Id))?.PasswordSalt),
             new PropertyByName<User>("AffiliateId", p => p.AffiliateId),
             new PropertyByName<User>("Active", p => p.IsActive),
             new PropertyByName<User>("UserRoles", async p=>string.Join(", ",(await _userService.GetUserRolesAsync(p)).Select(role =>role.SystemName))),
             new PropertyByName<User>("IsGuest", async p => await _userService.IsGuestAsync(p)),
             new PropertyByName<User>("IsRegistered", async p => await _userService.IsRegisteredAsync(p)),
             new PropertyByName<User>("IsAdministrator", async p => await _userService.IsAdminAsync(p)),
             new PropertyByName<User>("IsForumModerator", async p => await _userService.IsForumModeratorAsync(p)),
             new PropertyByName<User>("IsDeviceUser", async p => await _userService.IsDeviceUserAsync(p)),
             new PropertyByName<User>("IsDeviceOwner", async p => await _userService.IsOwnerAsync(p)),
             new PropertyByName<User>("IsDeviceOperator", async p => await _userService.IsOperatorAsync(p)),
             new PropertyByName<User>("IsDemoUser", async p => await _userService.IsDemoUserAsync(p)),
             new PropertyByName<User>("CreatedOnUtc", p => p.CreatedOnUtc),
             new PropertyByName<User>("AvatarPictureId", p => p.AvatarPictureId, !_userSettings.AllowUsersToUploadAvatars),
             //attributes
             new PropertyByName<User>("FirstName", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.FirstNameAttribute), !_userSettings.FirstNameEnabled),
             new PropertyByName<User>("LastName", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.LastNameAttribute), !_userSettings.LastNameEnabled),
             new PropertyByName<User>("Gender", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.GenderAttribute), !_userSettings.GenderEnabled),
             new PropertyByName<User>("Company", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.CompanyAttribute), !_userSettings.CompanyEnabled),
             new PropertyByName<User>("StreetAddress", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.StreetAddressAttribute), !_userSettings.StreetAddressEnabled),
             new PropertyByName<User>("StreetAddress2", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.StreetAddress2Attribute), !_userSettings.StreetAddress2Enabled),
             new PropertyByName<User>("ZipPostalCode", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.ZipPostalCodeAttribute), !_userSettings.ZipPostalCodeEnabled),
             new PropertyByName<User>("City", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.CityAttribute), !_userSettings.CityEnabled),
             new PropertyByName<User>("County", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.CountyAttribute), !_userSettings.CountyEnabled),
             new PropertyByName<User>("Country", getCountry, !_userSettings.CountryEnabled),
             new PropertyByName<User>("StateProvince", getStateProvince, !_userSettings.StateProvinceEnabled),
             new PropertyByName<User>("Phone", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.PhoneAttribute), !_userSettings.PhoneEnabled),
             new PropertyByName<User>("Fax", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.FaxAttribute), !_userSettings.FaxEnabled),
             new PropertyByName<User>("VatNumber", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.VatNumberAttribute)),
             new PropertyByName<User>("TimeZone", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.TimeZoneIdAttribute), !_dateTimeSettings.AllowUsersToSetTimeZone),
             new PropertyByName<User>("ForumPostCount", async p => await _genericAttributeService.GetAttributeAsync<int>(p, AppUserDefaults.ForumPostCountAttribute)),
             new PropertyByName<User>("Signature", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.SignatureAttribute)),
             new PropertyByName<User>("CustomUserAttributes",  GetCustomUserAttributesAsync)
         }, _commonSettings);

      return await manager.ExportToXlsxAsync(users);
   }

   /// <summary>
   /// Export user list to XML
   /// </summary>
   /// <param name="users">Users</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in XML format
   /// </returns>
   public virtual async Task<string> ExportUsersToXmlAsync(IList<User> users)
   {
      var settings = new XmlWriterSettings
      {
         Async = true,
         ConformanceLevel = ConformanceLevel.Auto
      };

      await using var stringWriter = new StringWriter();
      await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

      await xmlWriter.WriteStartDocumentAsync();
      await xmlWriter.WriteStartElementAsync("Users");
      await xmlWriter.WriteAttributeStringAsync("Version", AppVersion.CURRENT_VERSION);

      foreach (var user in users)
      {
         await xmlWriter.WriteStartElementAsync("User");
         await xmlWriter.WriteElementStringAsync("UserId", null, user.Id.ToString());
         await xmlWriter.WriteElementStringAsync("UserGuid", null, user.UserGuid.ToString());
         await xmlWriter.WriteElementStringAsync("Email", null, user.Email);
         await xmlWriter.WriteElementStringAsync("Username", null, user.Username);

         var userPassword = await _userService.GetCurrentPasswordAsync(user.Id);
         //await xmlWriter.WriteElementStringAsync("Password", null, userPassword?.Password);
         await xmlWriter.WriteElementStringAsync("PasswordFormatId", null, (userPassword?.PasswordFormatId ?? 0).ToString());
         //await xmlWriter.WriteElementStringAsync("PasswordSalt", null, userPassword?.PasswordSalt);

         await xmlWriter.WriteElementStringAsync("AffiliateId", null, user.AffiliateId.ToString());
         await xmlWriter.WriteElementStringAsync("Active", null, user.IsActive.ToString());

         await xmlWriter.WriteElementStringAsync("IsGuest", null, (await _userService.IsGuestAsync(user)).ToString());
         await xmlWriter.WriteElementStringAsync("IsRegistered", null, (await _userService.IsRegisteredAsync(user)).ToString());
         await xmlWriter.WriteElementStringAsync("IsAdministrator", null, (await _userService.IsAdminAsync(user)).ToString());
         await xmlWriter.WriteElementStringAsync("IsForumModerator", null, (await _userService.IsForumModeratorAsync(user)).ToString());
         await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, user.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

         await xmlWriter.WriteElementStringAsync("FirstName", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FirstNameAttribute));
         await xmlWriter.WriteElementStringAsync("LastName", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastNameAttribute));
         await xmlWriter.WriteElementStringAsync("Gender", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.GenderAttribute));
         await xmlWriter.WriteElementStringAsync("Company", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CompanyAttribute));

         await xmlWriter.WriteElementStringAsync("CountryId", null, (await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.CountryIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("StreetAddress", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.StreetAddressAttribute));
         await xmlWriter.WriteElementStringAsync("StreetAddress2", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.StreetAddress2Attribute));
         await xmlWriter.WriteElementStringAsync("ZipPostalCode", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.ZipPostalCodeAttribute));
         await xmlWriter.WriteElementStringAsync("City", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CityAttribute));
         await xmlWriter.WriteElementStringAsync("County", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CountyAttribute));
         await xmlWriter.WriteElementStringAsync("StateProvinceId", null, (await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.StateProvinceIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("Phone", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.PhoneAttribute));
         await xmlWriter.WriteElementStringAsync("Fax", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FaxAttribute));
         await xmlWriter.WriteElementStringAsync("VatNumber", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.VatNumberAttribute));
         await xmlWriter.WriteElementStringAsync("VatNumberStatusId", null, (await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.VatNumberStatusIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("TimeZoneId", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.TimeZoneIdAttribute));


         await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, user.AvatarPictureId.ToString());
         //await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, (await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.AvatarPictureIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("ForumPostCount", null, (await _genericAttributeService.GetAttributeAsync<int>(user, AppUserDefaults.ForumPostCountAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("Signature", null, await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.SignatureAttribute));

         var selectedUserAttributesString = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CustomUserAttributes);

         if (!string.IsNullOrEmpty(selectedUserAttributesString))
         {
            var selectedUserAttributes = new StringReader(selectedUserAttributesString);
            var selectedUserAttributesXmlReader = XmlReader.Create(selectedUserAttributes);
            await xmlWriter.WriteNodeAsync(selectedUserAttributesXmlReader, false);
         }

         await xmlWriter.WriteEndElementAsync();
      }

      await xmlWriter.WriteEndElementAsync();
      await xmlWriter.WriteEndDocumentAsync();
      await xmlWriter.FlushAsync();

      return stringWriter.ToString();
   }

   /// <summary>
   /// Export newsletter subscribers to TXT
   /// </summary>
   /// <param name="subscriptions">Subscriptions</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in TXT (string) format
   /// </returns>
   public virtual async Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions)
   {
      if (subscriptions == null)
         throw new ArgumentNullException(nameof(subscriptions));

      const char separator = ',';
      var sb = new StringBuilder();

      sb.Append(await _localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.Fields.Email"));
      sb.Append(separator);
      sb.Append(await _localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.Fields.Active"));
      sb.Append(separator);
      sb.Append(await _localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.Fields.Platform"));
      sb.Append(Environment.NewLine);

      foreach (var subscription in subscriptions)
      {
         sb.Append(subscription.Email);
         sb.Append(separator);
         sb.Append(subscription.Active);
         sb.Append(separator);
         sb.Append(Environment.NewLine);
      }

      return sb.ToString();
   }

   /// <summary>
   /// Export states to TXT
   /// </summary>
   /// <param name="states">States</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in TXT (string) format
   /// </returns>
   public virtual async Task<string> ExportStatesToTxtAsync(IList<StateProvince> states)
   {
      if (states == null)
         throw new ArgumentNullException(nameof(states));

      const char separator = ',';
      var sb = new StringBuilder();
      foreach (var state in states)
      {
         sb.Append((await _countryService.GetCountryByIdAsync(state.CountryId)).TwoLetterIsoCode);
         sb.Append(separator);
         sb.Append(state.Name);
         sb.Append(separator);
         sb.Append(state.Abbreviation);
         sb.Append(separator);
         sb.Append(state.Published);
         sb.Append(separator);
         sb.Append(state.DisplayOrder);
         sb.Append(Environment.NewLine); //new line
      }

      return sb.ToString();
   }

   /// <summary>
   /// Export user info (GDPR request) to XLSX 
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user GDPR info
   /// </returns>
   public virtual async Task<byte[]> ExportUserGdprInfoToXlsxAsync(User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      //user info and user attributes
      var userManager = new PropertyManager<User>(new[]
      {
             new PropertyByName<User>("Email", p => p.Email),
             new PropertyByName<User>("Username", p => p.Username, !_userSettings.UsernamesEnabled), 
             //attributes
             new PropertyByName<User>("First name", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.FirstNameAttribute), !_userSettings.FirstNameEnabled),
             new PropertyByName<User>("Last name", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.LastNameAttribute), !_userSettings.LastNameEnabled),
             new PropertyByName<User>("Gender", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.GenderAttribute), !_userSettings.GenderEnabled),
             new PropertyByName<User>("Date of birth", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.DateOfBirthAttribute), !_userSettings.DateOfBirthEnabled),
             new PropertyByName<User>("Company", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.CompanyAttribute), !_userSettings.CompanyEnabled),
             new PropertyByName<User>("Street address", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.StreetAddressAttribute), !_userSettings.StreetAddressEnabled),
             new PropertyByName<User>("Street address 2", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.StreetAddress2Attribute), !_userSettings.StreetAddress2Enabled),
             new PropertyByName<User>("Zip / postal code", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.ZipPostalCodeAttribute), !_userSettings.ZipPostalCodeEnabled),
             new PropertyByName<User>("City", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.CityAttribute), !_userSettings.CityEnabled),
             new PropertyByName<User>("County", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.CountyAttribute), !_userSettings.CountyEnabled),
             new PropertyByName<User>("Country", async p => (await _countryService.GetCountryByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(p, AppUserDefaults.CountryIdAttribute)))?.Name ?? string.Empty, !_userSettings.CountryEnabled),
             new PropertyByName<User>("State province", async p => (await _stateProvinceService.GetStateProvinceByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(p, AppUserDefaults.StateProvinceIdAttribute)))?.Name ?? string.Empty, !(_userSettings.StateProvinceEnabled && _userSettings.CountryEnabled)),
             new PropertyByName<User>("Phone", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.PhoneAttribute), !_userSettings.PhoneEnabled),
             new PropertyByName<User>("Fax", async p => await _genericAttributeService.GetAttributeAsync<string>(p, AppUserDefaults.FaxAttribute), !_userSettings.FaxEnabled),
             new PropertyByName<User>("User attributes",  GetCustomUserAttributesAsync)
         }, _commonSettings);

      //user orders
      var currentLanguage = await _workContext.GetWorkingLanguageAsync();

      //user addresses
      var addressManager = new PropertyManager<Address>(new[]
      {
             new PropertyByName<Address>("First name", p => p.FirstName),
             new PropertyByName<Address>("Last name", p => p.LastName),
             new PropertyByName<Address>("Email", p => p.Email),
             new PropertyByName<Address>("Company", p => p.Company, !_addressSettings.CompanyEnabled),
             new PropertyByName<Address>("Country", async p => await _countryService.GetCountryByAddressAsync(p) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
             new PropertyByName<Address>("State province", async p => await _stateProvinceService.GetStateProvinceByAddressAsync(p) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
             new PropertyByName<Address>("County", p => p.County, !_addressSettings.CountyEnabled),
             new PropertyByName<Address>("City", p => p.City, !_addressSettings.CityEnabled),
             new PropertyByName<Address>("Address 1", p => p.Address1, !_addressSettings.StreetAddressEnabled),
             new PropertyByName<Address>("Address 2", p => p.Address2, !_addressSettings.StreetAddress2Enabled),
             new PropertyByName<Address>("Zip / postal code", p => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
             new PropertyByName<Address>("Phone number", p => p.PhoneNumber, !_addressSettings.PhoneEnabled),
             new PropertyByName<Address>("Fax number", p => p.FaxNumber, !_addressSettings.FaxEnabled),
             new PropertyByName<Address>("Custom attributes", async p => await _userAttributeFormatter.FormatAttributesAsync(p.CustomAttributes, ";"))
         }, _commonSettings);


      await using var stream = new MemoryStream();
      // ok, we can run the real code of the sample now
      using (var workbook = new XLWorkbook())
      {
         // uncomment this line if you want the XML written out to the outputDir
         //xlPackage.DebugMode = true; 

         // get handles to the worksheets
         // Worksheet names cannot be more than 31 characters
         var userInfoWorksheet = workbook.Worksheets.Add("User info");
         var fWorksheet = workbook.Worksheets.Add("DataForFilters");
         fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

         //user info and user attributes
         var userInfoRow = 2;
         userManager.CurrentObject = user;
         userManager.WriteCaption(userInfoWorksheet);
         await userManager.WriteToXlsxAsync(userInfoWorksheet, userInfoRow);

         //user addresses
         if (await _userService.GetAddressesByUserIdAsync(user.Id) is IList<Address> addresses && addresses.Any())
         {
            userInfoRow += 2;

            var cell = userInfoWorksheet.Row(userInfoRow).Cell(1);
            cell.Value = "Address List";
            userInfoRow += 1;
            addressManager.SetCaptionStyle(cell);
            addressManager.WriteCaption(userInfoWorksheet, userInfoRow);

            foreach (var userAddress in addresses)
            {
               userInfoRow += 1;
               addressManager.CurrentObject = userAddress;
               await addressManager.WriteToXlsxAsync(userInfoWorksheet, userInfoRow);
            }
         }

         workbook.SaveAs(stream);
      }

      return stream.ToArray();
   }

   /// <summary>
   /// Export device list to XLSX
   /// </summary>
   /// <param name="devices">Users</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task<byte[]> ExportDevicesToXlsxAsync(IList<Device> devices)
   {
      async Task<object> getPasswordFormat(Device device)
      {
         var password = await _deviceService.GetCurrentPasswordAsync(device.Id);

         var passwordFormatId = password?.PasswordFormatId ?? 0;


         return CommonHelper.ConvertEnum(((PasswordFormat)passwordFormatId).ToString());
      }

      async Task<object> getCountry(Device device)
      {
         var countryId = await _genericAttributeService.GetAttributeAsync<int>(device, AppUserDefaults.CountryIdAttribute);


         var country = await _countryService.GetCountryByIdAsync(countryId);

         return country?.Name ?? string.Empty;
      }

      async Task<object> getStateProvince(Device device)
      {
         var stateProvinceId = await _genericAttributeService.GetAttributeAsync<int>(device, AppUserDefaults.StateProvinceIdAttribute);


         var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);

         return stateProvince?.Name ?? string.Empty;
      }

      async Task<User> getOwner(Device device)
      {
         var owner = await _userService.GetUserByIdAsync(device.OwnerId);
         return owner;
      }

      var language = await _workContext.GetWorkingLanguageAsync();

      //property manager 
      var manager = new PropertyManager<Device>(new[]
      {
         new PropertyByName<Device>("DeviceId", p => p.Id),
         new PropertyByName<Device>("Guid", p => p.Guid),
         new PropertyByName<Device>("SystemName", p => p.SystemName),

         new PropertyByName<Device>("Name", async p => await _localizationService.GetLocalizedAsync(p, device => device.Name, language.Id)),
         new PropertyByName<Device>("Description", async p => await _localizationService.GetLocalizedAsync(p, device => device.Description, language.Id)),

         //new PropertyByName<Device>("Password", async p => (await _deviceService.GetCurrentPasswordAsync(p.Id))?.Password),
         new PropertyByName<Device>("PasswordFormat", getPasswordFormat),
         //new PropertyByName<Device>("PasswordSalt", async p => (await _deviceService.GetCurrentPasswordAsync(p.Id))?.PasswordSalt),
         new PropertyByName<Device>("Active", p => p.IsActive),
         new PropertyByName<Device>("IsDeleted", p => p.IsDeleted),
         new PropertyByName<Device>("Enabled", p => p.Enabled),


         new PropertyByName<Device>("CreatedOnUtc", p => p.CreatedOnUtc),
         new PropertyByName<Device>("UpdatedOnUtc", p => p.UpdatedOnUtc),
         new PropertyByName<Device>("CannotLoginOnUtc", p => p.CannotLoginUntilDateUtc),

         new PropertyByName<Device>("LastActivity", async p => (await _deviceActivityService.GetLastActivityRecordAsync(p))?.CreatedOnUtc),
         new PropertyByName<Device>("LastLogin", async p => (await _deviceActivityService.GetLastActivityRecordAsync(p, "Device.Login"))?.CreatedOnUtc),

         new PropertyByName<Device>("OwnerEmail", async p => (await getOwner(p)).Email),
         new PropertyByName<Device>("OwnerUsername", async p => (await getOwner(p)).Username),
         //attributes
         new PropertyByName<Device>("FirstName", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.FirstNameAttribute), !_userSettings.FirstNameEnabled),
         new PropertyByName<Device>("LastName", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.LastNameAttribute), !_userSettings.LastNameEnabled),
         new PropertyByName<Device>("Company", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.CompanyAttribute), !_userSettings.CompanyEnabled),
         new PropertyByName<Device>("StreetAddress", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.StreetAddressAttribute), !_userSettings.StreetAddressEnabled),
         new PropertyByName<Device>("StreetAddress2", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.StreetAddress2Attribute), !_userSettings.StreetAddress2Enabled),
         new PropertyByName<Device>("ZipPostalCode", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.ZipPostalCodeAttribute), !_userSettings.ZipPostalCodeEnabled),
         new PropertyByName<Device>("City", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.CityAttribute), !_userSettings.CityEnabled),
         new PropertyByName<Device>("County", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.CountyAttribute), !_userSettings.CountyEnabled),
         new PropertyByName<Device>("Country", getCountry, !_userSettings.CountryEnabled),
         new PropertyByName<Device>("StateProvince", getStateProvince, !_userSettings.StateProvinceEnabled),
         new PropertyByName<Device>("Phone", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.PhoneAttribute), !_userSettings.PhoneEnabled),
         new PropertyByName<Device>("Fax", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.FaxAttribute), !_userSettings.FaxEnabled),
         new PropertyByName<Device>("VatNumber", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.VatNumberAttribute)),
         new PropertyByName<Device>("TimeZone", async p => await _genericAttributeService.GetAttributeAsync<string>(await getOwner(p), AppUserDefaults.TimeZoneIdAttribute), !_dateTimeSettings.AllowUsersToSetTimeZone),

         }, _commonSettings);

      return await manager.ExportToXlsxAsync(devices);
   }

   /// <summary>
   /// Export device list to XML
   /// </summary>
   /// <param name="devices">Devices</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in XML format
   /// </returns>
   public async Task<string> ExportDevicesToXmlAsync(IList<Device> devices)
   {
      var settings = new XmlWriterSettings
      {
         Async = true,
         ConformanceLevel = ConformanceLevel.Auto
      };

      await using var stringWriter = new StringWriter();
      await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

      await xmlWriter.WriteStartDocumentAsync();
      await xmlWriter.WriteStartElementAsync("Devices");
      await xmlWriter.WriteAttributeStringAsync("Version", AppVersion.CURRENT_VERSION);

      //var language = (await _languageService.GetAllLanguagesAsync()).First(x => x.LanguageCulture == AppCommonDefaults.DefaultLanguageCulture);
      var language = await _workContext.GetWorkingLanguageAsync();

      foreach (var device in devices)
      {
         await xmlWriter.WriteStartElementAsync("User");
         await xmlWriter.WriteElementStringAsync("DeviceId", null, device.Id.ToString());
         await xmlWriter.WriteElementStringAsync("Guid", null, device.Guid.ToString());
         await xmlWriter.WriteElementStringAsync("SystemName", null, device.SystemName);

         await xmlWriter.WriteElementStringAsync("Name", null, await _localizationService.GetLocalizedAsync(device, device => device.Name, language.Id));
         await xmlWriter.WriteElementStringAsync("Description", null, await _localizationService.GetLocalizedAsync(device, device => device.Description, language.Id));

         var devicePassword = await _deviceService.GetCurrentPasswordAsync(device.Id);
         //await xmlWriter.WriteElementStringAsync("Password", null, devicePassword?.Password);
         await xmlWriter.WriteElementStringAsync("PasswordFormatId", null, (devicePassword?.PasswordFormatId ?? 0).ToString());
         //await xmlWriter.WriteElementStringAsync("PasswordSalt", null, devicePassword?.PasswordSalt);

         await xmlWriter.WriteElementStringAsync("Active", null, device.IsActive.ToString());
         await xmlWriter.WriteElementStringAsync("Deleted", null, device.IsDeleted.ToString());
         await xmlWriter.WriteElementStringAsync("Enabled", null, device.Enabled.ToString());

         await xmlWriter.WriteElementStringAsync("Configuration", null, device.Configuration.ToString());
         await xmlWriter.WriteElementStringAsync("ClearDataDelay", null, device.ClearDataDelay.ToString());
         await xmlWriter.WriteElementStringAsync("CountDataRows", null, device.CountDataRows.ToString());
         await xmlWriter.WriteElementStringAsync("DataflowReconnectDelay", null, device.DataflowReconnectDelay.ToString());
         await xmlWriter.WriteElementStringAsync("DataSendingDelay", null, device.DataSendingDelay.ToString());
         
         // TODO get IPAddress from communicator
         //await xmlWriter.WriteElementStringAsync("IPAddress", null, device.IPAddress?.ToString() ?? "");

         await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, device.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));
         await xmlWriter.WriteElementStringAsync("UpdatedOnUtc", null, device.UpdatedOnUtc?.ToString(CultureInfo.InvariantCulture) ?? "");
         await xmlWriter.WriteElementStringAsync("CannotLoginUtc", null, device.CannotLoginUntilDateUtc?.ToString(CultureInfo.InvariantCulture) ?? "");

         await xmlWriter.WriteElementStringAsync("LastActivity", null, (await _deviceActivityService.GetLastActivityRecordAsync(device))?.CreatedOnUtc.ToString(CultureInfo.InvariantCulture) ?? "");
         await xmlWriter.WriteElementStringAsync("LastLogin", null, (await _deviceActivityService.GetLastActivityRecordAsync(device, "Device.Login"))?.CreatedOnUtc.ToString(CultureInfo.InvariantCulture) ?? "");

         var owner = await _userService.GetUserByIdAsync(device.OwnerId);
         await xmlWriter.WriteElementStringAsync("OwnerEmail", null, owner.Email);
         await xmlWriter.WriteElementStringAsync("OwnerUsername", null, owner.Username);
         await xmlWriter.WriteElementStringAsync("OwnerFirstName", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.LastNameAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerLastName", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.LastNameAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerCompany", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.CompanyAttribute));

         await xmlWriter.WriteElementStringAsync("OwnerCountryId", null, (await _genericAttributeService.GetAttributeAsync<int>(owner, AppUserDefaults.CountryIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("OwnerStreetAddress", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.StreetAddressAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerStreetAddress2", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.StreetAddress2Attribute));
         await xmlWriter.WriteElementStringAsync("OwnerZipPostalCode", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.ZipPostalCodeAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerCity", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.CityAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerCounty", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.CountyAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerStateProvinceId", null, (await _genericAttributeService.GetAttributeAsync<int>(owner, AppUserDefaults.StateProvinceIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("OwnerPhone", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.PhoneAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerFax", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.FaxAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerVatNumber", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.VatNumberAttribute));
         await xmlWriter.WriteElementStringAsync("OwnerVatNumberStatusId", null, (await _genericAttributeService.GetAttributeAsync<int>(owner, AppUserDefaults.VatNumberStatusIdAttribute)).ToString());
         await xmlWriter.WriteElementStringAsync("OwnerTimeZoneId", null, await _genericAttributeService.GetAttributeAsync<string>(owner, AppUserDefaults.TimeZoneIdAttribute));

         var selectedUserAttributesString = await _genericAttributeService.GetAttributeAsync<string>(device, AppUserDefaults.CustomUserAttributes);

         if (!string.IsNullOrEmpty(selectedUserAttributesString))
         {
            var selectedUserAttributes = new StringReader(selectedUserAttributesString);
            var selectedUserAttributesXmlReader = XmlReader.Create(selectedUserAttributes);
            await xmlWriter.WriteNodeAsync(selectedUserAttributesXmlReader, false);
         }

         await xmlWriter.WriteEndElementAsync();
      }

      await xmlWriter.WriteEndElementAsync();
      await xmlWriter.WriteEndDocumentAsync();
      await xmlWriter.FlushAsync();

      return stringWriter.ToString();
   }

   #endregion
}
