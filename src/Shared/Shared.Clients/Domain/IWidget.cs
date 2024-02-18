namespace Shared.Clients.Domain;

/// <summary>
/// Sensor presentation entity interface
/// </summary>
public interface IWidget
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   long Id { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   string Name { get; set; }

   /// <summary>
   /// Widget picture url
   /// </summary>
   string PictureUrl { get; set; }

   /// <summary>
   /// Gets or sets the short description
   /// </summary>
   string Description { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   bool Enabled { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting entities
   /// </summary>
   int DisplayOrder { get; set; }

   /// <summary>
   /// A type of the widget 
   /// </summary>
   WidgetType WidgetType { get; set; }

   /// <summary>
   /// Widget adjustment
   /// </summary>
   string Adjustment { get; set; }
}
