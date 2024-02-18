using Shared.Clients;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Shared.Common;

namespace Hub.Core.Domain.Clients;

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
   /// Delays executing next request until date time
   /// </summary>
   public DateTime DelayUntilUtc { get; set; }

   /// <summary>
   /// Current file readiness state
   /// </summary>
   public DownloadFileState CurrentState { get; set; }

   /// <summary>
   /// File size
   /// </summary>
   public long Size { get; set; }

   /// <summary>
   /// Query hash value to define unique one
   /// </summary>
   public string QueryString { get; set; }

   #region data transfer properties

   /// <summary>
   /// Username
   /// </summary>
   [NotMapped]
   public string Username { get; set; }

   /// <summary>
   /// Current file readiness state identifier
   /// </summary>
   [NotMapped]
   public int CurrentStateId
   {
      get => (int)CurrentState;
      set => CurrentState = (DownloadFileState)value;
   }

   #endregion
}
