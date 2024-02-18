using Hub.Core.Configuration;

namespace Hub.Services.Helpers
{
   /// <summary>
   /// DateTime settings
   /// </summary>
   public class DateTimeSettings : ISettings
   {
      /// <summary>
      /// Gets or sets a default platform time zone identifier
      /// </summary>
      public string DefaultPlatformTimeZoneId { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether users are allowed to select theirs time zone
      /// </summary>
      public bool AllowUsersToSetTimeZone { get; set; }
   }
}