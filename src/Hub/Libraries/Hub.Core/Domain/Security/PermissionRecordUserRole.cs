using Shared.Common;

namespace Hub.Core.Domain.Security;

/// <summary>
/// Represents a permission record-user role mapping class
/// </summary>
public partial class PermissionRecordUserRole : BaseEntity
{
   /// <summary>
   /// Gets or sets the permission record identifier
   /// </summary>
   public long PermissionRecordId { get; set; }

   /// <summary>
   /// Gets or sets the user role identifier
   /// </summary>
   public long UserRoleId { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public UserRole UserRole { get; set; }

   //   public PermissionRecord PermissionRecord { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}