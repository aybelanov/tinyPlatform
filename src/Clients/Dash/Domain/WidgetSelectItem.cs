using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Widget select item for client UI elemets like a dropdown list
/// </summary>
public class WidgetSelectItem : BaseEntity
{
   /// <summary>
   /// Widget name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Widget owner name
   /// </summary>
   public string OwnerName { get; set; }
}
