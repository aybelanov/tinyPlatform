using Hub.Core.Domain.Clients;
using Shared.Clients;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Devices;

/// <summary>
/// Represents a dashboard client device service interface 
/// </summary>
public interface IDeviceService
{
   /// <summary>
   /// Deletes devices
   /// </summary>
   /// <param name="device">Device entity</param>
   /// <returns></returns>
   Task DeleteAsync(Device device);

   /// <summary>
   /// Gets all devices by filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   Task<IFilterableList<Device>> GetAllDevicesAsync(DynamicFilter filter);

   /// <summary>
   /// Gets own (by user) devices  by filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   Task<IFilterableList<Device>> GetOwnDevicesAsync(DynamicFilter filter);

   /// <summary>
   /// Gets shared (granted) devices  by filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   Task<IFilterableList<Device>> GetSharedDevicesAsync(DynamicFilter filter);

   /// <summary>
   /// Gets device map item collection (for admin mode) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device map item collection</returns>
   Task<IFilterableList<DeviceMapItem>> GetAllDeviceMapItemsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets device map item collection (for current user) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device map item collection</returns>
   Task<IFilterableList<DeviceMapItem>> GetUserDeviceMapItemsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets device select items for client selectable UI elements like a "dropdown list" 
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item colection</returns>
   Task<IFilterableList<DeviceSelectItem>> GetAllDeviceSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Gets device select items for client selectable UI elements like a "dropdown list" 
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item colection</returns>
   Task<IFilterableList<DeviceSelectItem>> GetUserDeviceSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Inserts device to database
   /// </summary>
   /// <param name="device">Adding device entity</param>
   /// <returns>Device identifier</returns>
   Task<long> InsertDeviceAsync(Device device);

   /// <summary>
   /// Updates existing device
   /// </summary>
   /// <param name="device">Device entity</param>
   /// <returns></returns>
   Task UpdateDeviceAsync(Device device);

   /// <summary>
   /// Gets all user devices (own and shared)
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Result query</returns>
   IQueryable<Device> UserScope(long? userId);

   /// <summary>
   /// Is the device into the user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>result: true - into the scope, false - not into the scope</returns>
   Task<bool> IsUserDeviceAsync(long userId, long deviceId);

   /// <summary>
   /// Shares device to user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   Task ShareDeviceAsync(long userId, long deviceId);

   /// <summary>
   /// Stop sharing device to user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   Task UnshareDeviceAsync(long userId, long deviceId);
}
