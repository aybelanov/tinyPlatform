using Shared.Common;
using System;

namespace Hub.Core.Domain.Polls;

/// <summary>
/// Represents a poll
/// </summary>
public partial class Poll : BaseEntity
{
   /// <summary>
   /// Gets or sets the language identifier
   /// </summary>
   public long LanguageId { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the system keyword
   /// </summary>
   public string SystemKeyword { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Published { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity should be shown on home page
   /// </summary>
   public bool ShowOnHomepage { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the anonymous votes are allowed
   /// </summary>
   public bool AllowGuestsToVote { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets the poll start date and time
   /// </summary>
   public DateTime? StartDateUtc { get; set; }

   /// <summary>
   /// Gets or sets the poll end date and time
   /// </summary>
   public DateTime? EndDateUtc { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public Language Language { get; set; }
   //   public List<PollAnswer> Answers { get; set; } = new();

   //#pragma warning restore CS1591
   //   #endregion
}