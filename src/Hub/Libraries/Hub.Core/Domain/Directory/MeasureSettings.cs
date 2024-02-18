using Hub.Core.Configuration;

namespace Hub.Core.Domain.Directory;

/// <summary>
/// Measure settings
/// </summary>
public class MeasureSettings : ISettings
{
   /// <summary>
   /// Base dimension identifier
   /// </summary>
   public long BaseDimensionId { get; set; }

   /// <summary>
   /// Base weight identifier
   /// </summary>
   public long BaseWeightId { get; set; }
}