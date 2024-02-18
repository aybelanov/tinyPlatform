using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Services.Caching;
using Hub.Services.Events;

namespace Hub.Services.Users.Caching;

/// <summary>
/// Represents a user cache event consumer
/// </summary>
public partial class UserCacheEventConsumer : CacheEventConsumer<User>, IConsumer<UserPasswordChangedEvent>
{
   #region Methods

   ///// <summary>
   ///// Clear cache data
   ///// </summary>
   ///// <param name="entity">Entity</param>
   ///// <returns>A task that represents the asynchronous operation</returns>
   //protected override async Task ClearCacheAsync(User entity, EntityEventType eventType)
   //{
   //   await RemoveByPrefixAsync(AppUserServicesDefaults.UserUserRolesByUserPrefix, entity);
   //   await RemoveByPrefixAsync(AppUserServicesDefaults.UserAddressesByUserPrefix, entity);
   //   await RemoveAsync(AppUserServicesDefaults.UserByGuidCacheKey, entity.UserGuid);
   //   await RemoveAsync(AppUserServicesDefaults.UserByEmailCacheKey, entity.Email);

   //   if (!string.IsNullOrEmpty(entity.Username))
   //      await RemoveAsync(AppUserServicesDefaults.UserByUsernameCacheKey, entity.Username);

   //   if (!string.IsNullOrEmpty(entity.SystemName))
   //      await RemoveAsync(AppUserServicesDefaults.UserBySystemNameCacheKey, entity.SystemName);

   //   await base.ClearCacheAsync(entity, eventType); 
   //}

   /// <summary>
   /// Handle password changed event
   /// </summary>
   /// <param name="eventMessage">Event message</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task HandleEventAsync(UserPasswordChangedEvent eventMessage)
   {
      await RemoveAsync(AppUserServicesDefaults.UserPasswordLifetimeCacheKey, eventMessage.Password.UserId);
   }

   /// <summary>
   /// Clear cache by entity event type
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(User entity, EntityEventType entityEventType)
   {
      if (entityEventType == EntityEventType.Delete)
      {
         await RemoveAsync(AppUserServicesDefaults.UserAddressesCacheKey, entity);
         await RemoveByPrefixAsync(AppUserServicesDefaults.UserAddressesByUserPrefix, entity);
      }

      await base.ClearCacheAsync(entity, entityEventType);
   }

   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(User entity)
   {
      await RemoveByPrefixAsync(AppUserServicesDefaults.UserUserRolesByUserPrefix, entity);
      await RemoveAsync(AppUserServicesDefaults.UserByGuidCacheKey, entity.UserGuid);
      await RemoveAsync(AppUserServicesDefaults.UserByEmailCacheKey, entity.Email);

      if (!string.IsNullOrEmpty(entity.Username))
         await RemoveAsync(AppUserServicesDefaults.UserByUsernameCacheKey, entity.Username);

      if (string.IsNullOrEmpty(entity.SystemName))
         return;

      await RemoveAsync(AppUserServicesDefaults.UserBySystemNameCacheKey, entity.SystemName);
   }

   #endregion
}