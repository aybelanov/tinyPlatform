namespace Shared.Clients.Domain;

/// <summary>
/// Monitor entity interface
/// </summary>
public interface IMonitor
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
   /// Gets or sets the short description
   /// </summary>
   string Description { get; set; }

   /// <summary>
   /// Title of the monitor page
   /// </summary>
   string MenuItem { get; set; }

   /// <summary>
   /// Gets or sets a display order.
   /// This value is used when sorting associated products (used with "grouped" products)
   /// </summary>
   int DisplayOrder { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is showing in the main menu
   /// </summary>
   bool ShowInMenu { get; set; }
}
