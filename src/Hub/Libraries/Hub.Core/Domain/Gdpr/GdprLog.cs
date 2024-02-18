using System;
using Hub.Core.Domain.Forums;
using System.Collections.Generic;
using Shared.Common;
using Hub.Core.Domain.Users;

namespace Hub.Core.Domain.Gdpr;

/// <summary>
/// Represents a GDPR log
/// </summary>
public partial class GdprLog : BaseEntity
{
   /// <summary>
   /// Gets or sets the user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Gets or sets the consent identifier (0 if not related to any consent)
   /// </summary>
   public long ConsentId { get; set; }

   /// <summary>
   /// Gets or sets the user info (when a user records is already deleted)
   /// </summary>
   public string UserInfo { get; set; }

   /// <summary>
   /// Gets or sets the request type identifier
   /// </summary>
   public int RequestTypeId { get; set; }

   /// <summary>
   /// Gets or sets the request details
   /// </summary>
   public string RequestDetails { get; set; }

   /// <summary>
   /// Gets or sets the date and time of entity creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the request type
   /// </summary>
   public GdprRequestType RequestType
   {
      get => (GdprRequestType)RequestTypeId;
      set => RequestTypeId = (int)value;
   }

//   #region Navigation
//#pragma warning disable CS1591

//   public User User { get; set; }
//   public GdprConsent GdprConsent { get; set; }

//#pragma warning restore CS1591
//   #endregion
}
