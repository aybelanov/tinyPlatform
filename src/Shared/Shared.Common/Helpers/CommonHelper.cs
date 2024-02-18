﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Shared.Common.Helpers;

/// <summary>
/// Represents a common helper
/// </summary>
public class CommonHelper
{
   #region Fields

   //we use EmailValidator from FluentValidation. So let's keep them sync - https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Validators/EmailValidator.cs
   private const string EMAIL_EXPRESSION = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";

   private static readonly Regex _emailRegex;

   #endregion

   #region Ctor

   static CommonHelper()
   {
      _emailRegex = new Regex(EMAIL_EXPRESSION, RegexOptions.IgnoreCase);
   }

   #endregion

   #region Methods

   /// <summary>
   /// Ensures the subscriber email or throw.
   /// </summary>
   /// <param name="email">The email.</param>
   /// <returns></returns>
   public static string EnsureSubscriberEmailOrThrow(string email)
   {
      var output = EnsureNotNull(email);
      output = output.Trim();
      output = EnsureMaximumLength(output, 255);

      if (!IsValidEmail(output))
      {
         throw new Exception("Email is not valid.");
      }

      return output;
   }

   /// <summary>
   /// Verifies that a string is in valid e-mail format
   /// </summary>
   /// <param name="email">Email to verify</param>
   /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
   public static bool IsValidEmail(string email)
   {
      if (string.IsNullOrEmpty(email))
         return false;

      email = email.Trim();

      return _emailRegex.IsMatch(email);
   }

   /// <summary>
   /// Verifies that string is an valid IP-Address
   /// </summary>
   /// <param name="ipAddress">IPAddress to verify</param>
   /// <returns>true if the string is a valid IpAddress and false if it's not</returns>
   public static bool IsValidIpAddress(string ipAddress)
   {
      return IPAddress.TryParse(ipAddress, out var _);
   }

   /// <summary>
   /// Ensure that a string doesn't exceed maximum allowed length
   /// </summary>
   /// <param name="str">Input string</param>
   /// <param name="maxLength">Maximum length</param>
   /// <param name="postfix">A string to add to the end if the original string was shorten</param>
   /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
   public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
   {
      if (string.IsNullOrEmpty(str))
         return str;

      if (str.Length <= maxLength)
         return str;

      var pLen = postfix?.Length ?? 0;

      var result = str[0..(maxLength - pLen)];
      if (!string.IsNullOrEmpty(postfix))
      {
         result += postfix;
      }

      return result;
   }

