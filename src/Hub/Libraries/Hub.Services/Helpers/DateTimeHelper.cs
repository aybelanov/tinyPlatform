﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Services.Common;

namespace Hub.Services.Helpers;

/// <summary>
/// Represents a datetime helper
/// </summary>
public partial class DateTimeHelper : IDateTimeHelper
{
   #region Fields

   private readonly DateTimeSettings _dateTimeSettings;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DateTimeHelper(DateTimeSettings dateTimeSettings,
       IGenericAttributeService genericAttributeService,
       IWorkContext workContext)
   {
      _dateTimeSettings = dateTimeSettings;
      _genericAttributeService = genericAttributeService;
      _workContext = workContext;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
   /// </summary>
   /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
   /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
   protected virtual TimeZoneInfo FindTimeZoneById(string id)
   {
      return TimeZoneInfo.FindSystemTimeZoneById(id);
   }

   #endregion

   #region Methods

   /// <summary>
   /// Returns a sorted collection of all the time zones
   /// </summary>
   /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
   public virtual ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
   {
      return TimeZoneInfo.GetSystemTimeZones();
   }

   /// <summary>
   /// Converts the date and time to current user date and time
   /// </summary>
   /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a DateTime value that represents time that corresponds to the dateTime parameter in user time zone.
   /// </returns>
   public virtual async Task<DateTime> ConvertToUserTimeAsync(DateTime dt)
   {
      return await ConvertToUserTimeAsync(dt, dt.Kind);
   }

   /// <summary>
   /// Converts the date and time to current user date and time
   /// </summary>
   /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
   /// <param name="sourceDateTimeKind">The source datetimekind</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a DateTime value that represents time that corresponds to the dateTime parameter in user time zone.
   /// </returns>
   public virtual async Task<DateTime> ConvertToUserTimeAsync(DateTime dt, DateTimeKind sourceDateTimeKind)
   {
      dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
      if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
         return dt;

      var currentUserTimeZoneInfo = await GetCurrentTimeZoneAsync();
      return TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);
   }

   /// <summary>
   /// Converts the date and time to current user date and time
   /// </summary>
   /// <param name="dt">The date and time to convert.</param>
   /// <param name="sourceTimeZone">The time zone of dateTime.</param>
   /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
   /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in user time zone.</returns>
   public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
   {
      if (sourceTimeZone.IsInvalidTime(dt))
         return dt;

      return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
   }

   /// <summary>
   /// Converts the date and time to Coordinated Universal Time (UTC)
   /// </summary>
   /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
   /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
   public virtual DateTime ConvertToUtcTime(DateTime dt)
   {
      return ConvertToUtcTime(dt, dt.Kind);
   }

   /// <summary>
   /// Converts the date and time to Coordinated Universal Time (UTC)
   /// </summary>
   /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
   /// <param name="sourceDateTimeKind">The source datetimekind</param>
   /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
   public virtual DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
   {
      dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
      if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
         return dt;

      return TimeZoneInfo.ConvertTimeToUtc(dt);
   }

   /// <summary>
   /// Converts the date and time to Coordinated Universal Time (UTC)
   /// </summary>
   /// <param name="dt">The date and time to convert.</param>
   /// <param name="sourceTimeZone">The time zone of dateTime.</param>
   /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
   public virtual DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone)
   {
      if (sourceTimeZone.IsInvalidTime(dt))
      {
         //could not convert
         return dt;
      }

      return TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
   }

   /// <summary>
   /// Gets a user time zone
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user time zone; if user is null, then default platform time zone
   /// </returns>
   public virtual async Task<TimeZoneInfo> GetUserTimeZoneAsync(User user)
   {
      if (!_dateTimeSettings.AllowUsersToSetTimeZone)
         return DefaultPlatformTimeZone;

      TimeZoneInfo timeZoneInfo = null;

      var timeZoneId = string.Empty;
      if (user != null)
         timeZoneId = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.TimeZoneIdAttribute);

      try
      {
         if (!string.IsNullOrEmpty(timeZoneId))
            timeZoneInfo = FindTimeZoneById(timeZoneId);
      }
      catch (Exception exc)
      {
         Debug.Write(exc.ToString());
      }

      return timeZoneInfo ?? DefaultPlatformTimeZone;
   }

   /// <summary>
   /// Gets the current user time zone
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the current user time zone
   /// </returns>
   public virtual async Task<TimeZoneInfo> GetCurrentTimeZoneAsync()
   {
      return await GetUserTimeZoneAsync(await _workContext.GetCurrentUserAsync());
   }

   /// <summary>
   /// Gets or sets a default platform time zone
   /// </summary>
   public virtual TimeZoneInfo DefaultPlatformTimeZone
   {
      get
      {
         TimeZoneInfo timeZoneInfo = null;
         try
         {
            if (!string.IsNullOrEmpty(_dateTimeSettings.DefaultPlatformTimeZoneId))
               timeZoneInfo = FindTimeZoneById(_dateTimeSettings.DefaultPlatformTimeZoneId);
         }
         catch (Exception exc)
         {
            Debug.Write(exc.ToString());
         }

         return timeZoneInfo ?? TimeZoneInfo.Local;
      }
   }

   #endregion
}