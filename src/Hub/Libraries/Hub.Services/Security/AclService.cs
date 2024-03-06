using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Data.Extensions;
using Hub.Services.Users;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Security;

/// <summary>
/// ACL service
/// </summary>
public partial class AclService : IAclService
{
   #region Fields

   private readonly IUserService _userService;
   private readonly IRepository<AclRecord> _aclRecordRepository;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IWorkContext _workContext;
   private readonly CommonSettings _commonSettings;

   #endregion

   #region Ctor

   /// <summary> IoC Ctor </summary>
   public AclService(IUserService userService,
       IRepository<AclRecord> aclRecordRepository,
       IStaticCacheManager staticCacheManager,
       IWorkContext workContext,
       CommonSettings commonSettings)
   {
      _userService = userService;
      _aclRecordRepository = aclRecordRepository;
      _staticCacheManager = staticCacheManager;
      _workContext = workContext;
      _commonSettings = commonSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Inserts an ACL record
   /// </summary>
   /// <param name="aclRecord">ACL record</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InsertAclRecordAsync(AclRecord aclRecord)
   {
      await _aclRecordRepository.InsertAsync(aclRecord);
   }

   /// <summary>
   /// Get a value indicating whether any ACL records exist for entity type are related to user roles
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if exist; otherwise false
   /// </returns>
   protected virtual async Task<bool> IsEntityAclMappingExistAsync<TEntity>() where TEntity : BaseEntity, IAclSupported
   {
      var entityName = typeof(TEntity).Name;
      var key = _staticCacheManager.PrepareKeyForDefaultCache(AppSecurityDefaults.EntityAclRecordExistsCacheKey, entityName);

      var query = from acl in _aclRecordRepository.Table
                  where acl.EntityName == entityName
                  select acl;

      return await _staticCacheManager.GetAsync(key, query.Any);
   }

   #endregion

   #region Methods

   /// <summary>
   /// Apply ACL to the passed query
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="query">Query to filter</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the filtered query
   /// </returns>
   public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, User user)
       where TEntity : BaseEntity, IAclSupported
   {
      if (query is null)
         throw new ArgumentNullException(nameof(query));

      if (user is null)
         throw new ArgumentNullException(nameof(user));

      var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
      return await ApplyAcl(query, userRoleIds);
   }

   /// <summary>
   /// Apply ACL to the passed query
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="query">Query to filter</param>
   /// <param name="userRoleIds">Identifiers of user's roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the filtered query
   /// </returns>
   public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, long[] userRoleIds)
       where TEntity : BaseEntity, IAclSupported
   {
      if (query is null)
         throw new ArgumentNullException(nameof(query));

      if (userRoleIds is null)
         throw new ArgumentNullException(nameof(userRoleIds));

      if (!userRoleIds.Any() || _commonSettings.IgnoreAcl || !await IsEntityAclMappingExistAsync<TEntity>())
         return query;

      return from entity in query
             where !entity.SubjectToAcl || _aclRecordRepository.Table.Any(acl =>
                  acl.EntityName == typeof(TEntity).Name && acl.EntityId == entity.Id && userRoleIds.Contains(acl.UserRoleId))
             select entity;
   }

   /// <summary>
   /// Deletes an ACL record
   /// </summary>
   /// <param name="aclRecord">ACL record</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteAclRecordAsync(AclRecord aclRecord)
   {
      await _aclRecordRepository.DeleteAsync(aclRecord);
   }

   /// <summary>
   /// Gets ACL records
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the aCL records
   /// </returns>
   public virtual async Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      var entityId = entity.Id;
      var entityName = entity.GetType().Name;

      var query = from ur in _aclRecordRepository.Table
                  where ur.EntityId == entityId &&
                  ur.EntityName == entityName
                  select ur;
      var aclRecords = await query.ToListAsync();

      return aclRecords;
   }

   /// <summary>
   /// Inserts an ACL record
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="userRoleId">User role id</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertAclRecordAsync<TEntity>(TEntity entity, long userRoleId) where TEntity : BaseEntity, IAclSupported
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      if (userRoleId == 0)
         throw new ArgumentOutOfRangeException(nameof(userRoleId));

      var entityId = entity.Id;
      var entityName = entity.GetType().Name;

      var aclRecord = new AclRecord
      {
         EntityId = entityId,
         EntityName = entityName,
         UserRoleId = userRoleId
      };

      await InsertAclRecordAsync(aclRecord);
   }

   /// <summary>
   /// Find user role identifiers with granted access
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role identifiers
   /// </returns>
   public virtual async Task<long[]> GetUserRoleIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      var entityId = entity.Id;
      var entityName = entity.GetType().Name;

      var key = _staticCacheManager.PrepareKeyForDefaultCache(AppSecurityDefaults.AclRecordCacheKey, entityId, entityName);

      var query = from ur in _aclRecordRepository.Table
                  where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                  select ur.UserRoleId;

      return await _staticCacheManager.GetAsync(key, () => query.ToArray());
   }

   /// <summary>
   /// Authorize ACL permission
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
   {
      return await AuthorizeAsync(entity, await _workContext.GetCurrentUserAsync());
   }

   /// <summary>
   /// Authorize ACL permission
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, User user) where TEntity : BaseEntity, IAclSupported
   {
      if (entity == null)
         return false;

      if (user == null)
         return false;

      if (_commonSettings.IgnoreAcl)
         return true;

      if (!entity.SubjectToAcl)
         return true;

      foreach (var role1 in await _userService.GetUserRolesAsync(user))
         foreach (var role2Id in await GetUserRoleIdsWithAccessAsync(entity))
            if (role1.Id == role2Id)
               //yes, we have such permission
               return true;

      //no permission found
      return false;
   }

   #endregion
}