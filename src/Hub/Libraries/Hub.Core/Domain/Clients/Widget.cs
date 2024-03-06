using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Security;
using Shared.Clients.Domain;
using Shared.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents a sensor presentation class
/// </summary>
public class Widget : BaseEntity, IWidget, ILocalizedEntity, IAclSupported, ISoftDeletedEntity, IModifiedEntity
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   [NotMapped, Localizable]
   public string Name { get; set; }

   /// <summary>
   /// Widget picture idetnifier
   /// </summary>
   public long PictureId { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   [NotMapped, Localizable]
   public string Description { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Enabled { get; set; }

   /// <summary>
   /// Gets or sets the admin comment
   /// </summary>
   public string AdminComment { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting and grouping
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is subject to ACL
   /// </summary>
   public bool SubjectToAcl { get; set; }

   /// <summary>
   /// A type of the sensor presentation 
   /// </summary>
   public WidgetType WidgetType { get; set; }

   /// <summary>
   /// Live scheme picture identifier
   /// </summary>
   public long LiveSchemePictureId { get; set; }

   /// <summary>
   /// Widget adjustment
   /// </summary>
   public string Adjustment { get; set; }

   /// <inheritdoc/>
   public bool IsDeleted { get; set; }

   /// <inheritdoc/>
   public DateTime CreatedOnUtc { get; set; }

   /// <inheritdoc/>
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Widget user-owner identifier
   /// </summary>
   public long UserId { get; set; }

   #region data transfer properties

   /// <summary>
   /// Widget owner name
   /// </summary>
   [NotMapped]
   public string OwnerName { get; set; }

   /// <summary>
   /// Icon picture url
   /// </summary>
   [NotMapped]
   public string PictureUrl { get; set; }

   /// <summary>
   /// Mnemoscheme in SVG format
   /// </summary>
   [NotMapped]
   public string LiveSchemeUrl { get; set; }

   #endregion
}