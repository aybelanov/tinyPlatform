using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Caching;
using Hub.Core.Domain.Localization;
using Hub.Data;
using Hub.Services.Configuration;

namespace Hub.Services.Localization;

/// <summary>
/// Language service
/// </summary>
public partial class LanguageService : ILanguageService
{
   #region Fields

   private readonly IRepository<Language> _languageRepository;
   private readonly ISettingService _settingService;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly LocalizationSettings _localizationSettings;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public LanguageService(IRepository<Language> languageRepository,
       ISettingService settingService,
       IStaticCacheManager staticCacheManager,
       LocalizationSettings localizationSettings)
   {
      _languageRepository = languageRepository;
      _settingService = settingService;
      _staticCacheManager = staticCacheManager;
      _localizationSettings = localizationSettings;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Deletes a language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteLanguageAsync(Language language)
   {
      if (language == null)
         throw new ArgumentNullException(nameof(language));

      //update default admin area language (if required)
      if (_localizationSettings.DefaultAdminLanguageId == language.Id)
         foreach (var activeLanguage in await GetAllLanguagesAsync())
         {
            if (activeLanguage.Id == language.Id)
               continue;

            _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
            await _settingService.SaveSettingAsync(_localizationSettings);
            break;
         }

      await _languageRepository.DeleteAsync(language);
   }

   /// <summary>
   /// Gets all languages
   /// </summary>
   /// <param name="showHidden">A value indicating whether to show hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the languages
   /// </returns>
   public virtual async Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false)
   {
      //cacheable copy
      var key = _staticCacheManager.PrepareKeyForDefaultCache(AppLocalizationDefaults.LanguagesAllCacheKey, showHidden);

      var languages = await _staticCacheManager.GetAsync(key, async () =>
      {
         var allLanguages = await _languageRepository.GetAllAsync(query =>
            {
               if (!showHidden)
                  query = query.Where(l => l.Published);
               query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

               return query;
            });

         return allLanguages;
      });

      return languages;
   }

   /// <summary>
   /// Gets a language
   /// </summary>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language
   /// </returns>
   public virtual async Task<Language> GetLanguageByIdAsync(long languageId)
   {
      return await _languageRepository.GetByIdAsync(languageId, cache => default);
   }

   /// <summary>
   /// Inserts a language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertLanguageAsync(Language language)
   {
      await _languageRepository.InsertAsync(language);
   }

   /// <summary>
   /// Updates a language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateLanguageAsync(Language language)
   {
      //update language
      await _languageRepository.UpdateAsync(language);
   }

   /// <summary>
   /// Get 2 letter ISO language code
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>ISO language code</returns>
   public virtual string GetTwoLetterIsoLanguageName(Language language)
   {
      if (language == null)
         throw new ArgumentNullException(nameof(language));

      if (string.IsNullOrEmpty(language.LanguageCulture))
         return "en";

      var culture = new CultureInfo(language.LanguageCulture);
      var code = culture.TwoLetterISOLanguageName;

      return string.IsNullOrEmpty(code) ? "en" : code;
   }

   #endregion
}