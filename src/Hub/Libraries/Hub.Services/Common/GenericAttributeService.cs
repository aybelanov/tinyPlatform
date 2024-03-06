using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Common;
using Hub.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Common;

/// <summary>
/// Generic attribute service
/// </summary>
public partial class GenericAttributeService : IGenericAttributeService
{
   #region Fields

   private readonly IRepository<GenericAttribute> _genericAttributeRepository;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="genericAttributeRepository"></param>
   /// <param name="staticCacheManager"></param>
   public GenericAttributeService(IRepository<GenericAttribute> genericAttributeRepository,
       IStaticCacheManager staticCacheManager)
   {
      _genericAttributeRepository = genericAttributeRepository;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Deletes an attribute
   /// </summary>
   /// <param name="attribute">Attribute</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAttributeAsync(GenericAttribute attribute)
   {
      await _genericAttributeRepository.DeleteAsync(attribute);
   }

   /// <summary>
   /// Deletes an attributes
   /// </summary>
   /// <param name="attributes">Attributes</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAttributesAsync(IList<GenericAttribute> attributes)
   {
      await _genericAttributeRepository.DeleteAsync(attributes);
   }

   /// <summary>
   /// Inserts an attribute
   /// </summary>
   /// <param name="attribute">attribute</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertAttributeAsync(GenericAttribute attribute)
   {
      ArgumentNullException.ThrowIfNull(attribute);

      attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow;

      await _genericAttributeRepository.InsertAsync(attribute);
   }

   /// <summary>
   /// Updates the attribute
   /// </summary>
   /// <param name="attribute">Attribute</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateAttributeAsync(GenericAttribute attribute)
   {
      if (attribute == null)
         throw new ArgumentNullException(nameof(attribute));

      attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow;

      await _genericAttributeRepository.UpdateAsync(attribute);
   }

   /// <summary>
   /// Get attributes
   /// </summary>
   /// <param name="entityId">Entity identifier</param>
   /// <param name="keyGroup">Key group</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the get attributes
   /// </returns>
   public virtual async Task<IList<GenericAttribute>> GetAttributesForEntityAsync(long entityId, string keyGroup)
   {
      var key = _staticCacheManager.PrepareKeyForShortTermCache(HubCommonDefaults.GenericAttributeCacheKey, entityId, keyGroup);

      var query = from ga in _genericAttributeRepository.Table.AsNoTracking()
                  where ga.EntityId == entityId && ga.KeyGroup == keyGroup
                  select ga;

      var attributes = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

      return attributes;
   }

   /// <summary>
   /// Save attribute value
   /// </summary>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="key">Key</param>
   /// <param name="value">Value</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SaveAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType value)
   {
      ArgumentNullException.ThrowIfNull(entity);
      ArgumentNullException.ThrowIfNull(key);

      var keyGroup = entity.GetType().Name;

      var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);

      var prop = props.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

      var valueStr = CommonHelper.To<string>(value);

      if (prop != null)
      {
         if (string.IsNullOrWhiteSpace(valueStr))
            //delete
            await DeleteAttributeAsync(prop);
         else
         {
            //update
            prop.Value = valueStr;
            await UpdateAttributeAsync(prop);
         }
      }
      else
      {
         if (string.IsNullOrWhiteSpace(valueStr))
            return;

         //insert
         prop = new GenericAttribute
         {
            EntityId = entity.Id,
            Key = key,
            KeyGroup = keyGroup,
            Value = valueStr,
         };

         await InsertAttributeAsync(prop);
      }
   }

   /// <summary>
   /// Get an attribute of an entity
   /// </summary>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="key">Key</param>
   /// <param name="defaultValue">Default value</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the attribute
   /// </returns>
   public virtual async Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType defaultValue = default)
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      var keyGroup = entity.GetType().Name;

      var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);

      //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
      if (props == null)
         return defaultValue;

      props = props.ToList();
      if (!props.Any())
         return defaultValue;

      var prop = props.FirstOrDefault(ga =>
          ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

      if (prop == null || string.IsNullOrEmpty(prop.Value))
         return defaultValue;

      return CommonHelper.To<TPropType>(prop.Value);
   }

   /// <summary>
   /// Get an attribute of an entity
   /// </summary>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <param name="entityId">Entity identifier</param>
   /// <param name="key">Key</param>
   /// <param name="defaultValue">Default value</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the attribute
   /// </returns>
   public virtual async Task<TPropType> GetAttributeAsync<TEntity, TPropType>(long entityId, string key, TPropType defaultValue = default)
       where TEntity : BaseEntity
   {
      var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
      entity.Id = entityId;

      return await GetAttributeAsync(entity, key, defaultValue);
   }

   #endregion
}