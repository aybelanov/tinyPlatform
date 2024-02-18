using Shared.Common;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represent a user unit mapping
/// </summary>
public class UserDevice : BaseEntity
{
   /// <summary>
   /// Get or set a user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Get or set a unit indentifier
   /// </summary>
   public long DeviceId { get; set; }


//   #region Navigation
//#pragma warning disable CS1591

//   public User User { get; set; }
//   public Device Device { get; set; }

//#pragma warning restore CS1591
//   #endregion
}
