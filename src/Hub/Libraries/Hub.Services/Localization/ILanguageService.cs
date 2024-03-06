using Hub.Core.Domain.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Localization;

/// <summary>
/// Language service interface
/// </summary>
public partial interface ILanguageService
{
   /// <summary>
   /// Deletes a language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteLanguageAsync(Language language);

   /// <summary>
   /// Gets all languages
   /// </summary>
   /// <param name="showHidden">A value indicating whether to show hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the languages
   /// </returns>
   Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false);

   /// <summary>
   /// Gets a language
   /// </summary>
   /// <param name="languageId">Language identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language
   /// </returns>
   Task<Language> GetLanguageByIdAsync(long languageId);

   /// <summary>
   /// Inserts a language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertLanguageAsync(Language language);

   /// <summary>
   /// Updates a language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateLanguageAsync(Language language);

   /// <summary>
   /// Get 2 letter ISO language code
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>ISO language code</returns>
   string GetTwoLetterIsoLanguageName(Language language);
}