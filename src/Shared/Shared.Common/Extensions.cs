using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common;

/// <summary>
/// Repsents shared extensions methods 
/// </summary>
public static class Extensions
{
   private static readonly DateTime _jsBaseDatetime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); 

   /// <summary>
   /// Converts the given UTC date value to epoch time in seconds.
   /// </summary>
   /// <remarks>
   /// For the unspecified date time kind will be set DateTimeKind.Utc,
   /// because the hub (and the data base) works only with UTC,
   /// but the database (EF Core) returns unspecifed date time values
   /// </remarks>
   /// <returns>A long UNIX epoch secon value of the DateTime</returns>
   public static long ToUnixEpochTime(this DateTime dateTime)
   {
      if(dateTime.Kind== DateTimeKind.Unspecified)
         dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

      var date = dateTime.ToUniversalTime();
      var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
      var ts = ticks / TimeSpan.TicksPerSecond;
      return ts;
   }

   /// <summary>
   /// Converts the given nullable UTC date value to epoch time in seconds.
   /// </summary>
   /// <remarks>
   /// For the unspecified date time kind will be set DateTimeKind.Utc,
   /// because the hub (and the data base) works only with UTC,
   /// but the database (EF Core) returns unspecifed date time values
   /// </remarks>
   /// <returns>A long UNIX epoch secon value of the DateTime</returns>
   public static long? ToUnixEpochTime(this DateTime? dateTime)
   {
      if (dateTime.HasValue)
         return dateTime.Value.ToUnixEpochTime();

      return null;   
   }

   /// <summary>
   /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
   /// </summary>
   public static DateTime ToDateTimeFromUinxEpoch(this long intDate)
   {
      var timeInTicks = intDate * TimeSpan.TicksPerSecond;
      return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
   }

   /// <summary>
   /// Converts the given nullable epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
   /// </summary>
   public static DateTime? ToDateTimeFromUinxEpoch(this long? intDate)
   {
      if (intDate.HasValue)
         return intDate.Value.ToDateTimeFromUinxEpoch();

      return null;
   }

   /// <summary>
   /// Converts nullable time span to nullable ticks
   /// </summary>
   /// <param name="timeSpan">Time span</param>
   /// <returns>Nullable ticks</returns>
   public static long? ToNullableTicks(this TimeSpan? timeSpan) 
   {
      if (timeSpan.HasValue)
         return timeSpan.Value.Ticks;

      return null;
   }


   /// <summary>
   /// Converts nullable ticks to nullable time span
   /// </summary>
   /// <param name="ticks">Ticks</param>
   /// <returns>Nullable timespan</returns>
   public static TimeSpan? ToNullableTimeSpan(this long? ticks)
   {
      if (ticks.HasValue)
         return new TimeSpan(ticks.Value);

      return null;
   }

   /// <summary>
   /// Converts datetime to javascript ticks
   /// </summary>
   /// <param name="date">Datetime</param>
   /// <returns>javascript ticks</returns>
   public static double ToJsTicks(this DateTime date)
   {
      var ticks = date.Ticks;
      var jsTicks = (double)(ticks - _jsBaseDatetime.Ticks) / TimeSpan.TicksPerMillisecond; 
      return jsTicks;
   }

   /// <summary>
   /// Converts datetime to javascript ticks
   /// </summary>
   /// <param name="dateTimeticks">Datetime as ticks</param>
   /// <returns>javascript ticks</returns>
   public static double ToJsTicks(this long dateTimeticks)
   {
      var jsTicks = ((double)(dateTimeticks - _jsBaseDatetime.Ticks)) / TimeSpan.TicksPerMillisecond;
      return jsTicks;
   }
}