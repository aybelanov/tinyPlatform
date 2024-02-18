using Shared.Clients;
using System;

namespace Shared.Clients.Domain;

/// <summary>
/// User on client interface
/// </summary>
public interface IClientUser
{
   /// <summary>
   /// Gets or sets the date and time of last activity
   /// </summary>
   bool IsActive { get; set; }

   /// <summary>
   /// Gets or sets the date and time of last activity
   /// </summary>
   DateTime LastActivityUtc { get; set; }

   /// <summary>
   /// User online connection status
   /// </summary>
   OnlineStatus OnlineStatus { get; set; }

   /// <summary>
   /// Gets or sets the username
   /// </summary>
   string Username { get; set; }
}