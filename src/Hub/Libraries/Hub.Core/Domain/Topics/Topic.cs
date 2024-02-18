using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Seo;
using Hub.Core.Domain.Users;
using Shared.Common;

namespace Hub.Core.Domain.Topics;

/// <summary>
/// Represents a topic
/// </summary>
public partial class Topic : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic should be included in sitemap
   /// </summary>
   public bool IncludeInSitemap { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic should be included in top menu
   /// </summary>
   public bool IncludeInTopMenu { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
   /// </summary>
   public bool IncludeInFooterColumn1 { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
   /// </summary>
   public bool IncludeInFooterColumn2 { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
   /// </summary>
   public bool IncludeInFooterColumn3 { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic is accessible when a platform is closed
   /// </summary>
   public bool AccessibleWhenPlatformClosed { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this topic is password protected
   /// </summary>
   public bool IsPasswordProtected { get; set; }

   /// <summary>
   /// Gets or sets the password
   /// </summary>
   public string Password { get; set; }

   /// <summary>
   /// Gets or sets the title
   /// </summary>
   public string Title { get; set; }

   /// <summary>
   /// Gets or sets the body
   /// </summary>
   public string Body { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Published { get; set; }

   /// <summary>
   /// Gets or sets a value of used topic template identifier
   /// </summary>
   public long TopicTemplateId { get; set; }

   /// <summary>
   /// Gets or sets the meta keywords
   /// </summary>
   public string MetaKeywords { get; set; }

   /// <summary>
   /// Gets or sets the meta description
   /// </summary>
   public string MetaDescription { get; set; }

   /// <summary>
   /// Gets or sets the meta title
   /// </summary>
   public string MetaTitle { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is subject to ACL
   /// </summary>
   public bool SubjectToAcl { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public TopicTemplate TopicTemplate { get; set; }

//#pragma warning restore CS1591
//   #endregion
}
