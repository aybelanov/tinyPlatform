using Hub.Core.Domain.Localization;
using Shared.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents a sensor-to-widget mapping
/// </summary>
public class SensorWidget : BaseEntity, ILocalizedEntity
{
   /// <summary>
   /// Get or set a sensor identifier
   /// </summary>
   public long SensorId { get; set; }

   /// <summary>
   /// Get or set a sensor presentation indentifier
   /// </summary>
   public long WidgetId { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

   #region data transfer properties

   /// <summary>
   /// Sensor name
   /// </summary>
   [NotMapped]
   public string SensorName { get; set; }

   /// <summary>
   /// Widget name
   /// </summary>
   [NotMapped]
   public string WidgetName { get; set; }

   /// <summary>
   /// Mapping name
   /// </summary>
   [NotMapped, Localizable]
   public string Name {  get; set; }

   /// <summary>
   /// Mapping description
   /// </summary>
   [NotMapped, Localizable]
   public string Description { get; set; }

   #endregion
}