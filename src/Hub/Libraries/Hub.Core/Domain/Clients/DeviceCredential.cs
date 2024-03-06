using Hub.Core.Domain.Users;
using Shared.Common;
using System;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents device credential class
/// </summary>
public class DeviceCredential : BaseEntity
{
   /// <summary>
   /// Client credential to request token for Machine to Machine communication
   /// </summary>
   public string Password { get; set; }

   /// <summary>
   /// Gets or sets the password salt
   /// </summary>
   public string PasswordSalt { get; set; }

   /// <summary>
   /// Device identifier
   /// </summary>
   public long DeviceId { get; set; }

   /// <summary>
   /// Gets or sets the password format identifier
   /// </summary>
   public long PasswordFormatId { get; set; }

   /// <summary>
   /// Gets or sets the password format
   /// </summary>
   public PasswordFormat PasswordFormat
   {
      get => (PasswordFormat)PasswordFormatId;
      set => PasswordFormatId = (int)value;
   }

   /// <summary>
   /// Gets or sets the date and time of entity creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <inheritdoc/>
   public DateTime? UpdatedOnUtc { get; set; }
}
