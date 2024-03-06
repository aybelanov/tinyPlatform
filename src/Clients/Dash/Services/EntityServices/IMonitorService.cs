using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Monitors;
using Shared.Clients;
using System.Threading.Tasks;
using Monitor = Clients.Dash.Domain.Monitor;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Monitor entity service interface
/// </summary>
public interface IMonitorService
{
   /// <summary>
   /// Delete a monitor by the model
   /// </summary>
   /// <param name="model">Monitor model</param>
   /// <returns></returns>
   Task DeleteAsync(MonitorModel model);

   /// <summary>
   /// Deletes a device entity by the sensor model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   Task DeleteSharedAsync(MonitorModel model);

   /// <summary>
   /// Gets a monitor view
   /// </summary>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns>Monitor view</returns>
   Task<MonitorView> GetMonitorViewAsync(long monitorId);

   /// <summary>
   /// Get all monitors (for admins only) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>All monitor entity collection</returns>
   Task<IFilterableList<Monitor>> GetAllMonitorsAsync(DynamicFilter filter);

   /// <summary>
   /// Get all own monitors by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Own monitor entity collection</returns>
   Task<IFilterableList<Monitor>> GetOwnMonitorsAsync(DynamicFilter filter);

   /// <summary>
   /// Get all own monitors by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Shared monitor entity collection</returns>
   Task<IFilterableList<Monitor>> GetSharedMonitorsAsync(DynamicFilter filter);

   /// <summary>
   /// Get a monitor by identifier
   /// </summary>
   /// <param name="id">Entity inedtifier</param>
   /// <returns>Monitor entity</returns>
   Task<Monitor> GetByIdAsync(long id);

   /// <summary>
   /// Insert a monitor from the model
   /// </summary>
   /// <param name="model">Moitor model</param>
   /// <returns></returns>
   Task InsertAsync(MonitorModel model);

   /// <summary>
   /// Update a monitor from the model
   /// </summary>
   /// <param name="model">Monitor model</param>
   /// <returns></returns>
   Task UpdateAsync(MonitorModel model);

   /// <summary>
   /// Updates a shared device entity 
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   Task UpdateSharedAsync(MonitorModel model);

   /// <summary>
   /// Shares a monitor to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns></returns>
   Task ShareMonitorAsync(string userName, long monitorId);

   /// <summary>
   /// Unshares a monitor to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns></returns>
   Task UnshareMonitorAsync(string userName, long monitorId);
}