using Hub.Core.Domain.Users;
using Shared.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Logging;

/// <summary>
/// Represents an activity log record
/// </summary>
public partial class ActivityLog : BaseEntity
{
   /// <summary>
   /// Gets or sets the activity log type identifier
   /// </summary>
   public long ActivityLogTypeId { get; set; }

   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   public long? EntityId { get; set; }

   /// <summary>
   /// Gets or sets the entity name
   /// </summary>
   public string EntityName { get; set; }

   /// <summary>
   /// Gets or sets the subject identifier
   /// </summary>
   public long SubjectId { get; set; }

   /// <summary>
   /// Gets or sets the subject type
   /// </summary>
   public string SubjectName { get; set; }

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
   public string IpAddress { get; set; }

   /// <summary>
   /// Gets or sets the activity log type identifier
   /// </summary>
   [NotMapped]
   public string ActivityType { get; set; }
}