using System;
using Hub.Core.Domain.Common;
using Shared.Common;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represents a user password
/// </summary>
public partial class UserPassword : BaseEntity, ISoftDeletedEntity, IModifiedEntity
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public UserPassword()
   {
      PasswordFormat = PasswordFormat.Clear;
   }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the password
   /// </summary>
   public string Password { get; set; }

   /// <summary>
   /// Gets or sets the password format identifier
   /// </summary>
   public long PasswordFormatId { get; set; }

   /// <summary>
   /// Gets or sets the password salt
   /// </summary>
   public string PasswordSalt { get; set; }

   /// <summary>
   /// Gets or sets the date and time of entity creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <inheritdoc/>
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the password format
   /// </summary>
   public PasswordFormat PasswordFormat
   {
      get => (PasswordFormat)PasswordFormatId;
      set => PasswordFormatId = (int)value;
   }

   /// <inheritdoc/>
   public bool IsDeleted { get ; set; }


//   #region Navigation
//#pragma warning disable CS1591

//   public User User { get; set; }

//#pragma warning restore CS1591
//   #endregion
}