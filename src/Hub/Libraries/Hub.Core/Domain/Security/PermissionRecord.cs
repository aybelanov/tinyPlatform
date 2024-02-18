using System.Collections.Generic;
using Hub.Core.Domain.Users;
using Shared.Common;

namespace Hub.Core.Domain.Security;

/// <summary>
/// Represents a permission record
/// </summary>
public partial class PermissionRecord : BaseEntity
{
   /// <summary>
   /// Gets or sets the permission name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the permission system name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the permission category
   /// </summary>
   public string Category { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public List<UserRole> UserRoles { get; set; } = new();

//   public List<PermissionRecordUserRole> PermissionRecordUserRoles { get; set; } = new();

//#pragma warning restore CS1591
//   #endregion
}