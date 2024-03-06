using Hub.Core.Domain.Users;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hub.Services.Helpers
{
   /// <summary>
   /// Represents a datetime helper
   /// </summary>
   public partial interface IDateTimeHelper
   {
      /// <summary>
      /// Returns a sorted collection of all the time zones
      /// </summary>
      /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
      ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();

      /// <summary>
      /// Converts the date and time to current user date and time
      /// </summary>
      /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains a DateTime value that represents time that corresponds to the dateTime parameter in user time zone.
      /// </returns>
      Task<DateTime> ConvertToUserTimeAsync(DateTime dt);

      /// <summary>
      /// Converts the date and time to current user date and time
      /// </summary>
      /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
      /// <param name="sourceDateTimeKind">The source datetimekind</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains a DateTime value that represents time that corresponds to the dateTime parameter in user time zone.
      /// </returns>
      Task<DateTime> ConvertToUserTimeAsync(DateTime dt, DateTimeKind sourceDateTimeKind);

      /// <summary>
      /// Converts the date and time to current user date and time
      /// </summary>
      /// <param name="dt">The date and time to convert.</param>
      /// <param name="sourceTimeZone">The time zone of dateTime.</param>
      /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
      /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in user time zone.</returns>
      DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone);

      /// <summary>
      /// Converts the date and time to Coordinated Universal Time (UTC)
      /// </summary>
      /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
      /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
      DateTime ConvertToUtcTime(DateTime dt);

      /// <summary>
      /// Converts the date and time to Coordinated Universal Time (UTC)
      /// </summary>
      /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
      /// <param name="sourceDateTimeKind">The source datetimekind</param>
      /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
      DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind);

      /// <summary>
      /// Converts the date and time to Coordinated Universal Time (UTC)
      /// </summary>
      /// <param name="dt">The date and time to convert.</param>
      /// <param name="sourceTimeZone">The time zone of dateTime.</param>
      /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
      DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone);

      /// <summary>
      /// Gets a user time zone
      /// </summary>
      /// <param name="user">User</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user time zone; if user is null, then default platform time zone
      /// </returns>
      Task<TimeZoneInfo> GetUserTimeZoneAsync(User user);

      /// <summary>
      /// Gets the current user time zone
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the current user time zone
      /// </returns>
      Task<TimeZoneInfo> GetCurrentTimeZoneAsync();

      /// <summary>
      /// Gets or sets a default platform time zone
      /// </summary>
      TimeZoneInfo DefaultPlatformTimeZone
      {
         get;
      }
   }
}