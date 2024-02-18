using System;

namespace Shared.Clients;

/// <summary>
/// Data file download task interface
/// </summary>
public interface IDownloadTask
{
   /// <summary>
   /// Delays executing next request until date time
   /// </summary>
   DownloadFileState CurrentState { get; set; }

   /// <summary>
   /// File full name
   /// </summary>
   string FileName { get; set; }

   /// <summary>
   /// File readiness datetime on UTC
   /// </summary>
   DateTime ReadyDateTimeUtc { get; set; }

   /// <summary>
   /// File size
   /// </summary>
   long Size { get; set; }

   /// <summary>
   /// Download task creation datetime
   /// </summary>
   DateTime TaskDateTimeUtc { get; set; }

   /// <summary>
   /// User identifier
   /// </summary>
   long UserId { get; set; }
}