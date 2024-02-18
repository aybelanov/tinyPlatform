using System;
using System.Collections.Generic;
using Hub.Core.Domain.Users;
using Shared.Common;

namespace Hub.Core.Domain.Forums;

/// <summary>
/// Represents a forum post
/// </summary>
public partial class ForumPost : BaseEntity
{
   /// <summary>
   /// Gets or sets the forum topic identifier
   /// </summary>
   public long ForumTopicId { get; set; }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the text
   /// </summary>
   public string Text { get; set; }

   /// <summary>
   /// Gets or sets the IP address
   /// </summary>
   public string IPAddress { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance update
   /// </summary>
   public DateTime UpdatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the count of votes
   /// </summary>
   public int VoteCount { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public User User { get; set; }
//   public ForumTopic ForumTopic { get; set; }

//#pragma warning restore CS1591
//   #endregion

}
