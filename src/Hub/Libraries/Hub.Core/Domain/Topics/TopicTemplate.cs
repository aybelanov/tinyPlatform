using Shared.Common;

namespace Hub.Core.Domain.Topics;

/// <summary>
/// Represents a topic template
/// </summary>
public partial class TopicTemplate : BaseEntity
{
   /// <summary>
   /// Gets or sets the template name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the view path
   /// </summary>
   public string ViewPath { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public List<Topic> Topics { get; set; } = new();

   //#pragma warning restore CS1591
   //   #endregion
}
