using Shared.Clients.Domain;
using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Represents a widget entity
/// </summary>
public class Widget : BaseEntity, IWidget
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// Widget picture url
   /// </summary>
   public string PictureUrl { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Enabled { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting entities
   /// </summary>
   public int DisplayOrder { get; set; }

   /// <summary>
   /// A type of the widget 
   /// </summary>
   public WidgetType WidgetType { get; set; }

   /// <summary>
   /// SVG content
   /// </summary>
   public string LiveSchemeUrl { get; set; }

   /// <summary>
   /// Widget adjustment
   /// </summary>
   public string Adjustment { get; set; }

   /// <summary>
   /// User-owner identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Iser-owner name
   /// </summary>
   public string OwnerName { get; set; }
}