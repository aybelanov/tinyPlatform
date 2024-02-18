﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Models.Directory;
using Hub.Core;
using Hub.Services.Directory;
using Hub.Services.Localization;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the country model factory
/// </summary>
public partial class CountryModelFactory : ICountryModelFactory
{
   #region Fields

   private readonly ICountryService _countryService;
   private readonly ILocalizationService _localizationService;
   private readonly IStateProvinceService _stateProvinceService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public CountryModelFactory(ICountryService countryService,
       ILocalizationService localizationService,
       IStateProvinceService stateProvinceService,
       IWorkContext workContext)
   {
      _countryService = countryService;
      _localizationService = localizationService;
      _stateProvinceService = stateProvinceService;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Get states and provinces by country identifier
   /// </summary>
   /// <param name="countryId">Country identifier</param>
   /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of identifiers and names of states and provinces
   /// </returns>
   public virtual async Task<IList<StateProvinceModel>> GetStatesByCountryIdAsync(string countryId, bool addSelectStateItem)
   {
      if (string.IsNullOrEmpty(countryId))
         throw new ArgumentNullException(nameof(countryId));

      var country = await _countryService.GetCountryByIdAsync(Convert.ToInt32(countryId));
      var states = (await _stateProvinceService
          .GetStateProvincesByCountryIdAsync(country?.Id ?? 0, (await _workContext.GetWorkingLanguageAsync()).Id))
          .ToList();
      var result = new List<StateProvinceModel>();
      foreach (var state in states)
         result.Add(new StateProvinceModel
         {
            id = state.Id,
            name = await _localizationService.GetLocalizedAsync(state, x => x.Name)
         });

      if (country == null)
         //country is not selected ("choose country" item)
         if (addSelectStateItem)
            result.Insert(0, new StateProvinceModel
            {
               id = 0,
               name = await _localizationService.GetResourceAsync("Address.SelectState")
            });
         else
            result.Insert(0, new StateProvinceModel
            {
               id = 0,
               name = await _localizationService.GetResourceAsync("Address.Other")
            });
      else
         //some country is selected
         if (!result.Any())
         //country does not have states
         result.Insert(0, new StateProvinceModel
         {
            id = 0,
            name = await _localizationService.GetResourceAsync("Address.Other")
         });
      else
            //country has some states
            if (addSelectStateItem)
         result.Insert(0, new StateProvinceModel
         {
            id = 0,
            name = await _localizationService.GetResourceAsync("Address.SelectState")
         });

      return result;
   }

   #endregion
}
