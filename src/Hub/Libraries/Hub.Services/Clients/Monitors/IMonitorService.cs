using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Shared.Clients;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Monitors;

/// <summary>
/// Represents a dashboard client monitor service interface 
/// </summary>
public interface IMonitorService
{
   /// <summary>
   /// Delete monitor entities
   /// </summary>
   /// <param name="monitor">Deleting monitor entity</param>
   /// <returns>Async operation</returns>
   Task DeleteAsync(Monitor monitor);

   /// <summary>
   /// Get all my monitor entities by dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Collection of monitors (async operation)</returns>
   /// <remarks>For admins this returns all monitors</remarks>
   Task<IFilterableList<Monitor>> GetOwnMonitorsAsync(DynamicFilter filter);

   /// <summary>
   /// Get shared monitor entities by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Collection of monitors (async operation)</returns>
   Task<IFilterableList<Monitor>> GetSharedMonitorsAsync(DynamicFilter filter);

   /// <summary>
   /// Get a monitor by its id for specified user
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>The monitor entity (async operation)</returns>
   Task<Monitor> GetByIdAsync(long id);

   /// <summary>
   /// Insert a monitor entity to DB
   /// </summary>
   /// <param name="entity">Monitor entity</param>
   /// <returns>Identifier</returns>
   Task<long> InsertMonitorAsync(Monitor entity);

   /// <summary>
   /// Get all monitor entities  by dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>The collection of devices (async operation)</returns>
   Task<IFilterableList<Monitor>> GetAllMonitorsAsync(DynamicFilter filter);

   /// <summary>
   /// Map a monitor to a user
   /// </summary>
   /// <param name="monitorId">Monitor entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Async operation</returns>
   Task ShareMonitorAsync(long monitorId, long userId);

   /// <summary>
   /// Unmap a monitor to a user
   /// </summary>
   /// <param name="monitorId">Monitor entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Async operation</returns>
   Task UnshareMonitorAsync(long monitorId, long userId);

   /// <summary>
   /// Update monitor entity
   /// </summary>
   /// <param name="entity">Monitor entity</param>
   /// <returns>Async operation</returns>
   Task UpdateMonitorAsync(Monitor entity);

   /// <summary>
   /// Update shared monitor entity (show in menu and display order)
   /// </summary>
   /// <param name="monitor">Monitor entity</param>
   /// <param name="user">User</param>
   /// <returns>Async operation</returns>
   Task UpdateSharedMonitorAsync(Monitor monitor, User user);

   /// <summary>
   /// Is the monitor into the user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns>result: true - into the scope, false - not into the scope</returns>
   Task<bool> IsInUserScopeAsync(long userId, long monitorId);

   /// <summary>
   /// Gets a monitor view
   /// </summary>
   /// <param name="monitorId">Monitor identifier</param>
   /// <param name="userId">User identifier</param>
   /// <returns>Monitor view</returns>
   Task<Monitor> GetMonitorViewAsync(long monitorId, long? userId);

   /// <summary>
   /// User scope for monitors
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="viaJoin">Query scope via join</param>
   /// <returns>User scope query</returns>
   IQueryable<Monitor> UserScope(long? userId, bool viaJoin = false);
}
