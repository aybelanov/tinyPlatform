using Shared.Common;
using System;

namespace Hub.Core.Domain.Blogs;

/// <summary>
/// Represents a blog comment
/// </summary>
public partial class BlogComment : BaseEntity
{
   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the comment text
   /// </summary>
   public string CommentText { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the comment is approved
   /// </summary>
   public bool IsApproved { get; set; }

   /// <summary>
   /// Gets or sets the blog post identifier
   /// </summary>
   public long BlogPostId { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public BlogPost BlogPost { get; set; }
   //   public User User { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}