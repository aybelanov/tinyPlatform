namespace Shared.Clients.Domain;

/// <summary>
/// Sensor types
/// </summary>
public enum SensorType
{
   /// <summary>
   /// Scalar type
   /// </summary>
   Scalar = 1,

   /// <summary>
   /// Location
   /// </summary>
   Spatial,

   /// <summary>
   /// Complex data
   /// </summary>
   Complex,

   /// <summary>
   /// Media
   /// </summary>
   MediaStream
}
