using System.Collections.Generic;
using Shared.Common;

namespace Hub.Core.Domain.Logging;

/// <summary>
/// Represents an activity log type record
/// </summary>
public partial class ActivityLogType : BaseEntity
{
   /// <summary>
   /// Gets or sets the system keyword
   /// </summary>
   public string SystemKeyword { get; set; }

   /// <summary>
   /// Gets or sets the display name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the activity log type is enabled
   /// </summary>
   public bool Enabled { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   //public List<ActivityLog> ActivityLogRecords { get; set; } = new();

//#pragma warning restore CS1591
//   #endregion
}
