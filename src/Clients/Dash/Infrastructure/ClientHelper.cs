using Clients.Dash.Configuration;
using Clients.Dash.Domain;
using Clients.Widgets.Core;
using Radzen.Blazor;
using Shared.Clients;
using Shared.Common;
using Shared.Common.Helpers;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// Represents a common helper
/// </summary>
public class ClientHelper : CommonHelper
{
   /// <summary>
   /// Convert UTC date to local (by browser) time
   /// </summary>
   /// <param name="dateTime">Initial UTC datetiem</param>
   /// <returns>Local datetime</returns>
   public static DateTime ConvertUtcToBrowserTime(DateTime dateTime)
   {
      var result = ((DateTimeOffset)dateTime).ToOffset(Defaults.BrowserTimeOffset).DateTime;
      return result;
   }

   /// <summary>
   /// Convert UTC date to local (by browser) time
   /// </summary>
   /// <param name="dateTime">Initial UTC datetiem</param>
   /// <returns>Local datetime</returns>
   public static Task<DateTime> ConvertUtcToBrowserTimeAsync(DateTime dateTime)
   {
      return Task.FromResult(ConvertUtcToBrowserTime(dateTime));
   }

   /// <summary>
   /// Convert local (by browser) time date to UTC
   /// </summary>
   /// <param name="dateTime">Initial browser datetiem</param>
   /// <returns>Local datetime</returns>
   public static DateTime ConvertBrowserTimeToUtc(DateTime dateTime)
   {
      var result = ((DateTimeOffset)dateTime).ToOffset(-Defaults.BrowserTimeOffset).UtcDateTime;
      return result;
   }

   /// <summary>
   /// Convert local (by browser) time date to UTC
   /// </summary>
   /// <param name="dateTime">Initial browser datetiem</param>
   /// <returns>Local datetime</returns>
   public static Task<DateTime> ConvertBrowserTimeToUtcAsync(DateTime dateTime)
   {
      return Task.FromResult(ConvertBrowserTimeToUtc(dateTime));
   }

   /// <summary>
   /// Convert column local DateTime filter to UTC
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="dataGridColumn"></param>
   public static void ConvertColumnFilterToUtc<T>(RadzenDataGridColumn<T> dataGridColumn) where T : class 
   {
      var firstValue = dataGridColumn.GetFilterValue();
      if(firstValue != null)
      {
         var localTime = DateTime.Parse(firstValue.ToString());
         var utc = DateTime.SpecifyKind(ConvertBrowserTimeToUtc(localTime), DateTimeKind.Unspecified);
         dataGridColumn.SetFilterValue(utc);
      }

      var secondValue = dataGridColumn.GetSecondFilterValue();
      if (secondValue != null)
      {
         var localTime = DateTime.Parse(secondValue.ToString());
         var utc = DateTime.SpecifyKind(ConvertBrowserTimeToUtc(localTime), DateTimeKind.Unspecified);
         dataGridColumn.SetFilterValue(utc, false);
      }
   }

   /// <summary>
   /// Gets map marker status color
   /// </summary>
   /// <param name="onlineStatus">Online status (users, devices)</param>
   /// <returns>Color name or code</returns>
   public static string GetMapMarkerStatusColor(OnlineStatus onlineStatus)
   {
      return onlineStatus switch
      {
         OnlineStatus.Online => "green",
         OnlineStatus.BeenRecently => "yellow",
         OnlineStatus.Offline => "red",
         OnlineStatus.NoActivities => "gray",
         _ => "gray"
      };
   }

   /// <summary>
   /// Gets map marker text color by online status
   /// </summary>
   /// <param name="onlineStatus">Online status (users, devices)</param>
   /// <returns>Color name or code</returns>
   public static string GetMapMarkerTextColor(OnlineStatus onlineStatus)
   {
      return onlineStatus switch
      {
         OnlineStatus.Online => "rgba(0, 170, 0, 1)",
         OnlineStatus.BeenRecently => "rgba(255, 159, 64, 1)",
         OnlineStatus.Offline => "rgba(255, 99, 132, 1)",
         OnlineStatus.NoActivities => "rgba(155, 155, 155, 1)",
         _ => "rgba(155, 155, 155, 1)"
      };
   }

   /// <summary>
   /// Parses a geo sensor record to Openlayer geopoint
   /// </summary>
   /// <param name="record">Sensor record</param>
   /// <returns>Openlayer geopoint</returns>
   public static OpenLayerBase.GeoPoint ParseGeoRecord(SensorRecord record)
   {
      if(record == null)
         return null;

      var ticks = new DateTime(BitConverter.ToInt64(record.Bytes.AsSpan()[0..8])).ToUnixEpochTime() * 1000;
      var lon = BitConverter.ToDouble(record.Bytes.AsSpan()[8..16]);
      var lat = BitConverter.ToDouble(record.Bytes.AsSpan()[16..24]);
      var speed = Math.Round(BitConverter.ToDouble(record.Bytes.AsSpan()[24..32]), 0);
      var height = Math.Round(BitConverter.ToDouble(record.Bytes.AsSpan()[32..40]), 0);
      //var reliable = BitConverter.ToBoolean(record.Bytes.AsSpan()[40..41]);
      var course = Math.Round(BitConverter.ToDouble(record.Bytes.AsSpan()[41..49]), 0);

      var point = new OpenLayerBase.GeoPoint()
      {
         Lon = lon,
         Lat = lat,
         Height = height,
         Speed = speed,
         Course = course,
         Ticks = ticks,
      };

      return point;
   }


   /// <summary>
   /// Normalize the json string
   /// </summary>
   /// <param name="jsonString">Incomming string</param>
   /// <param name="indented">Is intended</param>
   /// <returns>Normalized json string</returns>
   public static string NormalizeJsonString(string jsonString, bool indented)
   {
      if (!string.IsNullOrEmpty(jsonString))
      {
         var intemediateObject = JsonSerializer.Deserialize<object>(jsonString);
         var options = new JsonSerializerOptions() { WriteIndented = indented };
         jsonString = JsonSerializer.Serialize(intemediateObject, options);
      }

      return jsonString?.Trim();
   }
}
