using Shared.Common;

namespace Hub.Services.Clients.Sensors;

/// <summary>
/// Device select item for client UI elemets like a dropdown list
/// </summary>
public class SensorSelectItem : BaseEntity
{
   /// <summary>
   /// Device system name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Sensor localized name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Sensor value measure unit
   /// </summary>
   public string MeasureUnit { get; set; }
}
