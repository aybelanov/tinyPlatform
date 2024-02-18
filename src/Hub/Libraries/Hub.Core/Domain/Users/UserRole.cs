using Hub.Core.Domain.Common;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Security;
using Shared.Common;
using System;
using System.Collections.Generic;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represents a user role
/// </summary>
public partial class UserRole : BaseEntity, ISoftDeletedEntity, IModifiedEntity
{
   /// <summary>
   /// Gets or sets the user role name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the user role is active
   /// </summary>
   public bool Active { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the user role is system
   /// </summary>
   public bool IsSystemRole { get; set; }

   /// <summary>
   /// Gets or sets the user role system name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the users must change passwords after a specified time
   /// </summary>
   public bool EnablePasswordLifetime { get; set; }

   /// <inheritdoc/>
   public bool IsDeleted { get; set; }
   
   /// <inheritdoc/>
   public DateTime CreatedOnUtc { get; set; }
  
   /// <inheritdoc/>
   public DateTime? UpdatedOnUtc { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public List<Campaign> Campaigns { get; set; } = new();
   
//   public List<AclRecord> AclRecords { get; set; } = new();
   
//   public List<User> Users { get; set; } = new();
 
//   public List<PermissionRecord> PermissionRecords { get; set; } = new();

//   public List<PermissionRecordUserRole> PermissionRecordUserRoles { get; set; } = new();


//#pragma warning restore CS1591
//   #endregion
}