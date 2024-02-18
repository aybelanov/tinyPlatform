using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Caching;
using Hub.Core.Domain.Gdpr;
using Hub.Core.Domain.Logging;
using Hub.Services;
using Hub.Services.Directory;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Plugins;
using Hub.Services.Topics;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Infrastructure.Cache;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the implementation of the base model factory that implements a most common admin model factories methods
   /// </summary>
   public partial class BaseAdminModelFactory : IBaseAdminModelFactory
   {
      #region Fields

      private readonly ICountryService _countryService;
      private readonly ICurrencyService _currencyService;
      private readonly IUserActivityService _userActivityService;
      private readonly IDeviceActivityService _deviceActivityService;
      private readonly IUserService _userService;
      private readonly IDateTimeHelper _dateTimeHelper;
      private readonly IEmailAccountService _emailAccountService;
      private readonly ILanguageService _languageService;
      private readonly ILocalizationService _localizationService;
      private readonly IPluginService _pluginService;
      private readonly IStateProvinceService _stateProvinceService;
      private readonly IStaticCacheManager _staticCacheManager;
      private readonly ITopicTemplateService _topicTemplateService;

      #endregion

      #region Ctor

      public BaseAdminModelFactory(ICountryService countryService,
          ICurrencyService currencyService,
          IUserActivityService userActivityService,
          IUserService userService,
          IDeviceActivityService deviceActivityService,
          IDateTimeHelper dateTimeHelper,
          IEmailAccountService emailAccountService,
          ILanguageService languageService,
          ILocalizationService localizationService,
          IPluginService pluginService,
          IStateProvinceService stateProvinceService,
          IStaticCacheManager staticCacheManager,
          ITopicTemplateService topicTemplateService)
      {
         _countryService = countryService;
         _currencyService = currencyService;
         _userActivityService = userActivityService;
         _deviceActivityService = deviceActivityService;
         _userService = userService;
         _dateTimeHelper = dateTimeHelper;
         _emailAccountService = emailAccountService;
         _languageService = languageService;
         _localizationService = localizationService;
         _pluginService = pluginService;
         _stateProvinceService = stateProvinceService;
         _staticCacheManager = staticCacheManager;
         _topicTemplateService = topicTemplateService;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Prepare default item
      /// </summary>
      /// <param name="items">Available items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
      /// <param name="defaultItemValue">Default item value; defaults 0</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected virtual async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //whether to insert the first special item for the default value
         if (!withSpecialDefaultItem)
            return;

         //prepare item text
         defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

         //insert this default item at first
         items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare available activity log types
      /// </summary>
      /// <param name="items">Activity log type items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <param name="toDevice">Prepare for device activity log</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null, bool toDevice = false)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available activity log types
         var availableActivityTypes = toDevice ? await _deviceActivityService.GetAllActivityTypesAsync() : await _userActivityService.GetAllActivityTypesAsync();

         if (toDevice)
            availableActivityTypes = availableActivityTypes.Where(x => x.SystemKeyword.StartsWith("Device.")).ToList();
         else
            availableActivityTypes = availableActivityTypes.Where(x => !x.SystemKeyword.StartsWith("Device.")).ToList();

         foreach (var activityType in availableActivityTypes)
            items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available countries
      /// </summary>
      /// <param name="items">Country items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available countries
         var availableCountries = await _countryService.GetAllCountriesAsync(showHidden: true);
         foreach (var country in availableCountries)
            items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"));
      }

      /// <summary>
      /// Prepare available states and provinces
      /// </summary>
      /// <param name="items">State and province items</param>
      /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, long? countryId,
          bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         if (countryId.HasValue)
         {
            //prepare available states and provinces of the country
            var availableStates = await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId.Value, showHidden: true);
            foreach (var state in availableStates)
               items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Name });

            //insert special item for the default value
            if (items.Count > 1)
               await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectState"));
         }

         //insert special item for the default value
         if (!items.Any())
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.Other"));
      }

      /// <summary>
      /// Prepare available languages
      /// </summary>
      /// <param name="items">Language items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available languages
         var availableLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
         foreach (var language in availableLanguages)
            items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available user roles
      /// </summary>
      /// <param name="items">User role items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareUserRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available user roles
         var availableUserRoles = await _userService.GetAllUserRolesAsync();
         foreach (var userRole in availableUserRoles)
            items.Add(new SelectListItem { Value = userRole.Id.ToString(), Text = userRole.Name });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available email accounts
      /// </summary>
      /// <param name="items">Email account items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available email accounts
         var availableEmailAccounts = await _emailAccountService.GetAllEmailAccountsAsync();
         foreach (var emailAccount in availableEmailAccounts)
            items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available time zones
      /// </summary>
      /// <param name="items">Time zone items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available time zones
         var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
         foreach (var timeZone in availableTimeZones)
            items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available currencies
      /// </summary>
      /// <param name="items">Currency items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareCurrenciesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available currencies
         var availableCurrencies = await _currencyService.GetAllCurrenciesAsync(true);
         foreach (var currency in availableCurrencies)
            items.Add(new SelectListItem { Value = currency.Id.ToString(), Text = currency.Name });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available log levels
      /// </summary>
      /// <param name="items">Log level items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available log levels
         var availableLogLevelItems = await LogLevel.Debug.ToSelectListAsync(false);
         foreach (var logLevelItem in availableLogLevelItems)
            items.Add(logLevelItem);

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available load plugin modes
      /// </summary>
      /// <param name="items">Load plugin mode items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareLoadPluginModesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available load plugin modes
         var availableLoadPluginModeItems = await LoadPluginsMode.All.ToSelectListAsync(false);
         foreach (var loadPluginModeItem in availableLoadPluginModeItems)
            items.Add(loadPluginModeItem);

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available plugin groups
      /// </summary>
      /// <param name="items">Plugin group items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PreparePluginGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available plugin groups
         var availablePluginGroups = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
             .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
         foreach (var group in availablePluginGroups)
            items.Add(new SelectListItem { Value = @group, Text = @group });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available topic templates
      /// </summary>
      /// <param name="items">Topic template items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareTopicTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available topic templates
         var availableTemplates = await _topicTemplateService.GetAllTopicTemplatesAsync();
         foreach (var template in availableTemplates)
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      /// <summary>
      /// Prepare available GDPR request types
      /// </summary>
      /// <param name="items">Request type items</param>
      /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
      /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task PrepareGdprRequestTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
      {
         if (items == null)
            throw new ArgumentNullException(nameof(items));

         //prepare available request types
         var gdprRequestTypeItems = await GdprRequestType.ConsentAgree.ToSelectListAsync(false);
         foreach (var gdprRequestTypeItem in gdprRequestTypeItems)
            items.Add(gdprRequestTypeItem);

         //insert special item for the default value
         await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
      }

      #endregion
   }
}