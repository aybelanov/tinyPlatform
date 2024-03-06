using Shared.Common;

namespace Hub.Core.Domain.Polls;

/// <summary>
/// Represents a poll answer
/// </summary>
public partial class PollAnswer : BaseEntity
{
   /// <summary>
   /// Gets or sets the poll identifier
   /// </summary>
   public long PollId { get; set; }

   /// <summary>
   /// Gets or sets the poll answer name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the current number of votes
   /// </summary>
   public int NumberOfVotes { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public Poll Poll { get; set; }
   //   public List<PollVotingRecord> VotingRecords { get; set; } = new();   

   //#pragma warning restore CS1591
   //   #endregion
}