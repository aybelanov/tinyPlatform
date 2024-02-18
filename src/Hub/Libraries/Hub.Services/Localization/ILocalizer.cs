using Hub.Core.Domain.Localization;
using Shared.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Localization;

/// <summary>
/// Localized entity service interface.
/// It should use with cahched function method into CRUD entity methods 
/// </summary>
public partial interface ILocalizer
{
   /// <summary>
   /// Update localized properties for entity
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entity">Localized entity</param>
   /// <param name="lang">Language</param>
   /// <returns>Async operation</returns>
   Task SaveLocaleAsync<T>(T entity, Language lang) where T : BaseEntity;

   /// <summary>
   /// Update localized properties for entities
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entities">Localized entities</param>
   /// <param name="lang">Language</param>
   /// <returns>Async operation</returns>
   Task SaveLocaleAsync<T>(IList<T> entities, Language lang) where T : BaseEntity;

   /// <summary>
   /// Delete localized property value for the entity by the entity identifier
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entityId">Identifier of the entity for deleting</param>
   /// <param name="langId">Language identifier</param>
   /// <returns>Async operation</returns>
   Task DeleteLocaleAsync<T>(long entityId, long? langId = null) where T : BaseEntity;

   /// <summary>
   /// Delete localized property value for the entity by the entity identifiers
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <param name="entityIds">Identifiers of entities for deleting</param>
   /// <param name="langId">Language identifier</param>
   /// <returns>Async operation</returns>
   Task DeleteLocaleAsync<T>(IEnumerable<long> entityIds, long? langId = null) where T : BaseEntity;
}
