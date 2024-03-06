using Hub.Core;
using Hub.Core.Domain.Directory;
using Hub.Services.Configuration;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class CurrencyController : BaseAdminController
{
   #region Fields

   private readonly CurrencySettings _currencySettings;
   private readonly ICurrencyModelFactory _currencyModelFactory;
   private readonly ICurrencyService _currencyService;
   private readonly IUserActivityService _userActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly ILocalizedEntityService _localizedEntityService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly ISettingService _settingService;

   #endregion

   #region Ctor

   public CurrencyController(CurrencySettings currencySettings,
       ICurrencyModelFactory currencyModelFactory,
       ICurrencyService currencyService,
       IUserActivityService userActivityService,
       ILocalizationService localizationService,
       ILocalizedEntityService localizedEntityService,
       INotificationService notificationService,
       IPermissionService permissionService,
       ISettingService settingService)
   {
      _currencySettings = currencySettings;
      _currencyModelFactory = currencyModelFactory;
      _currencyService = currencyService;
      _userActivityService = userActivityService;
      _localizationService = localizationService;
      _localizedEntityService = localizedEntityService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _settingService = settingService;
   }

   #endregion

   #region Utilities

   protected virtual async Task UpdateLocalesAsync(Currency currency, CurrencyModel model)
   {
      foreach (var localized in model.Locales)
         await _localizedEntityService.SaveLocalizedValueAsync(currency, x => x.Name, localized.Name, localized.LanguageId);
   }

   #endregion

   #region Methods

   public virtual IActionResult Index()
   {
      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> List(bool liveRates = false)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      var model = new CurrencySearchModel();

      try
      {
         //prepare model
         model = await _currencyModelFactory.PrepareCurrencySearchModelAsync(model, liveRates);
      }
      catch (Exception e)
      {
         await _notificationService.ErrorNotificationAsync(e);
      }

      return View(model);
   }

   [HttpPost]
   [FormValueRequired("save")]
   public virtual async Task<IActionResult> List(CurrencySearchModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      _currencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProviderModel.ExchangeRateProvider;
      _currencySettings.AutoUpdateEnabled = model.ExchangeRateProviderModel.AutoUpdateEnabled;
      await _settingService.SaveSettingAsync(_currencySettings);

      return RedirectToAction("List", "Currency");
   }

   [HttpPost]
   public virtual async Task<IActionResult> ListGrid(CurrencySearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _currencyModelFactory.PrepareCurrencyListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ApplyRates(IEnumerable<CurrencyExchangeRateModel> rateModels)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      foreach (var rate in rateModels)
      {
         var currency = await _currencyService.GetCurrencyByCodeAsync(rate.CurrencyCode);
         if (currency == null)
            continue;

         currency.Rate = rate.Rate;
         currency.UpdatedOnUtc = DateTime.UtcNow;
         await _currencyService.UpdateCurrencyAsync(currency);
      }

      return Json(new { result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> MarkAsPrimaryExchangeRateCurrency(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      _currencySettings.PrimaryExchangeRateCurrencyId = id;
      await _settingService.SaveSettingAsync(_currencySettings);

      return Json(new { result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> MarkAsPrimaryPlatformCurrency(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      _currencySettings.PrimaryPlatformCurrencyId = id;
      await _settingService.SaveSettingAsync(_currencySettings);

      return Json(new { result = true });
   }

   #endregion

   #region Create / Edit / Delete

   public virtual async Task<IActionResult> Create()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      //prepare model
      var model = await _currencyModelFactory.PrepareCurrencyModelAsync(new CurrencyModel(), null);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> Create(CurrencyModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      if (ModelState.IsValid)
      {
         var currency = model.ToEntity<Currency>();
         currency.CreatedOnUtc = DateTime.UtcNow;
         currency.UpdatedOnUtc = DateTime.UtcNow;
         await _currencyService.InsertCurrencyAsync(currency);

         //activity log
         await _userActivityService.InsertActivityAsync("AddNewCurrency",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCurrency"), currency.Id), currency);

         //locales
         await UpdateLocalesAsync(currency, model);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Added"));

         if (!continueEditing)
            return RedirectToAction("List");

         return RedirectToAction("Edit", new { id = currency.Id });
      }

      //prepare model
      model = await _currencyModelFactory.PrepareCurrencyModelAsync(model, null, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> Edit(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      //try to get a currency with the specified id
      var currency = await _currencyService.GetCurrencyByIdAsync(id);
      if (currency == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _currencyModelFactory.PrepareCurrencyModelAsync(null, currency);

      return View(model);
   }

   [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
   public virtual async Task<IActionResult> Edit(CurrencyModel model, bool continueEditing)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      //try to get a currency with the specified id
      var currency = await _currencyService.GetCurrencyByIdAsync(model.Id);
      if (currency == null)
         return RedirectToAction("List");

      if (ModelState.IsValid)
      {
         //ensure we have at least one published currency
         var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
         if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id && !model.Published)
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
            return RedirectToAction("Edit", new { id = currency.Id });
         }

         currency = model.ToEntity(currency);
         currency.UpdatedOnUtc = DateTime.UtcNow;
         await _currencyService.UpdateCurrencyAsync(currency);

         //activity log
         await _userActivityService.InsertActivityAsync("EditCurrency",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCurrency"), currency.Id), currency);

         //locales
         await UpdateLocalesAsync(currency, model);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Updated"));

         if (!continueEditing)
            return RedirectToAction("List");

         return RedirectToAction("Edit", new { id = currency.Id });
      }

      //prepare model
      model = await _currencyModelFactory.PrepareCurrencyModelAsync(model, currency, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Delete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
         return AccessDeniedView();

      //try to get a currency with the specified id
      var currency = await _currencyService.GetCurrencyByIdAsync(id);
      if (currency == null)
         return RedirectToAction("List");

      try
      {
         if (currency.Id == _currencySettings.PrimaryPlatformCurrencyId)
            throw new AppException(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeletePrimary"));

         if (currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId)
            throw new AppException(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeleteExchange"));

         //ensure we have at least one published currency
         var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
         if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
         {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
            return RedirectToAction("Edit", new { id = currency.Id });
         }

         await _currencyService.DeleteCurrencyAsync(currency);

         //activity log
         await _userActivityService.InsertActivityAsync("DeleteCurrency",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCurrency"), currency.Id), currency);

         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Deleted"));

         return RedirectToAction("List");
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
         return RedirectToAction("Edit", new { id = currency.Id });
      }
   }

   #endregion
}