using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Represents a device service interface
/// </summary>
public interface IHubDeviceService
{
   /// <summary>
   /// Delete entity 
   /// </summary>
   /// <param name="device">Entity</param>
   /// <returns>Async operation</returns>
   Task DeleteAsync(Device device);

   /// <summary>
   /// Delete entities
   /// </summary>
   /// <param name="devices">Entities</param>
   /// <returns>Async operation</returns>
   Task DeleteAsync(IEnumerable<Device> devices);

   /// <summary>
   /// Get device entites by its id for specified user
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>Device entity (async operation)</returns>
   Task<Device> GetDeviceByIdAsync(long id);

   /// <summary>
   /// Get device entites by its ids for specified user
   /// </summary>
   /// <param name="ids">Collection of device ids</param>
   /// <returns>Collection of device entites (async operation)</returns>
   Task<IList<Device>> GetDevicesByIdsAsync(IList<long> ids);

   /// <summary>
   /// Gets a device entity by its systemname for specified user
   /// </summary>
   /// <param name="systemName">System name</param>
   /// <returns>Device entity</returns>
   Task<Device> GetDeviceBySystemNameAsync(string systemName);

   /// <summary>
   /// Gets own devices for a user by its identifier
   /// </summary>
   /// <param name="userId">user identifier</param>
   /// <returns>Device collection</returns>
   Task<IList<Device>> GetOwnDevicesByUserIdAsync(long userId);

   /// <summary>
   /// Gets own devices for a user by its identifier
   /// </summary>
   /// <param name="userId">user identifier</param>
   /// <returns>Device collection</returns>
   Task<IList<Device>> GetSharedDeviceByUserIdAsync(long userId);

   /// <summary>
   /// Map a device to a user 
   /// </summary>
   /// <param name="deviceId">Device entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Task (async operation)</returns>
   Task MapDeviceToUserAsync(long deviceId, long userId);

   /// <summary>
   /// Map devices to a user 
   /// </summary>
   /// <param name="deviceIds">The device identifier collection</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Task (async operation)</returns>
   Task MapDeviceToUserAsync(IEnumerable<long> deviceIds, long userId);

   /// <summary>
   /// Map devices to a user 
   /// </summary>
   /// <param name="deviceId">The device identifier</param>
   /// <param name="userIds">The user identifier colection</param>
   /// <returns>Task (async operation)</returns>
   Task MapDeviceToUserAsync(long deviceId, IEnumerable<long> userIds);

   /// <summary>
   /// Unmap a device from a user
   /// </summary>
   /// <param name="deviceId">Device entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Task (async operation)</returns>
   Task UnmapDeviceFromUserAsync(long deviceId, long userId);

   /// <summary>
   /// Insert collection of the device entities to DB
   /// </summary>
   /// <param name="entities">Device entities</param>
   /// <returns>Task (async operation)</returns>
   Task InsertDeviceAsync(IEnumerable<Device> entities);

   /// <summary>
   /// Insert a device entity to DB
   /// </summary>
   /// <param name="entity">A device entity</param>
   /// <returns>Task (async operation)</returns>
   Task<long> InsertDeviceAsync(Device entity);

   /// <summary>
   /// Update collection of the device entity
   /// </summary>
   /// <param name="entities">A device entity collection</param>
   /// <returns>Task (async operation)</returns>
   Task UpdateDeviceAsync(IEnumerable<Device> entities);

   /// <summary>
   /// Update the device entity
   /// </summary>
   /// <param name="entity">A device entity</param>
   /// <returns>Task (async operation)</returns>
   Task UpdateDeviceAsync(Device entity);

   /// <summary>
   /// Gets current device's credentials
   /// </summary>
   /// <param name="id"></param>
   /// <returns>Device credentila entity</returns>
   Task<DeviceCredential> GetCurrentDeviceCredentialAsync(long id);

   /// <summary>
   /// Adds new credentials
   /// </summary>
   /// <param name="devicePassword"></param>
   /// <returns></returns>
   Task InsertDeviceCredentialsAsync(DeviceCredential devicePassword);

   /// <summary>
   /// Gets device credentials (passwords)
   /// </summary>
   /// <param name="deviceId">Device identifier; pass null to load all records</param>
   /// <param name="passwordFormat">Password format; pass null to load all records</param>
   /// <param name="credentialsToReturn">Number of returning passwords; pass null to load all records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of user passwords
   /// </returns>
   Task<IList<DeviceCredential>> GetDeviceCredentialsAsync(long? deviceId = null,
      PasswordFormat? passwordFormat = null, int? credentialsToReturn = null);

   /// <summary>
   /// Get current device password
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device password
   /// </returns>
   Task<DeviceCredential> GetCurrentPasswordAsync(long deviceId);

   /// <summary>
   /// Devie a user password
   /// </summary>
   /// <param name="credential">Device password</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task UpdateDeviceCredentialAsync(DeviceCredential credential);

   /// <summary>
   /// Gets all devices by prameters 
   /// </summary>
   /// <param name="ids">Device identifiers</param>
   /// <param name="ownerIds">User-owner identifiers</param>
   /// <param name="userIds">Device user identifier</param>
   /// <param name="systemName">Device system name</param>
   /// <param name="name">Device name</param>
   /// <param name="createdOnFrom">Devices created datetime from (UTC)</param>
   /// <param name="createdOnTo">Devices created datetime to (UTC)</param>
   /// <param name="updatedOnFrom">Devices updated datetime from (UTC)</param>
   /// <param name="updatedOnTo">Devices updated datetime to (UTC)</param>
   /// <param name="ipAddress">Device IP-address</param>
   /// <param name="isEnabled">Device enable status</param>
   /// <param name="isActive">Device active status</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records.
   /// <param name="includeDeleted">Include deleted item</param>
   /// GetTable to "true" if you don't want to load data from database</param>
   /// <returns>Paged device collection</returns>
   Task<IPagedList<Device>> GetAllPagedDevicesAsync(long[] ids = null, long[] ownerIds = null, long[] userIds = null, string systemName = null, string name = null,
      DateTime? createdOnFrom = null, DateTime? createdOnTo = null, DateTime? updatedOnFrom = null, DateTime? updatedOnTo = null, string ipAddress = null,
      bool? isEnabled = null, bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = false);


   /// <summary>
   /// Gets online devices
   /// </summary>
   /// <param name="systemName">Device system name</param>
   /// <param name="lastActivityFromUtc">device last activity date (from)</param>
   /// <param name="lastActivityToUtc">device frist activity date (to)</param>
   /// <param name="email">Email; null to load all devices</param>
   /// <param name="company">Company; null to load all devices</param>
   /// <param name="ipAddress">IP address; null to load all devices</param>
   /// <param name="onlineIds">Show devices that has connected right now from clients app; Online device identifiers</param>
   /// <param name="online">Show online devices</param>
   /// <param name="beenRecently">Show been recently devices</param>
   /// <param name="offline">Show offline devices</param>
   /// <param name="noActivities">Show offline devices without any activities</param>
   /// <param name="utcNow">Request date time</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the devices
   /// </returns>
   Task<IPagedList<Device>> GetOnlineDevicesAsync(long[] onlineIds, string systemName = null, string email = null, string company = null, string ipAddress = null,
      DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null, bool online = false, bool beenRecently = false, bool offline = false, bool noActivities = false,
      DateTime? utcNow = null, int pageIndex = 0, int pageSize = int.MaxValue);
}