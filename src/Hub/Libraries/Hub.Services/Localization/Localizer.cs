using Hub.Core.Domain.Localization;
using Hub.Data;
using Hub.Services.Localization.Extensions;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hub.Services.Localization;
#pragma warning disable CS1591

/// <summary>
/// Provides information about localizable entities
/// </summary>
public partial class Localizer : ILocalizer
{
   #region Fields

   private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
   private readonly IRepository<Language> _languageRepository;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public Localizer(IRepository<LocalizedProperty> localizedPropertyRepository, IRepository<Language> languageRepository)
   {
      _languageRepository = languageRepository;
      _localizedPropertyRepository = localizedPropertyRepository;
   }

   #endregion

   #region Save

   /// <summary>
   /// Update localized properties for entity
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entity">Localized entity</param>
   /// <param name="lang">Language</param>
   /// <returns>Async operation</returns>
   public virtual async Task SaveLocaleAsync<T>(T entity, Language lang) where T : BaseEntity
   {
      ArgumentNullException.ThrowIfNull(entity);

      await SaveLocaleAsync(new List<T> { entity }, lang);
   }


   /// <summary>
   /// Update localized properties for entities
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entities">Localized entities</param>
   /// <param name="lang">Language</param>
   /// <returns>Async operation</returns>
   public virtual async Task SaveLocaleAsync<T>(IList<T> entities, Language lang) where T : BaseEntity
   {
      if (entities is null)
         throw new ArgumentNullException(nameof(entities));

      if (!entities.Any())
         return;

      var properties = entities[0].GetLocalizablePropertyNames();
      var entityIds = entities.Select(x => x.Id);

      var localizedPropertyQuery =
         from p in _localizedPropertyRepository.Table
         where p.LocaleKeyGroup == typeof(T).Name && entityIds.Contains(p.EntityId)
         select p;

      var locales =
        (from l in _languageRepository.Table.AsNoTracking()
         join p in localizedPropertyQuery on l.Id equals p.LanguageId into llp
         from lp in llp.DefaultIfEmpty()
         select new { LanguageId = l.Id, LocalizedProperty = lp }).ToList();


      var toInsert = new List<LocalizedProperty>();
      var toUpdate = new List<LocalizedProperty>();

      foreach (var entity in entities)
      {
         foreach (var prop in properties)
         {
            var newPropertyValue = entity.GetType().GetProperty(prop).GetValue(entity) as string ?? string.Empty;

            var updatedLocale = locales.FirstOrDefault(x => x.LocalizedProperty?.LocaleKey == prop && x.LanguageId == lang.Id)?.LocalizedProperty;
            if (updatedLocale != null)
            {
               updatedLocale.LocaleValue = newPropertyValue;
               toUpdate.Add(updatedLocale);
            }
            else
            {
               toInsert.Add(new()
               {
                  EntityId = entity.Id,
                  LanguageId = lang.Id,
                  LocaleKeyGroup = typeof(T).Name,
                  LocaleKey = prop,
                  LocaleValue = newPropertyValue
               });
            }

            // we set locale values for all languages if they don't exist 
            var otherLangNotExistProperties = locales
               .Where(x => x.LanguageId != lang.Id && x.LocalizedProperty == default)
               .Select(x => new LocalizedProperty()
               {
                  EntityId = entity.Id,
                  LanguageId = x.LanguageId,
                  LocaleKeyGroup = typeof(T).Name,
                  LocaleKey = prop,
                  LocaleValue = newPropertyValue
               });

            toInsert.AddRange(otherLangNotExistProperties);
         }
      }

      await _localizedPropertyRepository.UpdateAsync(toUpdate);
      await _localizedPropertyRepository.InsertAsync(toInsert);
   }

   #endregion

   #region Delete

   /// <summary>
   /// Delete localized property value for the entity by the entity identifier
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entityId">Identifier of the entity for deleting</param>
   /// <param name="langId">Language identifier</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteLocaleAsync<T>(long entityId, long? langId = null) where T : BaseEntity
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(entityId, 1);

      await _localizedPropertyRepository.DeleteAsync(x => x.EntityId == entityId && x.LocaleKeyGroup == typeof(T).Name && (!langId.HasValue || x.LanguageId == langId));
   }

   /// <summary>
   /// Delete localized property value for the entity by the entity identifiers
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entityIds">Identifiers of entities for deleting</param>
   /// <param name="langId">Language identifier</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteLocaleAsync<T>(IEnumerable<long> entityIds, long? langId = null) where T : BaseEntity
   {
      ArgumentNullException.ThrowIfNull(entityIds);

      await _localizedPropertyRepository.DeleteAsync(x => entityIds.Contains(x.EntityId) && x.LocaleKeyGroup == typeof(T).Name && (!langId.HasValue || x.LanguageId == langId));
   }

   #endregion
}
#pragma warning restore CS1591