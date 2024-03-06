using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Shared.Clients;

/// <summary>
/// Represents a filter for the dynamic LINQ
/// </summary>
public class DynamicFilter
{
   /// <summary>
   /// String of the filter
   /// </summary>
   public string Filter { get; set; }

   /// <summary>
   /// Order by key
   /// </summary>
   public string OrderBy { get; set; }

   /// <summary>
   /// Skip the elements
   /// </summary>
   public int? Skip { get; set; }

   /// <summary>
   /// gets te top elements
   /// </summary>
   public int? Top { get; set; }

   /// <summary>
   /// Query to the repository as lambda string
   /// </summary>
   public string Query { get; set; }

   /// <summary>
   /// Gets collection count only
   /// </summary>
   public bool CountOnly { get; set; }

   /// <summary>
   /// Additional quries (values) to entities (keys)
   /// </summary>
   public IDictionary<string, string> AdditionalQueries { get; set; } = new Dictionary<string, string>();

   /// <summary>
   /// Entity identifier set
   /// </summary>
   public IEnumerable<long> Ids { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Device identifier
   /// </summary>
   public long? DeviceId { get; set; }

   /// <summary>
   /// Device identifiers
   /// </summary>
   public IEnumerable<long> DeviceIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Monitor identifier
   /// </summary>
   public long? MonitorId { get; set; }

   /// <summary>
   /// Monitor identifiers
   /// </summary>
   public IEnumerable<long> MonitorIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Widget identifier
   /// </summary>
   public long? WidgetId { get; set; }

   /// <summary>
   /// Widget identifiers
   /// </summary>
   public IEnumerable<long> WidgetIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// User identifier
   /// </summary>
   public long? UserId { get; set; }

   /// <summary>
   /// User identifiers
   /// </summary>
   public IEnumerable<long> UserIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Sensor identifier
   /// </summary>
   public long? SensorId { get; set; }

   /// <summary>
   /// Sensor identifiers
   /// </summary>
   public IEnumerable<long> SensorIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Sensor identifier
   /// </summary>
   public long? SensorWidgetId { get; set; }

   /// <summary>
   /// Sensor identifiers
   /// </summary>
   public IEnumerable<long> SensorWidgetIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Sensor identifier
   /// </summary>
   public long? PresentationId { get; set; }

   /// <summary>
   /// Sensor identifiers
   /// </summary>
   public IEnumerable<long> PresentatonIds { get; set; } = Enumerable.Empty<long>();

   /// <summary>
   /// Date time "from"
   /// </summary>
   public DateTime? From { get; set; }

   /// <summary>
   /// Date time "to"
   /// </summary>
   public DateTime? To { get; set; }

   /// <summary>
   /// Date time span
   /// </summary>
   public TimeSpan? TimeSpan { get; set; }

   /// <summary>
   /// Date time interval type
   /// </summary>
   public TimeIntervalType? TimeInterval { get; set; }

   /// <summary>
   /// Browser DateTime offset
   /// </summary>
   public int? BrowserOffset { get; set; }

   /// <summary>
   /// Connection status
   /// </summary>
   public IEnumerable<OnlineStatus> ConnectionStatuses { get; set; } = Enum.GetValues<OnlineStatus>(); //Enumerable.Empty<OnlineStatus>();

   /// <inheritdoc/>
   public override bool Equals(object obj)
   {
      if (obj is DynamicFilter filter)
      {
         var res = JsonSerializer.Serialize(this).Equals(JsonSerializer.Serialize(filter));
         return res;
      }

      return false;
   }

   /// <inheritdoc/>
   public override int GetHashCode()
   {
      return base.GetHashCode();
   }
}
