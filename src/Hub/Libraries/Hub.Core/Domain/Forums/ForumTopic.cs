using Shared.Common;
using System;

namespace Hub.Core.Domain.Forums;

/// <summary>
/// Represents a forum topic
/// </summary>
public partial class ForumTopic : BaseEntity
{
   /// <summary>
   /// Gets or sets the forum identifier
   /// </summary>
   public long ForumId { get; set; }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the topic type identifier
   /// </summary>
   public int TopicTypeId { get; set; }

   /// <summary>
   /// Gets or sets the subject
   /// </summary>
   public string Subject { get; set; }

   /// <summary>
   /// Gets or sets the number of posts
   /// </summary>
   public int NumPosts { get; set; }

   /// <summary>
   /// Gets or sets the number of views
   /// </summary>
   public int Views { get; set; }

   /// <summary>
   /// Gets or sets the last post identifier
   /// </summary>
   public long LastPostId { get; set; }

   /// <summary>
   /// Gets or sets the last post user identifier
   /// </summary>
   public long LastPostUserId { get; set; }

   /// <summary>
   /// Gets or sets the last post date and time
   /// </summary>
   public DateTime? LastPostTime { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance update
   /// </summary>
   public DateTime UpdatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the forum topic type
   /// </summary>
   public ForumTopicType ForumTopicType
   {
      get => (ForumTopicType)TopicTypeId;
      set => TopicTypeId = (int)value;
   }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public User User { get; set; }
   //   public Forum Forum { get; set; }
   //   public List<ForumPost> ForumPosts { get; set; } = new();

   //#pragma warning restore CS1591
   //   #endregion
}
