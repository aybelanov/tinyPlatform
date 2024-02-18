using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Caching;
using Hub.Core.Domain.Common;
using Hub.Data;
using Hub.Data.Extensions;

namespace Hub.Services.Common;

/// <summary>
/// Address attribute service
/// </summary>
public partial class AddressAttributeService : IAddressAttributeService
{
   #region Fields

   private readonly IRepository<AddressAttribute> _addressAttributeRepository;
   private readonly IRepository<AddressAttributeValue> _addressAttributeValueRepository;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public AddressAttributeService(IRepository<AddressAttribute> addressAttributeRepository,
       IRepository<AddressAttributeValue> addressAttributeValueRepository,
       IStaticCacheManager staticCacheManager)
   {
      _addressAttributeRepository = addressAttributeRepository;
      _addressAttributeValueRepository = addressAttributeValueRepository;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Deletes an address attribute
   /// </summary>
   /// <param name="addressAttribute">Address attribute</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAddressAttributeAsync(AddressAttribute addressAttribute)
   {
      await _addressAttributeRepository.DeleteAsync(addressAttribute);
   }

   /// <summary>
   /// Gets all address attributes
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the address attributes
   /// </returns>
   public virtual async Task<IList<AddressAttribute>> GetAllAddressAttributesAsync()
   {
      return await _addressAttributeRepository.GetAllAsync(query =>
      {
         return from aa in query
                orderby aa.DisplayOrder, aa.Id
                select aa;
      }, cache => default);
   }

   /// <summary>
   /// Gets an address attribute 
   /// </summary>
   /// <param name="addressAttributeId">Address attribute identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the address attribute
   /// </returns>
   public virtual async Task<AddressAttribute> GetAddressAttributeByIdAsync(long addressAttributeId)
   {
      return await _addressAttributeRepository.GetByIdAsync(addressAttributeId, cache => default);
   }

   /// <summary>
   /// Inserts an address attribute
   /// </summary>
   /// <param name="addressAttribute">Address attribute</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertAddressAttributeAsync(AddressAttribute addressAttribute)
   {
      await _addressAttributeRepository.InsertAsync(addressAttribute);
   }

   /// <summary>
   /// Updates the address attribute
   /// </summary>
   /// <param name="addressAttribute">Address attribute</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateAddressAttributeAsync(AddressAttribute addressAttribute)
   {
      await _addressAttributeRepository.UpdateAsync(addressAttribute);
   }

   /// <summary>
   /// Deletes an address attribute value
   /// </summary>
   /// <param name="addressAttributeValue">Address attribute value</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue)
   {
      await _addressAttributeValueRepository.DeleteAsync(addressAttributeValue);
   }

   /// <summary>
   /// Gets address attribute values by address attribute identifier
   /// </summary>
   /// <param name="addressAttributeId">The address attribute identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the address attribute values
   /// </returns>
   public virtual async Task<IList<AddressAttributeValue>> GetAddressAttributeValuesAsync(long addressAttributeId)
   {
      var key = _staticCacheManager.PrepareKeyForDefaultCache(HubCommonDefaults.AddressAttributeValuesByAttributeCacheKey, addressAttributeId);

      var query = from aav in _addressAttributeValueRepository.Table
                  orderby aav.DisplayOrder, aav.Id
                  where aav.AddressAttributeId == addressAttributeId
                  select aav;
      var addressAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

      return addressAttributeValues;
   }

   /// <summary>
   /// Gets an address attribute value
   /// </summary>
   /// <param name="addressAttributeValueId">Address attribute value identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the address attribute value
   /// </returns>
   public virtual async Task<AddressAttributeValue> GetAddressAttributeValueByIdAsync(long addressAttributeValueId)
   {
      return await _addressAttributeValueRepository.GetByIdAsync(addressAttributeValueId, cache => default);
   }

   /// <summary>
   /// Inserts an address attribute value
   /// </summary>
   /// <param name="addressAttributeValue">Address attribute value</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue)
   {
      await _addressAttributeValueRepository.InsertAsync(addressAttributeValue);
   }

   /// <summary>
   /// Updates the address attribute value
   /// </summary>
   /// <param name="addressAttributeValue">Address attribute value</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue)
   {
      await _addressAttributeValueRepository.UpdateAsync(addressAttributeValue);
   }

   #endregion
}