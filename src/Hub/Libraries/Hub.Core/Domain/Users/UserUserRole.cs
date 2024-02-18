using Shared.Common;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represents a user-user role mapping class
/// </summary>
public partial class UserUserRole : BaseEntity
{
   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the user role identifier
   /// </summary>
   public long UserRoleId { get; set; }


//   #region Navigation
//#pragma warning disable CS1591

//   public User User { get; set; }
//   public UserRole UserRole { get; set; }

//#pragma warning restore CS1591
//   #endregion
}