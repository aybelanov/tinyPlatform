using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Localization;
using Hub.Data;
using Hub.Data.Extensions;
//using Hub.Data.Extensions;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Localization;
using Hub.Web.Framework.Models.Extensions;
//using Hub.Web.Framework.Models.Extensions;
//using Microsoft.EntityFrameworkCore;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the language model factory implementation
/// </summary>
public partial class LanguageModelFactory : ILanguageModelFactory
{
   #region Fields

   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;

   #endregion

   #region Ctor

   public LanguageModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
       ILanguageService languageService,
       ILocalizationService localizationService)
   {
      _baseAdminModelFactory = baseAdminModelFactory;
      _languageService = languageService;
      _localizationService = localizationService;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare locale resource search model
   /// </summary>
   /// <param name="searchModel">Locale resource search model</param>
   /// <param name="language">Language</param>
   /// <returns>Locale resource search model</returns>
   protected virtual LocaleResourceSearchModel PrepareLocaleResourceSearchModel(LocaleResourceSearchModel searchModel, Language language)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (language == null)
         throw new ArgumentNullException(nameof(language));

      searchModel.LanguageId = language.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare language search model
   /// </summary>
   /// <param name="searchModel">Language search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language search model
   /// </returns>
   public virtual Task<LanguageSearchModel> PrepareLanguageSearchModelAsync(LanguageSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged language list model
   /// </summary>
   /// <param name="searchModel">Language search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language list model
   /// </returns>
   public virtual async Task<LanguageListModel> PrepareLanguageListModelAsync(LanguageSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get languages
      var languages = (await _languageService.GetAllLanguagesAsync(showHidden: true)).ToPagedList(searchModel);

      //prepare list model
      var model = new LanguageListModel().PrepareToGrid(searchModel, languages, () =>
      {
         return languages.Select(language => language.ToModel<LanguageModel>());
      });

      return model;
   }

   /// <summary>
   /// Prepare language model
   /// </summary>
   /// <param name="model">Language model</param>
   /// <param name="language">Language</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language model
   /// </returns>
   public virtual async Task<LanguageModel> PrepareLanguageModelAsync(LanguageModel model, Language language, bool excludeProperties = false)
   {
      if (language != null)
      {
         //fill in model values from the entity
         model ??= language.ToModel<LanguageModel>();

         //prepare nested search model
         PrepareLocaleResourceSearchModel(model.LocaleResourceSearchModel, language);
      }

      //set default values for the new model
      if (language == null)
      {
         model.DisplayOrder = (await _languageService.GetAllLanguagesAsync()).Max(l => l.DisplayOrder) + 1;
         model.Published = true;
      }

      //prepare available currencies
      await _baseAdminModelFactory.PrepareCurrenciesAsync(model.AvailableCurrencies,
          defaultItemText: await _localizationService.GetResourceAsync("Admin.Common.EmptyItemText"));

      return model;
   }

   /// <summary>
   /// Prepare paged locale resource list model
   /// </summary>
   /// <param name="searchModel">Locale resource search model</param>
   /// <param name="language">Language</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the locale resource list model
   /// </returns>
   public virtual async Task<LocaleResourceListModel> PrepareLocaleResourceListModelAsync(LocaleResourceSearchModel searchModel, Language language)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (language == null)
         throw new ArgumentNullException(nameof(language));

      //get locale resources
      var localeResources = (await _localizationService.GetAllResourceValuesAsync(language.Id, loadPublicLocales: null))//.ToList()
          .OrderBy(localeResource => localeResource.Key).AsQueryable();

      //var localeResources =
      //    (from s in _localeStringRepository.Table
      //    where s.LanguageId == language.Id
      //    orderby s.ResourceName
      //    group s by s.ResourceName into locale
      //    select new
      //    {
      //       Key = locale.Key,
      //       Value = from l in locale.DefaultIfEmpty(new() { Id= 0, ResourceValue = string.Empty })
      //               where l.ResourceValue != null
      //               orderby l.LanguageId
      //               select new { Key = l.LanguageId, Value = l.ResourceValue }
      //    });
      //.ToDictionary(k => k.Key, v => new KeyValuePair<long, string>(v.Value.First().Key, v.Value.First().Value))
      //.AsQueryable();

      //filter locale resources
      if (!string.IsNullOrEmpty(searchModel.SearchResourceName))
         localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(searchModel.SearchResourceName.ToLowerInvariant()));
      if (!string.IsNullOrEmpty(searchModel.SearchResourceValue))
         localeResources = localeResources.Where(l => l.Value.Value.ToLowerInvariant().Contains(searchModel.SearchResourceValue.ToLowerInvariant()));

      var pagedLocaleResources = localeResources.ToPagedList(searchModel.Page - 1, searchModel.PageSize);

      //prepare list model
      var model = new LocaleResourceListModel().PrepareToGrid(searchModel, pagedLocaleResources, () =>
      {
         //fill in model values from the entity
         return pagedLocaleResources.Select(localeResource => new LocaleResourceModel
         {
            LanguageId = language.Id,
            Id = localeResource.Value.Key,
            ResourceName = localeResource.Key,
            ResourceValue = localeResource.Value.Value
         });
      });

      return model;
   }

   #endregion
}