using Hub.Core.Domain.Users;
using Shared.Common;

namespace Hub.Core.Domain.Security;

/// <summary>
/// Represents an ACL record
/// </summary>
public partial class AclRecord : BaseEntity
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   public long EntityId { get; set; }

   /// <summary>
   /// Gets or sets the entity name
   /// </summary>
   public string EntityName { get; set; }

   /// <summary>
   /// Gets or sets the user role identifier
   /// </summary>
   public long UserRoleId { get; set; }


//   #region Navigation
//#pragma warning disable CS1591

//   public UserRole UserRole { get; set; }

//#pragma warning restore CS1591
//   #endregion
}
