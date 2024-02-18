using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the base model factory that implements a most common admin model factories methods
/// </summary>
public partial interface IBaseAdminModelFactory
{
   /// <summary>
   /// Prepare available activity log types
   /// </summary>
   /// <param name="items">Activity log type items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <param name="toDevice">Prepare for device activity log</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null, bool toDevice = false);

   /// <summary>
   /// Prepare available countries
   /// </summary>
   /// <param name="items">Country items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available states and provinces
   /// </summary>
   /// <param name="items">State and province items</param>
   /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, long? countryId, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available languages
   /// </summary>
   /// <param name="items">Language items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available user roles
   /// </summary>
   /// <param name="items">User role items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareUserRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available email accounts
   /// </summary>
   /// <param name="items">Email account items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available time zones
   /// </summary>
   /// <param name="items">Time zone items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available currencies
   /// </summary>
   /// <param name="items">Currency items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareCurrenciesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available log levels
   /// </summary>
   /// <param name="items">Log level items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available load plugin modes
   /// </summary>
   /// <param name="items">Load plugin mode items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareLoadPluginModesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available plugin groups
   /// </summary>
   /// <param name="items">Plugin group items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PreparePluginGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available topic templates
   /// </summary>
   /// <param name="items">Topic template items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareTopicTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

   /// <summary>
   /// Prepare available GDPR request types
   /// </summary>
   /// <param name="items">Request type items</param>
   /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
   /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareGdprRequestTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
}