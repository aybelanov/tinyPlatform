using System;
using Hub.Core.Domain.Users;
using System.Collections.Generic;
using Shared.Common;

namespace Hub.Core.Domain.Forums;

/// <summary>
/// Represents a private message
/// </summary>
public partial class PrivateMessage : BaseEntity
{
   /// <summary>
   /// Gets or sets the user identifier who sent the message
   /// </summary>
   public long FromUserId { get; set; }

   /// <summary>
   /// Gets or sets the user identifier who should receive the message
   /// </summary>
   public long ToUserId { get; set; }

   /// <summary>
   /// Gets or sets the subject
   /// </summary>
   public string Subject { get; set; }

   /// <summary>
   /// Gets or sets the text
   /// </summary>
   public string Text { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether message is read
   /// </summary>
   public bool IsRead { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether message is deleted by author
   /// </summary>
   public bool IsDeletedByAuthor { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether message is deleted by recipient
   /// </summary>
   public bool IsDeletedByRecipient { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public User FromUser { get; set; }
//   public User ToUser { get; set; }
  
//#pragma warning restore CS1591
//   #endregion
}
