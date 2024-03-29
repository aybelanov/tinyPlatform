﻿namespace Shared.Clients;

/// <summary>
/// Represents online status enum
/// </summary>
public enum OnlineStatus
{
   /// <summary>
   /// No any activities
   /// </summary>
   NoActivities = 1,

   /// <summary>
   /// Subjet is offline
   /// </summary>
   Offline,

   /// <summary>
   /// Subject has been recently
   /// </summary>
   BeenRecently,

   /// <summary>
   /// Subject is online
   /// </summary>
   Online
}
