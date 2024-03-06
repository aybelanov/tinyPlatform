using Shared.Clients;
using Shared.Common;
using System;

namespace Clients.Dash.Domain;

/// <summary>
/// Data file download task
/// </summary>
public class DownloadTask : BaseEntity, IDownloadTask
{
   /// <summary>
   /// User identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Username
   /// </summary>
   public string Username { get; set; }

   /// <summary>
   /// File full name
   /// </summary>
   public string FileName { get; set; }

   /// <summary>
   /// Download task creation datetime
   /// </summary>
   public DateTime TaskDateTimeUtc { get; set; }

   /// <summary>
   /// File readiness datetime on UTC
   /// </summary>
   public DateTime ReadyDateTimeUtc { get; set; }

   /// <summary>
   /// Current file readiness state
   /// </summary>
   public DownloadFileState CurrentState { get; set; }

   /// <summary>
   /// File size
   /// </summary>
   public long Size { get; set; }
}
