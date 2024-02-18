﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Hub.Data;

/// <summary>
/// Represents an entity repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public partial interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity
{
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
   Task<TEntity> GetByIdAsync(long? id, Func<IStaticCacheManager, CacheKey> getCacheKey = null);

   /// <summary>
   /// Get entity entries by identifiers
   /// </summary>
   /// <param name="ids">Entity entry identifiers</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   Task<IList<TEntity>> GetByIdsAsync(IList<long> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null);

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
       Func<IStaticCacheManager, CacheKey> getCacheKey = null);

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
       Func<IStaticCacheManager, CacheKey> getCacheKey = null);

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>Entity entries</returns>
   IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
       Func<IStaticCacheManager, CacheKey> getCacheKey = null);

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the entity entries
   /// </returns>
   Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func,
       Func<IStaticCacheManager, Task<CacheKey>> getCacheKey);

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the paged list of entity entries
   /// </returns>
   Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
       int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

   /// <summary>
   /// Get all entity entries
   /// </summary>
   /// <param name="func">Function to select entries</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the paged list of entity entries
   /// </returns>
   Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
       int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

   /// <summary>
   /// Insert the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertAsync(TEntity entity, bool publishEvent = true);

   /// <summary>
   /// Insert entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertAsync(IList<TEntity> entities, bool publishEvent = true);

   /// <summary>
   /// Bulk insert entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task BulkInsertAsync(IList<TEntity> entities, bool publishEvent = true);

   /// <summary>
   /// Update the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateAsync(TEntity entity, bool publishEvent = true);

   /// <summary>
   /// Update entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true);

   /// <summary>
   /// Delete the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteAsync(TEntity entity, bool publishEvent = true);

   /// <summary>
   /// Delete entity entries
   /// </summary>
   /// <param name="entities">Entity entries</param>
   /// <param name="publishEvent">Whether to publish event notification</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true);

   /// <summary>
   /// Delete entity entries by the passed predicate
   /// </summary>
   /// <param name="predicate">A function to test each element for a condition</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of deleted records
   /// </returns>
   Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

   /// <summary>
   /// Loads the original copy of the entity entry
   /// </summary>
   /// <param name="entity">Entity entry</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the copy of the passed entity entry
   /// </returns>
   Task<TEntity> LoadOriginalCopyAsync(TEntity entity);

   /// <summary>
   /// Truncates database table
   /// </summary>
   /// <param name="resetIdentity">Performs reset identity column</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task TruncateAsync(bool resetIdentity = false);

   #endregion

   #region Properties

   /// <summary>
   /// Gets a table
   /// </summary>
   IQueryable<TEntity> Table { get; }

   ///// <summary>
   ///// Current Db context
   ///// </summary>
   //DbContext DbContext { get; }

   #endregion
}