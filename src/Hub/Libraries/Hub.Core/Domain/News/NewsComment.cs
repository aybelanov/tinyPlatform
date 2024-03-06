using Shared.Common;
using System;

namespace Hub.Core.Domain.News;

/// <summary>
/// Represents a news comment
/// </summary>
public partial class NewsComment : BaseEntity
{
   /// <summary>
   /// Gets or sets the comment title
   /// </summary>
   public string CommentTitle { get; set; }

   /// <summary>
   /// Gets or sets the comment text
   /// </summary>
   public string CommentText { get; set; }

   /// <summary>
   /// Gets or sets the news item identifier
   /// </summary>
   public long NewsItemId { get; set; }

   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the comment is approved
   /// </summary>
   public bool IsApproved { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public User User { get; set; }
   //   public NewsItem NewsItem { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}