using Hub.Core.Caching;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Services.Events;
using Shared.Common;
using System.Threading.Tasks;

namespace Hub.Services.Caching
{
   /// <summary>
   /// Represents the base entity cache event consumer
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   public abstract partial class CacheEventConsumer<TEntity> :
       IConsumer<EntityInsertedEvent<TEntity>>,
       IConsumer<EntityUpdatedEvent<TEntity>>,
       IConsumer<EntityDeletedEvent<TEntity>>
       where TEntity : BaseEntity
   {
      #region Fields

      /// <summary>
      /// Cache manager
      /// </summary>
      protected readonly IStaticCacheManager _staticCacheManager;

      #endregion

      #region Ctor

      /// <summary>
      /// Default Ctor
      /// </summary>
      protected CacheEventConsumer()
      {
         _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Clear cache by entity event type
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <param name="entityEventType">Entity event type</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected virtual async Task ClearCacheAsync(TEntity entity, EntityEventType entityEventType)
      {
         await RemoveByPrefixAsync(AppEntityCacheDefaults<TEntity>.ByIdsPrefix);
         await RemoveByPrefixAsync(AppEntityCacheDefaults<TEntity>.AllPrefix);

         if (entityEventType != EntityEventType.Insert)
            await RemoveAsync(AppEntityCacheDefaults<TEntity>.ByIdCacheKey, entity);

         await ClearCacheAsync(entity);
      }

      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected virtual Task ClearCacheAsync(TEntity entity)
      {
         return Task.CompletedTask;
      }

      /// <summary>
      /// Removes items by cache key prefix
      /// </summary>
      /// <param name="prefix">Cache key prefix</param>
      /// <param name="prefixParameters">Parameters to create cache key prefix</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected virtual async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
      {
         await _staticCacheManager.RemoveByPrefixAsync(prefix, prefixParameters);
      }

      /// <summary>
      /// Remove the value with the specified key from the cache
      /// </summary>
      /// <param name="cacheKey">Cache key</param>
      /// <param name="cacheKeyParameters">Parameters to create cache key</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
      {
         await _staticCacheManager.RemoveAsync(cacheKey, cacheKeyParameters);
      }

      #endregion

      #region Methods

      /// <summary>
      /// Handle entity inserted event
      /// </summary>
      /// <param name="eventMessage">Event message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
      {
         await ClearCacheAsync(eventMessage.Entity, EntityEventType.Insert);
      }

      /// <summary>
      /// Handle entity updated event
      /// </summary>
      /// <param name="eventMessage">Event message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
      {
         await ClearCacheAsync(eventMessage.Entity, EntityEventType.Update);
      }

      /// <summary>
      /// Handle entity deleted event
      /// </summary>
      /// <param name="eventMessage">Event message</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
      {
         await ClearCacheAsync(eventMessage.Entity, EntityEventType.Delete);
      }

      #endregion

      #region Nested

      /// <summary>
      /// Entity event type 
      /// </summary>
      protected enum EntityEventType
      {
         /// <summary>
         /// Insert event
         /// </summary>
         Insert,

         /// <summary>
         /// Update event
         /// </summary>
         Update,

         /// <summary>
         /// Delete event
         /// </summary>
         Delete
      }

      #endregion
   }
}