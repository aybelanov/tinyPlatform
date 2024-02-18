using Shared.Clients.Domain;
using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Represents a monitor class
/// </summary>
public class Monitor : BaseEntity, IMonitor
{
   /// <summary>
   /// Gets or sets the monitor name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// Title of the monitor page
   /// </summary>
   public string MenuItem { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool ShowInMenu { get; set; }

   /// <summary>
   /// Monitor picture url
   /// </summary>
   public string PictureUrl { get; set; }

   /// <summary>
   /// Device user-owner identifier
   /// </summary>
   public string OwnerId { get; set; }

   /// <summary>
   /// Device user-owner name
   /// </summary>
   public string OwnerName { get; set; }
}
