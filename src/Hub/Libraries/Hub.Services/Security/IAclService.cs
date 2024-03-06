using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Shared.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Security;

/// <summary>
/// ACL service interface
/// </summary>
public partial interface IAclService
{
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
   Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, User user) where TEntity : BaseEntity, IAclSupported;

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
   Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, long[] userRoleIds) where TEntity : BaseEntity, IAclSupported;

   /// <summary>
   /// Deletes an ACL record
   /// </summary>
   /// <param name="aclRecord">ACL record</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeleteAclRecordAsync(AclRecord aclRecord);

   /// <summary>
   /// Gets ACL records
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the aCL records
   /// </returns>
   Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

   /// <summary>
   /// Inserts an ACL record
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <param name="userRoleId">User role id</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertAclRecordAsync<TEntity>(TEntity entity, long userRoleId) where TEntity : BaseEntity, IAclSupported;

   /// <summary>
   /// Find user role identifiers with granted access
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role identifiers
   /// </returns>
   Task<long[]> GetUserRoleIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

   /// <summary>
   /// Authorize ACL permission
   /// </summary>
   /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

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
   Task<bool> AuthorizeAsync<TEntity>(TEntity entity, User user) where TEntity : BaseEntity, IAclSupported;
}