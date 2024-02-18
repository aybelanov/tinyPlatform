using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Widgets;
using Shared.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Widget presentation service interface
/// </summary>
public interface IWidgetService
{
   /// <summary>
   /// Delete a widget by the model
   /// </summary>
   /// <param name="model">Widget model</param>
   /// <returns></returns>
   Task DeleteAsync(WidgetModel model);

   /// <summary>
   /// Get all widgets
   /// </summary>
   /// <returns>Widget entity collection</returns>
   Task<IFilterableList<Widget>> GetAllWidgetsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets all widget entities of the current user
   /// </summary>
   /// <returns>Widget collection</returns>
   Task<IFilterableList<Widget>> GetUserWidgetsAsync(DynamicFilter filter);

   /// <summary>
   /// Get a widget by identifier
   /// </summary>
   /// <param name="id">Entity inedtifier</param>
   /// <returns>Widget entity</returns>
   Task<Widget> GetByIdAsync(long id);

   /// <summary>
   /// Get widgets by identifiers
   /// </summary>
   /// <param name="ids">Entity inedtifiers</param>
   /// <returns>Widget entity collection</returns>
   Task<IFilterableList<Widget>> GetByIdsAsync(IEnumerable<long> ids);

   /// <summary>
   /// Gets all device select item list (for the admin mode)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   Task<IFilterableList<WidgetSelectItem>> GetAllWidgetSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Gets device select item list for the current user
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   Task<IFilterableList<WidgetSelectItem>> GetUserWidgetSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Insert a widget from the model
   /// </summary>
   /// <param name="model">Moitor model</param>
   /// <returns></returns>
   Task InsertAsync(WidgetModel model);

   /// <summary>
   /// Update a widget from the model
   /// </summary>
   /// <param name="model">Widget model</param>
   /// <returns></returns>
   Task UpdateAsync(WidgetModel model);
}