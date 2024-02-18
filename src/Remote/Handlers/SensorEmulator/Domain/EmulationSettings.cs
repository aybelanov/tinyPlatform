namespace SensorEmulator.Domain;

/// <summary>
/// Represents a emulator settings 
/// </summary>
public class EmulationSettings
{
    /// <summary>
    /// average sensor value
    /// </summary>
    public int AverageValue { get; set; }

    /// <summary>
    /// deviation from the average delay in percents
    /// </summary>
    public int DeviationValue { get; set; }

    /// <summary>
    /// average delay between messages in seconds
    /// </summary>
    public int AverageDelay { get; set; }

    /// <summary>
    /// deviation from the average delay in percents 
    /// </summary>
    public int DeviationDelay { get; set; }
}