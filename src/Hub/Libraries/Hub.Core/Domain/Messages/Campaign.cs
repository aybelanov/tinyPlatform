using Shared.Common;
using System;

namespace Hub.Core.Domain.Messages;

/// <summary>
/// Represents a campaign
/// </summary>
public partial class Campaign : BaseEntity
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the subject
   /// </summary>
   public string Subject { get; set; }

   /// <summary>
   /// Gets or sets the body
   /// </summary>
   public string Body { get; set; }

   /// <summary>
   /// Gets or sets the user role identifier  which subscribers it will be sent to; set 0 for all newsletter subscribers
   /// </summary>
   public long UserRoleId { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Gets or sets the date and time in UTC before which this email should not be sent
   /// </summary>
   public DateTime? DontSendBeforeDateUtc { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public UserRole UserRole { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}
