namespace CamStreamingExample;

/// <summary>
/// Represents a sensor class
/// </summary>
public class Sensor
{
   /// <summary>
   /// Sensor identifier 
   /// </summary>
   public int Id { get; set; }

   /// <summary>
   /// System name of the sensor
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Json seriliaze configuration of a sensor
   /// </summary>
   public string Configuration { get; set; }
}
