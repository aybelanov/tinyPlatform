using Shared.Common;
using System;

namespace Clients.Dash.Domain;

/// <summary>
/// Represents activity log record
/// </summary>
public class ActivityLogRecord : BaseEntity
{
   /// <summary>
   /// Gets or sets the activity log type identifier
   /// </summary>
   public string ActivityType { get; set; }

   /// <summary>
   /// Gets or sets the entity name
   /// </summary>
   public string EntityName { get; set; }

   /// <summary>
   /// Gets or sets the activity comment
   /// </summary>
   public string Comment { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the IP address
   /// </summary>
   public virtual string IpAddress { get; set; }
}
