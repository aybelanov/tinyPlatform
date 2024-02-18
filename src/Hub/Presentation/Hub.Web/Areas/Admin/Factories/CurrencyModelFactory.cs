﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Models.Directory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Core.Domain.Directory;
using Hub.Services.Directory;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the currency model factory implementation
   /// </summary>
   public partial class CurrencyModelFactory : ICurrencyModelFactory
   {
      #region Fields

      private readonly CurrencySettings _currencySettings;
      private readonly ICurrencyService _currencyService;
      private readonly IDateTimeHelper _dateTimeHelper;
      private readonly IExchangeRatePluginManager _exchangeRatePluginManager;
      private readonly ILocalizationService _localizationService;
      private readonly ILocalizedModelFactory _localizedModelFactory;

      #endregion

      #region Ctor

      public CurrencyModelFactory(CurrencySettings currencySettings,
          ICurrencyService currencyService,
          IDateTimeHelper dateTimeHelper,
          IExchangeRatePluginManager exchangeRatePluginManager,
          ILocalizationService localizationService,
          ILocalizedModelFactory localizedModelFactory)
      {
         _currencySettings = currencySettings;
         _currencyService = currencyService;
         _dateTimeHelper = dateTimeHelper;
         _exchangeRatePluginManager = exchangeRatePluginManager;
         _localizationService = localizationService;
         _localizedModelFactory = localizedModelFactory;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Prepare exchange rate provider model
      /// </summary>
      /// <param name="model">Currency exchange rate provider model</param>
      /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected virtual async Task PrepareExchangeRateProviderModelAsync(CurrencyExchangeRateProviderModel model, bool prepareExchangeRates)
      {
         if (model == null)
            throw new ArgumentNullException(nameof(model));

         model.AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled;

         //prepare available exchange rate providers
         var availableExchangeRateProviders = await _exchangeRatePluginManager.LoadAllPluginsAsync();

         model.ExchangeRateProviders = availableExchangeRateProviders.Select(provider => new SelectListItem
         {
            Text = provider.PluginDescriptor.FriendlyName,
            Value = provider.PluginDescriptor.SystemName,
            Selected = _exchangeRatePluginManager.IsPluginActive(provider)
         }).ToList();

         //prepare exchange rates
         if (prepareExchangeRates)
            await PrepareExchangeRateModelsAsync(model.ExchangeRates);
      }

      /// <summary>
      /// Prepare exchange rate models
      /// </summary>
      /// <param name="models">List of currency exchange rate model</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected virtual async Task PrepareExchangeRateModelsAsync(IList<CurrencyExchangeRateModel> models)
      {
         if (models == null)
            throw new ArgumentNullException(nameof(models));

         //get exchange rates
         var exchangeRates = await _currencyService.GetCurrencyLiveRatesAsync();

         //filter by existing currencies
         var currencies = await _currencyService.GetAllCurrenciesAsync(true);
         exchangeRates = exchangeRates
             .Where(rate => currencies
                 .Any(currency => currency.CurrencyCode.Equals(rate.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))).ToList();

         //prepare models
         foreach (var rate in exchangeRates)
            models.Add(new CurrencyExchangeRateModel { CurrencyCode = rate.CurrencyCode, Rate = rate.Rate });
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare currency search model
      /// </summary>
      /// <param name="searchModel">Currency search model</param>
      /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the currency search model
      /// </returns>
      public virtual async Task<CurrencySearchModel> PrepareCurrencySearchModelAsync(CurrencySearchModel searchModel, bool prepareExchangeRates = false)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare exchange rate provider model
         await PrepareExchangeRateProviderModelAsync(searchModel.ExchangeRateProviderModel, prepareExchangeRates);

         //prepare page parameters
         searchModel.SetGridPageSize();

         return searchModel;
      }

      /// <summary>
      /// Prepare paged currency list model
      /// </summary>
      /// <param name="searchModel">Currency search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the currency list model
      /// </returns>
      public virtual async Task<CurrencyListModel> PrepareCurrencyListModelAsync(CurrencySearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get currencies
         var currencies = (await _currencyService.GetAllCurrenciesAsync(showHidden: true)).ToPagedList(searchModel);

         //prepare list model
         var model = new CurrencyListModel().PrepareToGrid(searchModel, currencies, () =>
         {
            return currencies.Select(currency =>
               {
                  //fill in model values from the entity
                  var currencyModel = currency.ToModel<CurrencyModel>();

                  //fill in additional values (not existing in the entity)
                  currencyModel.IsPrimaryExchangeRateCurrency = currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId;
                  currencyModel.IsPrimaryPlatformCurrency = currency.Id == _currencySettings.PrimaryPlatformCurrencyId;

                  return currencyModel;
               });
         });

         return model;
      }

      /// <summary>
      /// Prepare currency model
      /// </summary>
      /// <param name="model">Currency model</param>
      /// <param name="currency">Currency</param>
      /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the currency model
      /// </returns>
      public virtual async Task<CurrencyModel> PrepareCurrencyModelAsync(CurrencyModel model, Currency currency, bool excludeProperties = false)
      {
         Func<CurrencyLocalizedModel, long, Task> localizedModelConfiguration = null;

         if (currency != null)
         {
            //fill in model values from the entity
            model ??= currency.ToModel<CurrencyModel>();

            //convert dates to the user time
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(currency.CreatedOnUtc, DateTimeKind.Utc);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
               locale.Name = await _localizationService.GetLocalizedAsync(currency, entity => entity.Name, languageId, false, false);
            };
         }

         //set default values for the new model
         if (currency == null)
         {
            model.Published = true;
            model.Rate = 1;
         }

         //prepare localized models
         if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

         return model;
      }

      #endregion
   }
}