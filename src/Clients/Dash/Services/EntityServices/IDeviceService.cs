using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Devices;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;


/// <summary>
/// Device service interface
/// </summary>
public interface IDeviceService
{
   /// <summary>
   /// Delete a device by the model
   /// </summary>
   /// <param name="model">Model</param>
   /// <returns></returns>
   Task DeleteAsync(DeviceModel model);

   /// <summary>
   /// Deletes a device entity by the sensor model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   Task DeleteSharedAsync(DeviceModel model);

   /// <summary>
   /// Get own devices by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   Task<IFilterableList<Device>> GetAllDevicesAsync(DynamicFilter filter);

   /// <summary>
   /// Get own devices by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   Task<IFilterableList<Device>> GetOwnDevicesAsync(DynamicFilter filter);

   /// <summary>
   /// Gets shared devices by the dynamic filter
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
   /// Gets all device select item list (for admin mode)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   Task<IFilterableList<DeviceSelectItem>> GetAllDeviceSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Gets devce select item list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Filterable device select item collection</returns>
   Task<IFilterableList<DeviceSelectItem>> GetUserDeviceSelectListAsync(DynamicFilter filter);

   /// <summary>
   /// Get a device by identifier
   /// </summary>
   /// <param name="id">Entity inedtifier</param>
   /// <returns>Device entity</returns>
   Task<Device> GetByIdAsync(long id);

   /// <summary>
   /// Get devices by identifiers
   /// </summary>
   /// <param name="ids">Entity inedtifiers</param>
   /// <returns>Device entity collection</returns>
   Task<IFilterableList<Device>> GetByIdsAsync(IEnumerable<long> ids);

   /// <summary>
   /// Insert a device from the model
   /// </summary>
   /// <param name="model">Model</param>
   /// <returns></returns>
   Task InsertAsync(DeviceModel model);

   /// <summary>
   /// Update a device from the model
   /// </summary>
   /// <param name="model">Model</param>
   /// <returns></returns>
   Task UpdateAsync(DeviceModel model);

   /// <summary>
   /// Updates a shared device entity 
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   Task UpdateSharedAsync(DeviceModel model);

   /// <summary>
   /// Changes a device password
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="password">new password</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   Task ChangePassword(long deviceId, string password);

   /// <summary>
   /// Ensures that the system name is occupied 
   /// </summary>
   /// <param name="password">Password</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Validation result</returns>
   Task<CommonResponse> CheckPasswordFormatAsync(string password, long deviceId);

   /// <summary>
   /// Ensures that the system name is not occupied 
   /// </summary>
   /// <param name="systemName">System name</param>
   /// <returns>Validation result</returns>
   Task<CommonResponse> CheckSystemNameAvailabilityAsync(string systemName);

   /// <summary>
   /// Shares a device to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   Task ShareDeviceAsync(string userName, long deviceId);

   /// <summary>
   /// Unshares a device to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   Task UnshareDeviceAsync(string userName, long deviceId);
}