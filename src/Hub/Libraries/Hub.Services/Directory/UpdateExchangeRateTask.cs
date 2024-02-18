﻿using System;
using Hub.Core.Domain.Directory;
using Hub.Services.ScheduleTasks;

namespace Hub.Services.Directory;

/// <summary>
/// Represents a task for updating exchange rates
/// </summary>
public partial class UpdateExchangeRateTask : IScheduleTask
{
   #region Fields

   private readonly CurrencySettings _currencySettings;
   private readonly ICurrencyService _currencyService;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public UpdateExchangeRateTask(CurrencySettings currencySettings,
         ICurrencyService currencyService)
   {
      _currencySettings = currencySettings;
      _currencyService = currencyService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Executes a task
   /// </summary>
   public async System.Threading.Tasks.Task ExecuteAsync()
   {
      if (!_currencySettings.AutoUpdateEnabled)
         return;

      var exchangeRates = await _currencyService.GetCurrencyLiveRatesAsync();
      foreach (var exchangeRate in exchangeRates)
      {
         var currency = await _currencyService.GetCurrencyByCodeAsync(exchangeRate.CurrencyCode);
         if (currency == null)
            continue;

         currency.Rate = exchangeRate.Rate;
         currency.UpdatedOnUtc = DateTime.UtcNow;
         await _currencyService.UpdateCurrencyAsync(currency);
      }
   }

   #endregion
}