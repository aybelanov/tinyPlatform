using Shared.Common;
using System;

namespace Hub.Core.Domain.Forums;

/// <summary>
/// Represents a forum subscription item
/// </summary>
public partial class ForumSubscription : BaseEntity
{
   /// <summary>
   /// Gets or sets the forum subscription identifier
   /// </summary>
   public Guid SubscriptionGuid { get; set; }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the forum identifier
   /// </summary>
   public long ForumId { get; set; }

   /// <summary>
   /// Gets or sets the topic identifier
   /// </summary>
   public long TopicId { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public User User { get; set; }
   //   public ForumPost ForumPost { get; set; }
   //   public ForumPost ForumTopic { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}
