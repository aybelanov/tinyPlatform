using Shared.Common;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represents a user-address mapping class
/// </summary>
public partial class UserAddress : BaseEntity
{
   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the address identifier
   /// </summary>
   public long AddressId { get; set; }


   //   #region Navigation
   //#pragma warning disable CS1591

   //   public User User { get; set; }
   //   public Address Address { get; set; }   

   //#pragma warning restore CS1591
   //   #endregion
}