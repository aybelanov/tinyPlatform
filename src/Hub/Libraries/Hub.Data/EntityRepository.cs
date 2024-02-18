using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Events;
using Hub.Data.Extensions;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using static Grpc.Core.Metadata;

namespace Hub.Data;

/// <summary>
/// Represents the entity repository implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
   #region Fields

   private readonly IEventPublisher _eventPublisher;
   private readonly AppDbContext _dataProvider;
   private readonly IStaticCacheManager _staticCacheManager;

   private IQueryable<TEntity> _table;
   private bool _disposed = false;
   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="eventPublisher"></param>
   /// <param name="dataProvider"></param>
   /// <param name="staticCacheManager"></param>
   public EntityRepository(IEventPublisher eventPublisher,
       AppDbContext dataProvider,
       IStaticCacheManager staticCacheManager)
   {
      _eventPublisher = eventPublisher;
      _dataProvider = dataProvider;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="getAllAsync">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, CacheKey> getCacheKey)
   {
      if (getCacheKey == null)
         return await getAllAsync();

      //caching
      var cacheKey = getCacheKey(_staticCacheManager)
                     ?? _staticCacheManager.PrepareKeyForDefaultCache(AppEntityCacheDefaults<TEntity>.AllCacheKey);
      return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
   }

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="getAll">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>Entity entries</returns>
   protected virtual IList<TEntity> GetEntities(Func<IList<TEntity>> getAll, Func<IStaticCacheManager, CacheKey> getCacheKey)
   {
      if (getCacheKey == null)
         return getAll();

      //caching
      var cacheKey = getCacheKey(_staticCacheManager)
                     ?? _staticCacheManager.PrepareKeyForDefaultCache(AppEntityCacheDefaults<TEntity>.AllCacheKey);

      return _staticCacheManager.Get(cacheKey, getAll);
   }

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="getAllAsync">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, Task<CacheKey>> getCacheKey)
   {
      if (getCacheKey == null)
         return await getAllAsync();

      //caching
      var cacheKey = await getCacheKey(_staticCacheManager)
                     ?? _staticCacheManager.PrepareKeyForDefaultCache(AppEntityCacheDefaults<TEntity>.AllCacheKey);
      return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
   }

   #endregion

   #region Methods

   /// <summary>
   /// Get the entity entry
   /// </summary>
   /// <param name="id">Entity entry identifier</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entry
   /// </returns>
   public virtual async Task<TEntity> GetByIdAsync(long? id, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
   {
      if (!id.HasValue || id == 0)
         return null;

      async Task<TEntity> getEntityAsync()
      {
         return await Table.FirstOrDefaultAsync(entity => entity.Id == Convert.ToInt32(id));
      }

      if (getCacheKey == null)
         return await getEntityAsync();

      //caching
      var cacheKey = getCacheKey(_staticCacheManager)
          ?? _staticCacheManager.PrepareKeyForDefaultCache(AppEntityCacheDefaults<TEntity>.ByIdCacheKey, id);

      return await _staticCacheManager.GetAsync(cacheKey, getEntityAsync);
   }

   /// <summary>
   /// Get entity entries by identifiers
   /// </summary>
   /// <param name="ids">Entity entry identifiers</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<long> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
   {
      if (!ids?.Any() ?? true)
         return new List<TEntity>();

      async Task<IList<TEntity>> getByIdsAsync()
      {
         //get entries
         var entries = await Table.Where(entry => ids.Contains(entry.Id)).ToListAsync();

         //sort by passed identifiers
         var sortedEntries = new List<TEntity>();
         foreach (var id in ids)
         {
            var sortedEntry = entries.Find(entry => entry.Id == id);
            if (sortedEntry != null)
               sortedEntries.Add(sortedEntry);
         }

         return sortedEntries;
      }

      if (getCacheKey == null)
         return await getByIdsAsync();

      //caching
      var cacheKey = getCacheKey(_staticCacheManager)
          ?? _staticCacheManager.PrepareKeyForDefaultCache(AppEntityCacheDefaults<TEntity>.ByIdsCacheKey, ids);
      return await _staticCacheManager.GetAsync(cacheKey, getByIdsAsync);
   }

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
       Func<IStaticCacheManager, CacheKey> getCacheKey = null)
   {
      async Task<IList<TEntity>> getAllAsync()
      {
         var query = Table;
         query = func != null ? func(query) : query;

         return await query.ToListAsync();
      }

      return await GetEntitiesAsync(getAllAsync, getCacheKey);
   }

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>Entity entries</returns>
   public virtual IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
       Func<IStaticCacheManager, CacheKey> getCacheKey = null)
   {
      IList<TEntity> getAll()
      {
         var query = Table;
         query = func != null ? func(query) : query;

         return query.ToList();
      }

      return GetEntities(getAll, getCacheKey);
   }

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
      Func<IStaticCacheManager, CacheKey> getCacheKey = null)
   {
      async Task<IList<TEntity>> getAllAsync()
      {
         var query = Table;
         query = func != null ? await func(query) : query;

         return await query.ToListAsync();
      }

      return await GetEntitiesAsync(getAllAsync, getCacheKey);
   }

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   public virtual async Task<IList<TEntity>> GetAllAsync(
       Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
       Func<IStaticCacheManager, Task<CacheKey>> getCacheKey = null)
   {
      async Task<IList<TEntity>> getAllAsync()
      {
         var query = Table;
         query = func != null ? await func(query) : query;

         return await query.ToListAsync();
      }

      return await GetEntitiesAsync(getAllAsync, getCacheKey);
   }

   /// <summary>
   /// Get paged list of all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the paged list of entity entries
   /// </returns>
   public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
       int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
   {
      var query = Table;

      query = func != null ? func(query) : query;

      return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
   }

   /// <summary>
   /// Get paged list of all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the paged list of entity entries
   /// </returns>
   public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
       int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
   {
      var query = Table;

      query = func != null ? await func(query) : query;

      return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
   }

   /// <summary>
   /// Insert the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertAsync(TEntity entity, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entity);

      await _dataProvider.AddAsync(entity);
      await _dataProvider.SaveChangesAsync();
      _dataProvider.Entry(entity).State = EntityState.Detached;

      //event notification
      if (publishEvent)
         await _eventPublisher.EntityInsertedAsync(entity);
   }

   /// <summary>
   /// Insert entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entities);

      await _dataProvider.AddRangeAsync(entities);
      await _dataProvider.SaveChangesAsync();
      
      foreach (var entity in entities)
         _dataProvider.Entry(entity).State = EntityState.Detached;

      if (!publishEvent)
         return;

      //event notification
      foreach (var entity in entities)
         await _eventPublisher.EntityInsertedAsync(entity);
   }

   /// <summary>
   /// Bulk insert entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task BulkInsertAsync(IList<TEntity> entities, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entities);

      using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
      await _dataProvider.BulkInsertAsync(entities);
      transaction.Complete();

      //event notification
      foreach (var entity in entities)
         await _eventPublisher.EntityInsertedAsync(entity);
   }


   /// <summary>
   /// Loads the original copy of the entity
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the copy of the passed entity
   /// </returns>
   public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
   {
      return await _dataProvider.GetTable<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == Convert.ToInt32(entity.Id));
   }

   /// <summary>
   /// Update the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateAsync(TEntity entity, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entity);

      _dataProvider.Update(entity);
      await _dataProvider.SaveChangesAsync();
      _dataProvider.Entry(entity).State = EntityState.Detached;

      //event notification
      if (publishEvent)
         await _eventPublisher.EntityUpdatedAsync(entity);
   }

   /// <summary>
   /// Update entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entities);

      if (entities.Count == 0)
         return;
      
      _dataProvider.UpdateRange(entities);
      await _dataProvider.SaveChangesAsync();

      foreach (var entity in entities)
         _dataProvider.Entry(entity).State = EntityState.Detached;

      //event notification
      if (publishEvent)
         foreach (var entity in entities)
            await _eventPublisher.EntityUpdatedAsync(entity);
   }

   /// <summary>
   /// Delete the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAsync(TEntity entity, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entity);
      
      _dataProvider.Remove(entity);
      await _dataProvider.SaveChangesAsync();
      _dataProvider.Entry(entity).State = EntityState.Detached;

      //event notification
      if (publishEvent)
         await _eventPublisher.EntityDeletedAsync(entity);
   }

   /// <summary>
   /// Delete entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
   {
      ArgumentNullException.ThrowIfNull(entities);

      if (entities.Count == 0)
         return;
      
      _dataProvider.RemoveRange(entities);
      await _dataProvider.SaveChangesAsync();
      
      foreach (var entity in entities)
         _dataProvider.Entry(entity).State = EntityState.Detached;

      //event notification
      if (publishEvent)
         foreach (var entity in entities)
            await _eventPublisher.EntityDeletedAsync(entity);
   }

   /// <summary>
   /// Delete entity entries by the passed predicate
   /// </summary>
   /// <param name="predicate">A function to test each element for a condition</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of deleted records
   /// </returns>
   public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
   {
      // TODO Soft delete 
      ArgumentNullException.ThrowIfNull(predicate);

      using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
      var countDeletedRecords = await _dataProvider.BulkDeleteAsync(predicate);
      transaction.Complete();

      return countDeletedRecords;
   }

   /// <summary>
   /// Truncates database table
   /// </summary>
   /// <param name="resetIdentity">Performs reset identity column</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task TruncateAsync(bool resetIdentity = false)
   {
      await LinqToDBForEFTools.GetTable<TEntity>(_dataProvider).TruncateAsync(resetIdentity);
   }


   /// <inheritdoc cref="IDisposable.Dispose"/>
   /// <remarks>
   /// <see href="https://learn.microsoft.com/ru-ru/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application">link</see>
   /// </remarks>
   public void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }


   /// <summary>
   /// Dispose pattern implementation
   /// </summary>
   /// <param name="disposing"></param>
   protected virtual void Dispose(bool disposing)
   {
      if (!_disposed)
         if (disposing)
            _dataProvider.Dispose();
      _disposed = true;
   }

  
   #endregion

   #region Properties

   /// <summary>
   /// Gets a table
   /// </summary>
   public virtual IQueryable<TEntity> Table => _table ??= _dataProvider.GetTable<TEntity>();

   ///// <summary>
   ///// Current Db context
   ///// </summary>
   //public virtual DbContext DbContext => _dataProvider;

   #endregion
}