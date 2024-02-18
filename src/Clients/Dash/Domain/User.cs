using Shared.Clients;
using Shared.Clients.Domain;
using Shared.Common;
using System;

namespace Clients.Dash.Domain;

/// <summary>
/// Represents a user class
/// </summary>
public class User : BaseEntity, IClientUser
{
   /// <summary>
   /// Username
   /// </summary>
   public string Username { get; set; }

   /// <summary>
   /// User online status
   /// </summary>
   public OnlineStatus OnlineStatus { get; set; }

   /// <summary>
   /// Last activity date (in unix epoche)
   /// </summary>
   public DateTime LastActivityUtc { get; set; }

   /// <summary>
   /// Is user active (admin approved or not banned)
   /// </summary>
   public bool IsActive { get; set; }

   /// <summary>
   /// User avatar url
   /// </summary>
   public string AvatarUrl { get; set; }
}
