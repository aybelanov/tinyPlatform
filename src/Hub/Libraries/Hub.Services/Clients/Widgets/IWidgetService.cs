using Hub.Core.Domain.Clients;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Widgets;

/// <summary>
/// Represents a dashboard client common service interface 
/// </summary>
public interface IWidgetService
{
    /// <summary>
    /// Deletes widget from the datavase
    /// </summary>
    /// <param name="widget">Widget entity</param>
    /// <returns></returns>
    Task DeleteAsync(Widget widget);

    /// <summary>
    /// Gets a widget by its identifier
    /// </summary>
    /// <param name="widgetId">Widget identifier</param>
    /// <returns>Widget entity</returns>
    Task<Widget> GetByIdAsync(long widgetId);

    /// <summary>
    /// Gets widgets by the dynamic filter
    /// </summary>
    /// <param name="filter">Dinamic filter</param>
    /// <returns>Widget filterable collection</returns>
    Task<IFilterableList<Widget>> GetAllWidgetsAsync(DynamicFilter filter);

    /// <summary>
    /// Gets all widget select list by the dynamic filter
    /// </summary>
    /// <param name="filter">Dinamic filter</param>
    /// <returns>Widget filterable collection</returns>
    Task<IFilterableList<WidgetSelectItem>> GetAllWidgetSelectListAsync(DynamicFilter filter);

    /// <summary>
    /// Gets own widget select list by the dynamic filter
    /// </summary>
    /// <param name="filter">Dinamic filter</param>
    /// <returns>Widget filterable collection</returns>
    Task<IFilterableList<WidgetSelectItem>> GetOwnWidgetSelectListAsync(DynamicFilter filter);

    /// <summary>
    /// Adds a widget to the database
    /// </summary>
    /// <param name="widget">Widget entity</param>
    /// <returns>Widget identifier</returns>
    Task<long> InsertAsync(Widget widget);

    /// <summary>
    /// Update a widget
    /// </summary>
    /// <param name="widget">Widget entity</param>
    /// <param name="updateLocale">Update locales</param>
    /// <returns></returns>
    Task UpdateAsync(Widget widget, bool updateLocale = true);

    /// <summary>
    /// Is a widget in a user scope
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="widgetId">Widget identifier</param>
    /// <returns>Result: true - in scope; false - not in scope.</returns>
    Task<bool> IsInUserScopeAsync(long userId, long widgetId);
}