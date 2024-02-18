using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Data.Extensions;
using Hub.Services.Localization;
using Hub.Web.Framework.Models;

namespace Hub.Web.Framework.Factories;

/// <summary>
/// Represents the base localized model factory implementation
/// </summary>
public partial class LocalizedModelFactory : ILocalizedModelFactory
{
   #region Fields

   private readonly ILanguageService _languageService;

   #endregion

   #region Ctor

   /// <summary>IoC Ctor</summary>
   public LocalizedModelFactory(ILanguageService languageService)
   {
      _languageService = languageService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare localized model for localizable entities
   /// </summary>
   /// <typeparam name="T">Localized model type</typeparam>
   /// <param name="configure">Model configuration action</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of localized model
   /// </returns>
   public virtual async Task<IList<T>> PrepareLocalizedModelsAsync<T>(Func<T, long, Task> configure = null) where T : ILocalizedLocaleModel
   {
      //get all available languages
      var availableLanguages = await _languageService.GetAllLanguagesAsync(true);

      //prepare models
      var localizedModels = await availableLanguages.SelectAwait(async language =>
      {
         //create localized model
         var localizedModel = Activator.CreateInstance<T>();

         //set language
         localizedModel.LanguageId = language.Id;

         //invoke the model configuration action
         if (configure != null)
            await configure(localizedModel, localizedModel.LanguageId);

         return localizedModel;
      }).ToListAsync();

      return localizedModels;
   }

   #endregion
}