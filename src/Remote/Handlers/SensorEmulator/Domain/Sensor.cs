namespace SensorEmulator.Domain;

/// <summary>
/// Represents a sensor class
/// </summary>
public class Sensor
{
    /// <summary>
    /// System name of the sensor
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Json seriliaze configuration of a sensor
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// Sensor group key
    /// </summary>
    public string Group { get; set; }
}
