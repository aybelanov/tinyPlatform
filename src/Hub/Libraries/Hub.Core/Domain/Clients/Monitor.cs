using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Shared.Clients.Domain;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents a monitor class
/// </summary>
public class Monitor : BaseEntity, IMonitor, IModifiedEntity, ILocalizedEntity, IAclSupported, ISoftDeletedEntity
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   [NotMapped, Localizable]
   public string Name { get; set; }

   /// <summary>
   /// Monotor picture idetnifier
   /// </summary>
   public long PictureId { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   [NotMapped, Localizable]
   public string Description { get; set; }

   /// <summary>
   /// MenuItem of the monitor page
   /// </summary>
   [NotMapped, Localizable]
   public string MenuItem { get; set; }

   /// <summary>
   /// Gets or sets the admin comment
   /// </summary>
   public string AdminComment { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting anf groupping
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is showing in the main menu
   /// </summary>
   public bool ShowInMenu { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is subject to ACL
   /// </summary>
   public bool SubjectToAcl { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit creating
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time of unit updating
   /// </summary>
   public DateTime? UpdatedOnUtc { get; set; }

   /// <summary>
   /// Soft deleted flag
   /// </summary>
   public bool IsDeleted { get; set; }

   /// <summary>
   /// The monitor user-owner identifier
   /// </summary>
   public long OwnerId { get; set; }

   #region data transfer properties

   /// <summary>
   /// The monitor user-owner name
   /// </summary>
   [NotMapped]
   public string OwnerName { get; set; }

   /// <summary>
   /// Icon picture url
   /// </summary>
   [NotMapped]
   public string PictireUrl { get; set; }

   /// <summary>
   /// Presentation collection
   /// </summary>
   [NotMapped]
   public IList<Presentation> Presentations { get; set; }

   #endregion

}