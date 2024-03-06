using Devices.Dispatcher.Domain;
using Devices.Dispatcher.Services.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// Represents a common helper
/// </summary>
public static class CommonHelper
{
   /// <summary>
   /// Seeds initial settings
   /// </summary>
   /// <typeparam name="T">Setting class type</typeparam>
   /// <param name="settings">setting values</param>
   /// <returns>Setting collection</returns>
   /// <exception cref="ArgumentNullException"></exception>
   public static IEnumerable<Setting> SettingsSeed<T>(T settings) where T : ISettings, new()
   {
      var i = 0;
      foreach (var prop in typeof(T).GetProperties())
      {
         // get properties we can read and write to
         if (!prop.CanRead || !prop.CanWrite)
            continue;

         if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
            continue;

         var key = typeof(T).Name + "." + prop.Name;
         var value = prop.GetValue(settings, null);

         if (key == null)
            throw new ArgumentNullException(nameof(key));

         key = key.Trim().ToLowerInvariant();

         var valueStr = string.Empty;

         if (value != null)
            valueStr = TypeDescriptor.GetConverter(prop.PropertyType).ConvertToInvariantString(value);

         var setting = new Setting
         {
            Id = ++i,
            Name = key,
            Value = valueStr,
         };

         yield return setting;
      }
   }

   /// <summary>
   /// An extension method to determine if an IP address is internal, as specified in RFC1918
   /// https://stackoverflow.com/questions/8113546/how-to-determine-whether-an-ip-address-is-private
   /// </summary>
   /// <param name="ipAddress">The IP address that will be tested</param>
   /// <returns>Returns true if the IP is internal, false if it is external</returns>
   public static bool IsIP4InternalIP(this IPAddress ipAddress)
   {
      if (IPAddress.IsLoopback(ipAddress))
         return true;
      else if (ipAddress.ToString() == "::1")
         return false;

      var bytes = ipAddress.MapToIPv4().GetAddressBytes();
      return bytes[0] switch
      {
         10 => true,
         172 => bytes[1] < 32 && bytes[1] >= 16,
         192 => bytes[1] == 168,
         _ => false,
      };
   }
}
