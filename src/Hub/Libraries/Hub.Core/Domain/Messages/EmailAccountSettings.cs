using Hub.Core.Configuration;

namespace Hub.Core.Domain.Messages;

/// <summary>
/// Email account settings
/// </summary>
public class EmailAccountSettings : ISettings
{
   /// <summary>
   /// Gets or sets a platform default email account identifier
   /// </summary>
   public long DefaultEmailAccountId { get; set; }
}
