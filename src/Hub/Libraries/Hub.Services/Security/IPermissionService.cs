using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Security;

/// <summary>
/// Permission service interface
/// </summary>
public partial interface IPermissionService
{
   /// <summary>
   /// Gets all permissions
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the permissions
   /// </returns>
   Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync();

   /// <summary>
   /// Gets all user permissions
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="category">Permission category</param>
   /// <returns>Permission collection</returns>
   Task<IList<PermissionRecord>> GetUserPermissionsAsync(User user, string category = null);

   /// <summary>
   /// Inserts a permission
   /// </summary>
   /// <param name="permission">Permission</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertPermissionRecordAsync(PermissionRecord permission);

   /// <summary>
   /// Gets a permission record by identifier
   /// </summary>
   /// <param name="permissionId">Permission</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a permission record
   /// </returns>
   Task<PermissionRecord> GetPermissionRecordByIdAsync(long permissionId);

   /// <summary>
   /// Updates the permission
   /// </summary>
   /// <param name="permission">Permission</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdatePermissionRecordAsync(PermissionRecord permission);

   /// <summary>
   /// Deletes the permission
   /// </summary>
   /// <param name="permission">Permission</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeletePermissionRecordAsync(PermissionRecord permission);

   /// <summary>
   /// Install permissions
   /// </summary>
   /// <param name="permissionProvider">Permission provider</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InstallPermissionsAsync(IPermissionProvider permissionProvider);

   /// <summary>
   /// Install permissions
   /// </summary>
   /// <param name="permissionProvider">Permission provider</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UninstallPermissionsAsync(IPermissionProvider permissionProvider);

   /// <summary>
   /// Authorize permission
   /// </summary>
   /// <param name="permission">Permission record</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   Task<bool> AuthorizeAsync(PermissionRecord permission);

   /// <summary>
   /// Authorize permission
   /// </summary>
   /// <param name="permission">Permission record</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   Task<bool> AuthorizeAsync(PermissionRecord permission, User user);

   /// <summary>
   /// Authorize permission
   /// </summary>
   /// <param name="permissionRecordSystemName">Permission record system name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   Task<bool> AuthorizeAsync(string permissionRecordSystemName);

   /// <summary>
   /// Authorize permission
   /// </summary>
   /// <param name="permissionRecordSystemName">Permission record system name</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   Task<bool> AuthorizeAsync(string permissionRecordSystemName, User user);

   /// <summary>
   /// Authorize permission
   /// </summary>
   /// <param name="permissionRecordSystemName">Permission record system name</param>
   /// <param name="userRoleId">User role identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - authorized; otherwise, false
   /// </returns>
   Task<bool> AuthorizeAsync(string permissionRecordSystemName, long userRoleId);

   /// <summary>
   /// Gets a permission record-user role mapping
   /// </summary>
   /// <param name="permissionId">Permission identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task<IList<PermissionRecordUserRole>> GetMappingByPermissionRecordIdAsync(long permissionId);

   /// <summary>
   /// Delete a permission record-user role mapping
   /// </summary>
   /// <param name="permissionId">Permission identifier</param>
   /// <param name="userRoleId">User role identifier</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task DeletePermissionRecordUserRoleMappingAsync(long permissionId, long userRoleId);

   /// <summary>
   /// Inserts a permission record-user role mapping
   /// </summary>
   /// <param name="permissionRecordUserRoleMapping">Permission record-user role mapping</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InsertPermissionRecordUserRoleMappingAsync(PermissionRecordUserRole permissionRecordUserRoleMapping);
}