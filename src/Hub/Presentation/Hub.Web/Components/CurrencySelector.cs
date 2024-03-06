﻿using Hub.Core.Domain.Directory;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class CurrencySelectorViewComponent : AppViewComponent
{
   private readonly ICommonModelFactory _commonModelFactory;
   private readonly CurrencySettings _currencySettings;

   public CurrencySelectorViewComponent(ICommonModelFactory commonModelFactory, CurrencySettings currencySettings)
   {
      _commonModelFactory = commonModelFactory;
      _currencySettings = currencySettings;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!_currencySettings.DisplayCurrencySelector)
         return Content("");

      var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
      if (model.AvailableCurrencies.Count == 1)
         return Content("");

      return View(model);
   }
}
