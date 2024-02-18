using System.Collections.Generic;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Shared.Common;

namespace Hub.Core.Domain.Gdpr;

/// <summary>
/// Represents a GDPR consent
/// </summary>
public partial class GdprConsent : BaseEntity, ILocalizedEntity
{
   /// <summary>
   /// Gets or sets the message displayed to users
   /// </summary>
   public string Message { get; set; }

   /// <summary>
   /// Gets or setsa value indicating whether the consent is required
   /// </summary>
   public bool IsRequired { get; set; }

   /// <summary>
   /// Gets or sets the message displayed to users when he/she doesn't agree to this consent
   /// </summary>
   public string RequiredMessage { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this consent is displayed during registrations
   /// </summary>
   public bool DisplayDuringRegistration { get; set; }

   /// <summary>
   /// Gets or sets the value indicating whether this consent is displayed on my account page (user info)
   /// </summary>
   public bool DisplayOnUserInfoPage { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public List<GdprLog> GdprLogRecords { get; set; } = new();

//#pragma warning restore CS1591
//   #endregion
}