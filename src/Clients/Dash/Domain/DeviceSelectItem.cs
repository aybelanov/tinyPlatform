using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Device select item for client UI elemets like a dropdown list
/// </summary>
public class DeviceSelectItem : BaseEntity
{
   /// <summary>
   /// Device system name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Is device "shared"
   /// </summary>
   public bool IsShared { get; set; }

   /// <summary>
   /// Localized device name 
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// System name with shared mark asterix
   /// </summary>
   public string ItemName => IsShared ? Name + " (shared)" : Name;
}