   /// <summary>
   /// Ensures that a string only contains numeric values
   /// </summary>
   /// <param name="str">Input string</param>
   /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
   public static string EnsureNumericOnly(string str)
   {
      return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsDigit).ToArray());
   }

   /// <summary>
   /// Ensure that a string is not null
   /// </summary>
   /// <param name="str">Input string</param>
   /// <returns>Result</returns>
   public static string EnsureNotNull(string str)
   {
      return str ?? string.Empty;
   }

   /// <summary>
   /// Indicates whether the specified strings are null or empty strings
   /// </summary>
   /// <param name="stringsToValidate">Array of strings to validate</param>
   /// <returns>Boolean</returns>
   public static bool AreNullOrEmpty(params string[] stringsToValidate)
   {
      return stringsToValidate.Any(string.IsNullOrEmpty);
   }

   /// <summary>
   /// Compare two arrays
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <param name="a1">Array 1</param>
   /// <param name="a2">Array 2</param>
   /// <returns>Result</returns>
   public static bool ArraysEqual<T>(T[] a1, T[] a2)
   {
      //also see Enumerable.SequenceEqual(a1, a2);
      if (ReferenceEquals(a1, a2))
         return true;

      if (a1 == null || a2 == null)
         return false;

      if (a1.Length != a2.Length)
         return false;

      var comparer = EqualityComparer<T>.Default;
      return !a1.Where((t, i) => !comparer.Equals(t, a2[i])).Any();
   }

   /// <summary>
   /// Sets a property on an object to a value.
   /// </summary>
   /// <param name="instance">The object whose property to set.</param>
   /// <param name="propertyName">The name of the property to set.</param>
   /// <param name="value">The value to set the property to.</param>
   public static void SetProperty(object instance, string propertyName, object value)
   {
      if (instance == null)
         throw new ArgumentNullException(nameof(instance));
      if (propertyName == null)
         throw new ArgumentNullException(nameof(propertyName));

      var instanceType = instance.GetType();
      var pi = instanceType.GetProperty(propertyName);
      if (pi == null)
         throw new Exception(string.Format("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType));
      if (!pi.CanWrite)
         throw new Exception(string.Format("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType));
      if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
         value = To(value, pi.PropertyType);
      pi.SetValue(instance, value, Array.Empty<object>());
   }

   /// <summary>
   /// Converts a value to a destination type.
   /// </summary>
   /// <param name="value">The value to convert.</param>
   /// <param name="destinationType">The type to convert the value to.</param>
   /// <returns>The converted value.</returns>
   public static object To(object value, Type destinationType)
   {
      return To(value, destinationType, CultureInfo.InvariantCulture);
   }

   /// <summary>
   /// Converts a value to a destination type.
   /// </summary>
   /// <param name="value">The value to convert.</param>
   /// <param name="destinationType">The type to convert the value to.</param>
   /// <param name="culture">Culture</param>
   /// <returns>The converted value.</returns>
   public static object To(object value, Type destinationType, CultureInfo culture)
   {
      if (value == null)
         return null;

      var sourceType = value.GetType();

      var destinationConverter = TypeDescriptor.GetConverter(destinationType);
      if (destinationConverter.CanConvertFrom(value.GetType()))
         return destinationConverter.ConvertFrom(null, culture, value);

      var sourceConverter = TypeDescriptor.GetConverter(sourceType);
      if (sourceConverter.CanConvertTo(destinationType))
         return sourceConverter.ConvertTo(null, culture, value, destinationType);

      if (destinationType.IsEnum && value is int)
         return Enum.ToObject(destinationType, (int)value);

      if (!destinationType.IsInstanceOfType(value))
         return Convert.ChangeType(value, destinationType, culture);

      return value;
   }

   /// <summary>
   /// Converts a value to a destination type.
   /// </summary>
   /// <param name="value">The value to convert.</param>
   /// <typeparam name="T">The type to convert the value to.</typeparam>
   /// <returns>The converted value.</returns>
   public static T To<T>(object value)
   {
      //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
      return (T)To(value, typeof(T));
   }

   /// <summary>
   /// Convert enum for front-end
   /// </summary>
   /// <param name="str">Input string</param>
   /// <returns>Converted string</returns>
   public static string ConvertEnum(string str)
   {
      if (string.IsNullOrEmpty(str))
         return string.Empty;
      var result = string.Empty;
      foreach (var c in str)
         if (c.ToString() != c.ToString().ToLowerInvariant())
            result += " " + c.ToString();
         else
            result += c.ToString();

      //ensure no spaces (e.g. when the first letter is upper case)
      result = result.TrimStart();
      return result;
   }

   /// <summary>
   /// Get difference in years
   /// </summary>
   /// <param name="startDate"></param>
   /// <param name="endDate"></param>
   /// <returns></returns>
   public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
   {
      //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
      //this assumes you are looking for the western idea of age and not using East Asian reckoning.
      var age = endDate.Year - startDate.Year;
      if (startDate > endDate.AddYears(-age))
         age--;
      return age;
   }

   /// <summary>
   /// Get DateTime to the specified year, month, and day using the conventions of the current thread culture
   /// </summary>
   /// <param name="year">The year</param>
   /// <param name="month">The month</param>
   /// <param name="day">The day</param>
   /// <returns>An instance of the Nullable{System.DateTime}</returns>
   public static DateTime? ParseDate(int? year, int? month, int? day)
   {
      if (!year.HasValue || !month.HasValue || !day.HasValue)
         return null;

      DateTime? date = null;
      try
      {
         date = new DateTime(year.Value, month.Value, day.Value, CultureInfo.CurrentCulture.Calendar);
      }
      catch { }
      return date;
   }

   /// <summary>
   /// Returns the file size in human readable format
   /// </summary>
   /// <param name="size">File byte size</param>
   /// <returns>Readable string</returns>
   /// <remarks ><see href="https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net"/></remarks>
   public static string ByteSizeToString(long size)
   {
      // Get absolute value
      long absolute_i = (size < 0 ? -size : size);
      // Determine the suffix and readable value
      string suffix;
      double readable;
      if (absolute_i >= 0x1000000000000000) // Exabyte
      {
         suffix = "EB";
         readable = (size >> 50);
      }
      else if (absolute_i >= 0x4000000000000) // Petabyte
      {
         suffix = "PB";
         readable = (size >> 40);
      }
      else if (absolute_i >= 0x10000000000) // Terabyte
      {
         suffix = "TB";
         readable = (size >> 30);
      }
      else if (absolute_i >= 0x40000000) // Gigabyte
      {
         suffix = "GB";
         readable = (size >> 20);
      }
      else if (absolute_i >= 0x100000) // Megabyte
      {
         suffix = "MB";
         readable = (size >> 10);
      }
      else if (absolute_i >= 0x400) // Kilobyte
      {
         suffix = "KB";
         readable = size;
      }
      else
      {
         return size.ToString("0 B"); // Byte
      }
      // Divide by 1024 to get fractional value
      readable = (readable / 1024);
      // Return formatted number with suffix
      return readable.ToString("0.### ") + suffix;
   }

   //static string ByteSizeToString(this long byteCount)
   //{
   //   string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
   //   if (byteCount == 0)
   //      return "0" + suf[0];
   //   long bytes = Math.Abs(byteCount);
   //   int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
   //   double num = Math.Round(bytes / Math.Pow(1024, place), 1);
   //   return (Math.Sign(byteCount) * num).ToString() + suf[place];
   //}

   #endregion
}
