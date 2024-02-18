using Hub.Core.Domain.Common;
using Shared.Clients;
using Shared.Clients.Domain;
using Shared.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represents a user
/// </summary>
public partial class User : BaseEntity, IClientUser ,ISoftDeletedEntity, IModifiedEntity
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public User()
   {
      UserGuid = Guid.NewGuid();
   }

   /// <summary>
   /// Gets or sets the user GUID
   /// </summary>
   public Guid UserGuid { get; set; }

   /// <summary>
   /// Gets or sets the username
   /// </summary>
   public string Username { get; set; }

   /// <summary>
   /// Gets or sets the email
   /// </summary>
   public string Email { get; set; }

   /// <summary>
   /// Gets or sets the email that should be re-validated. Used in scenarios when a user is already registered and wants to change an email address.
   /// </summary>
   public string EmailToRevalidate { get; set; }

   /// <summary>
   /// Gets or sets the admin comment
   /// </summary>
   public string AdminComment { get; set; }

   /// <summary>
   /// Gets or sets the affiliate identifier
   /// </summary>
   public long AffiliateId { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the user is required to re-login
   /// </summary>
   public bool RequireReLogin { get; set; }

   /// <summary>
   /// Gets or sets a value indicating number of failed login attempts (wrong password)
   /// </summary>
   public int FailedLoginAttempts { get; set; }

   /// <summary>
   /// Gets or sets the date and time until which a user cannot login (locked out)
   /// </summary>
   public DateTime? CannotLoginUntilDateUtc { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the user is active
   /// </summary>
   public bool IsActive { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the user has been deleted
   /// </summary>
   public bool IsDeleted { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the user account is system
   /// </summary>
   public bool IsSystemAccount { get; set; }

   /// <summary>
   /// Gets or sets the user system name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the last IP address
   /// </summary>
   public string LastIpAddress { get; set; }

   /// <summary>
   /// Gets or sets the date and time of entity creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Modified date of the entity
   /// </summary>
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of last login
   /// </summary>
   public DateTime? LastLoginUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of last activity
   /// </summary>
   public DateTime LastActivityUtc { get; set; }

   /// <summary>
   /// Gets or sets the billing address identifier
   /// </summary>
   public long? BillingAddressId { get; set; }

   /// <summary>
   /// Gets or sets the shipping address identifier
   /// </summary>
   public long? ShippingAddressId { get; set; }

   /// <summary>
   /// User device limit
   /// </summary>
   public int DeviceCountLimit { get; set; }

   /// <summary>
   /// User avatar picture identifier
   /// </summary>
   public long AvatarPictureId { get; set; }

   /// <summary>
   /// User online connection status
   /// </summary>
   [NotMapped]
   public OnlineStatus OnlineStatus {  get; set; }
}