using System;
using Hub.Core.Domain.Users;
using System.Collections.Generic;
using Shared.Common;

namespace Hub.Core.Domain.Forums;

/// <summary>
/// Represents a forum post vote
/// </summary>
public partial class ForumPostVote : BaseEntity
{
   /// <summary>
   /// Gets or sets the forum post identifier
   /// </summary>
   public long ForumPostId { get; set; }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether this vote is up or is down
   /// </summary>
   public bool IsUp { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public User User { get; set; }
//   public ForumPost ForumPost { get; set; }

//#pragma warning restore CS1591
//   #endregion
}
