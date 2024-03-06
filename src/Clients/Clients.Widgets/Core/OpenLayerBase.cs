using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json.Serialization;

namespace Clients.Widgets.Core;

/// <summary>
/// Open layer base class
/// </summary>
public abstract class OpenLayerBase : WidgetBase
{
   #region Props 

   /// <summary>
   /// Open street layer
   /// </summary>
   [Parameter]
   public MapView View { get; set; }

   /// <summary>
   /// Bing map api key
   /// </summary>
   [Parameter, EditorRequired]
   public string BingKey { get; set; }

   #endregion

   #region Ctors

   /// <summary>
   /// Default Ctor
   /// </summary>
   public OpenLayerBase()
   {
      View = new()
      {
         Zoom = 12,
         LayerType = LayerTypes.OSM,
         Culture = "en-US",
         Theme = "light",
         Center = new()
         {
            Lon = 103.829658,
            Lat = 1.348738,
            Color = "rgb(255, 99, 132)",
            Height = 100,
            Ticks = DateTime.UtcNow.Ticks,
         }
      };
   }

   #endregion

   #region nested classes

   /// <summary>
   /// Represents a yandex map view
   /// </summary>
   public class MapView
   {
      /// <summary>
      /// Map theme
      /// </summary>
      public string Theme { get; set; }

      /// <summary>
      /// Map culture
      /// </summary>
      public string Culture { get; set; }

      /// <summary>
      /// Map type 
      /// </summary>
      public LayerTypes LayerType { get; set; }

      /// <summary>
      /// Center of a map
      /// </summary>
      public GeoPoint Center { get; set; }

      /// <summary>
      /// Map zoom
      /// </summary>
      public int? Zoom { get; set; }
   }

   /// <summary>
   /// Model of geo coordinates
   /// </summary>
   public class GeoPoint : IEquatable<GeoPoint>
   {
      /// <summary>
      /// Longtitude
      /// </summary>
      public double Lon { get; set; }

      /// <summary>
      /// Latitude
      /// </summary>
      public double Lat { get; set; }

      /// <summary>
      /// Height
      /// </summary>
      public double Height { get; set; }

      /// <summary>
      /// Speed
      /// </summary>
      public double Speed { get; set; }

      /// <summary>
      /// Course
      /// </summary>
      public double Course { get; set; }

      /// <summary>
      /// Timestamp of registration the point
      /// </summary>
      public long Ticks { get; set; }

      /// <inheritdoc />
      public bool Equals(GeoPoint other)
      {
         if (other != null)
            return Lat == other.Lat && Lon == other.Lon;

         return false;
      }

      /// <inheritdoc/>
      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      ///<inheritdoc />
      public override bool Equals(object obj)
      {
         return Equals(obj as GeoPoint);
      }

      /// <summary>
      /// Point color
      /// </summary>
      public string Color { get; set; }
   }

   /// <summary>
   /// Open layers map types
   /// </summary>
   [JsonConverter(typeof(JsonStringEnumConverter))]
   public enum LayerTypes
   {
      OSM = 1,
      Aerial,
      AerialWithLabelsOnDemand,
      Birdseye,
      BirdseyeWithLabels,
      BirdseyeV2,
      BirdseyeV2WithLabels,
      CanvasDark,
      CanvasLight,
      CanvasGray,
      OrdnanceSurvey,
      RoadOnDemand,
      Streetside
   }

   /// <summary>
   /// Represents map object that is shown as a marker
   /// </summary>
   public class Marker
   {
      public long EntityId { get; set; }
      public string Icon { get; set; }
      public string Name { get; set; }
      public string Content { get; set; }
      public double Lat { get; set; }
      public double Lon { get; set; }
      public bool Visible { get; set; }
      public string TextColor { get; set; }
      public string Link { get; set; }
   }

   /// <summary>
   /// Represents a map type's collection
   /// </summary>
   public class MapSelectItem
   {
      /// <summary>
      /// Map type
      /// </summary>
      public LayerTypes MapType { get; set; }

      /// <summary>
      /// Localized name of the map type
      /// </summary>
      public string Locale { get; set; }
   }


   #endregion
}
