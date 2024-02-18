using Hub.Core.Domain.Clients;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Monitors;

/// <summary>
/// Represents a presentation interface 
/// </summary>
public interface IPresentationService
{
    /// <summary>
    /// Gets monitor presentation collection by the dynamic filter
    /// </summary>
    /// <param name="filter">Dynamic filter</param>
    /// <returns>Filterable collection of the monitor prentations</returns>
    Task<IFilterableList<Presentation>> GetPresentationsAsync(DynamicFilter filter);

    /// <summary>
    /// Gets presentation entity by the identifier
    /// </summary>
    /// <param name="presentationId">Presentation identifier</param>
    /// <returns>Presentation entity</returns>
    Task<Presentation> GetPresentationByIdAsync(long presentationId);

    /// <summary>
    /// Map a monitor to a sensor and to a sensor presentation
    /// </summary>
    /// <param name="map">Monitor presentation entity</param>
    /// <returns>Async task</returns>
    Task MapPresentationAsync(Presentation map);

    /// <summary>
    /// Update a monitor to a sensor-to-widget presentation
    /// </summary>
    /// <param name="map">Monitor presentation entity</param>
    /// <returns>Async task</returns>
    Task UpdateMapPresentationAsync(Presentation map);

    /// <summary>
    /// Unmap a monitor to a sensor-to-widget presentation
    /// </summary>
    /// <param name="map">Monitor presentation mapping</param>
    /// <returns>Async task</returns>
    Task UnmapPresentationAsync(Presentation map);

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

    /// <summary>
    /// Is a sensor-to-widget map in the user scope
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="sensorWidgetId">Sensor-to-widget identifier</param>
    /// <returns>Result: true - into the scope, false - not inot the scope</returns>
    Task<bool> IsSensorWidgetInUserScopeAsync(long userId, long sensorWidgetId);
}
