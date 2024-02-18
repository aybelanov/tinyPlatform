using Shared.Common;

namespace Shared.Devices;

public interface ISensor
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   long Id { get; set; }

   /// <summary>
   /// Json seriliaze configuration of a sensor
   /// </summary>
   string Configuration { get; set; }

   /// <summary>
   /// Priority of sensding data from a sensor
   /// </summary>
   PriorityType PriorityType { get; set; }

   /// <summary>
   /// System name of the sensor. Unique in a whatcher system 
   /// </summary>
   string SystemName { get; set; }
}