﻿using Shared.Common;

namespace Hub.Core.Domain.Seo;

/// <summary>
/// Represents an URL record
/// </summary>
public partial class UrlRecord : BaseEntity
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   public long EntityId { get; set; }

   /// <summary>
   /// Gets or sets the entity name
   /// </summary>
   public string EntityName { get; set; }

   /// <summary>
   /// Gets or sets the slug
   /// </summary>
   public string Slug { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether the record is active
   /// </summary>
   public bool IsActive { get; set; }

   /// <summary>
   /// Gets or sets the language identifier
   /// </summary>
   public long LanguageId { get; set; }

   //      #region Navigation
   //#pragma warning disable CS1591

   //      public Language Language { get; set; } 

   //#pragma warning restore CS1591
   //      #endregion
}
