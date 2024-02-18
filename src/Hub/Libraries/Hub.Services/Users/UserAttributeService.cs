using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Caching;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Data.Extensions;

namespace Hub.Services.Users
{
   /// <summary>
   /// User attribute service
   /// </summary>
   public partial class UserAttributeService : IUserAttributeService
   {
      #region Fields

      private readonly IRepository<UserAttribute> _userAttributeRepository;
      private readonly IRepository<UserAttributeValue> _userAttributeValueRepository;
      private readonly IStaticCacheManager _staticCacheManager;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public UserAttributeService(IRepository<UserAttribute> userAttributeRepository,
            IRepository<UserAttributeValue> userAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
      {
         _userAttributeRepository = userAttributeRepository;
         _userAttributeValueRepository = userAttributeValueRepository;
         _staticCacheManager = staticCacheManager;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Deletes a user attribute
      /// </summary>
      /// <param name="userAttribute">User attribute</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeleteUserAttributeAsync(UserAttribute userAttribute)
      {
         await _userAttributeRepository.DeleteAsync(userAttribute);
      }

      /// <summary>
      /// Gets all user attributes
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attributes
      /// </returns>
      public virtual async Task<IList<UserAttribute>> GetAllUserAttributesAsync()
      {
         return await _userAttributeRepository.GetAllAsync(query =>
         {
            return from ca in query
                   orderby ca.DisplayOrder, ca.Id
                   select ca;
         }, cache => default);
      }

      /// <summary>
      /// Gets a user attribute 
      /// </summary>
      /// <param name="userAttributeId">User attribute identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute
      /// </returns>
      public virtual async Task<UserAttribute> GetUserAttributeByIdAsync(long userAttributeId)
      {
         return await _userAttributeRepository.GetByIdAsync(userAttributeId, cache => default);
      }

      /// <summary>
      /// Inserts a user attribute
      /// </summary>
      /// <param name="userAttribute">User attribute</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertUserAttributeAsync(UserAttribute userAttribute)
      {
         await _userAttributeRepository.InsertAsync(userAttribute);
      }

      /// <summary>
      /// Updates the user attribute
      /// </summary>
      /// <param name="userAttribute">User attribute</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdateUserAttributeAsync(UserAttribute userAttribute)
      {
         await _userAttributeRepository.UpdateAsync(userAttribute);
      }

      /// <summary>
      /// Deletes a user attribute value
      /// </summary>
      /// <param name="userAttributeValue">User attribute value</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeleteUserAttributeValueAsync(UserAttributeValue userAttributeValue)
      {
         await _userAttributeValueRepository.DeleteAsync(userAttributeValue);
      }

      /// <summary>
      /// Gets user attribute values by user attribute identifier
      /// </summary>
      /// <param name="userAttributeId">The user attribute identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute values
      /// </returns>
      public virtual async Task<IList<UserAttributeValue>> GetUserAttributeValuesAsync(long userAttributeId)
      {
         var key = _staticCacheManager.PrepareKeyForDefaultCache(AppUserServicesDefaults.UserAttributeValuesByAttributeCacheKey, userAttributeId);

         var query = from cav in _userAttributeValueRepository.Table
                     orderby cav.DisplayOrder, cav.Id
                     where cav.UserAttributeId == userAttributeId
                     select cav;

         var userAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

         return userAttributeValues;
      }

      /// <summary>
      /// Gets a user attribute value
      /// </summary>
      /// <param name="userAttributeValueId">User attribute value identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute value
      /// </returns>
      public virtual async Task<UserAttributeValue> GetUserAttributeValueByIdAsync(long userAttributeValueId)
      {
         return await _userAttributeValueRepository.GetByIdAsync(userAttributeValueId, cache => default);
      }

      /// <summary>
      /// Inserts a user attribute value
      /// </summary>
      /// <param name="userAttributeValue">User attribute value</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertUserAttributeValueAsync(UserAttributeValue userAttributeValue)
      {
         await _userAttributeValueRepository.InsertAsync(userAttributeValue);
      }

      /// <summary>
      /// Updates the user attribute value
      /// </summary>
      /// <param name="userAttributeValue">User attribute value</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdateUserAttributeValueAsync(UserAttributeValue userAttributeValue)
      {
         await _userAttributeValueRepository.UpdateAsync(userAttributeValue);
      }

      #endregion
   }
}