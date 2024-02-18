using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Localization;
using Shared.Common;

namespace Hub.Services.Localization;

/// <summary>
/// Localized entity service interface
/// </summary>
public partial interface ILocalizedEntityService
{
   /// <summary>
   /// Find localized value
   /// </summary>
   /// <param name="languageId">Language identifier</param>
   /// <param name="entityId">Entity identifier</param>
   /// <param name="localeKeyGroup">Locale key group</param>
   /// <param name="localeKey">Locale key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the found localized value
   /// </returns>
   Task<string> GetLocalizedValueAsync(long languageId, long entityId, string localeKeyGroup, string localeKey);

   /// <summary>
   /// Save localized value
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="keySelector">Key selector</param>
   /// <param name="localeValue">Locale value</param>
   /// <param name="languageId">Language ID</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SaveLocalizedValueAsync<T>(T entity,
       Expression<Func<T, string>> keySelector,
       string localeValue,
       long languageId) where T : BaseEntity, ILocalizedEntity;

   /// <summary>
   /// Save localized value
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="keySelector">Key selector</param>
   /// <param name="localeValue">Locale value</param>
   /// <param name="languageId">Language ID</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SaveLocalizedValueAsync<T, TPropType>(T entity,
      Expression<Func<T, TPropType>> keySelector,
      TPropType localeValue,
      long languageId) where T : BaseEntity, ILocalizedEntity;
}
