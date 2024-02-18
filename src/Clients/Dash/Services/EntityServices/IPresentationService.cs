using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Monitors;
using Shared.Clients;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Presentation entity service interface
/// </summary>
public interface IPresentationService
{
   /// <summary>
   /// Gets all sensor-to-widget mapping select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   Task<IFilterableList<PresentationSelectItem>> GetAllPresentationSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Gets own (user) sensor-to-widget mapping select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   Task<IFilterableList<PresentationSelectItem>> GetOwnPresentationSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Gets a presentation select item by the identifier 
   /// </summary>
   /// <param name="sensorWidgetId">Sensor widget identifier</param>
   /// <returns>Presentation select item</returns>
   Task<PresentationSelectItem> GetPresentationSelectItemAsync(long sensorWidgetId);

   /// <summary>
   /// Gets monitor'presentation by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Monitor presentation entity collection</returns>
   Task<IFilterableList<Presentation>> GetPresentationsAsync(DynamicFilter filter);

   /// <summary>
   /// Maps a presentation (sensor-to-widget) to a monitor
   /// </summary>
   /// <param name="model">Monitor mapping model</param>
   /// <returns></returns>
   Task MapPresentationToMonitorAsync(PresentationModel model);

   /// <summary>
   /// Maps a presentation (sensor-to-widget) to a monitor
   /// </summary>
   /// <param name="model">Monitor mapping model</param>
   /// <returns></returns>
   Task UpdateMapPresentationToMonitorAsync(PresentationModel model);

   /// <summary>
   /// Unmaps a presentation (sensor-widget) from a monitor
   /// </summary>
   /// <param name="model">Monitor mapping model</param>
   /// <returns></returns>
   Task UnmapPresentaionFromMonitorAsync(PresentationModel model);

   /// <summary>
   /// Maps a sensor to a widget
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns></returns>
   Task MapSensorToWidgetAsync(long sensorId, long widgetId);

   /// <summary>
   /// Unmaps a sensor to a widget
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns></returns>
   Task UnmapSensorFromWidgetAsync(long sensorId, long widgetId);
}
