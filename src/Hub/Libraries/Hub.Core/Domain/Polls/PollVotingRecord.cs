using Shared.Common;
using System;

namespace Hub.Core.Domain.Polls;

/// <summary>
/// Represents a poll voting record
/// </summary>
public partial class PollVotingRecord : BaseEntity
{
   /// <summary>
   /// Gets or sets the poll answer identifier
   /// </summary>
   public long PollAnswerId { get; set; }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public PollAnswer PollAnswer { get; set; }
   //   public User User { get; set; }   

   //#pragma warning restore CS1591
   //   #endregion
}