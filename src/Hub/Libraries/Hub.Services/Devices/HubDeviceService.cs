using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Clients;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Service of devices
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class HubDeviceService(IRepository<Device> deviceRepository,
   IRepository<User> userRepository,
   IRepository<DeviceCredential> deviceCredentialRepository,
   IRepository<GenericAttribute> gaRepository,
   IRepository<UserDevice> deviceUserRepository,
   IRepository<Sensor> sensorRepository,
   IRepository<SensorWidget> sensorWidgetRepository,
   IRepository<Presentation> monitorSensorWidgetRepository,
   IRepository<ActivityLog> activityLogRepository,
   DeviceSettings deviceSettings,
   IStaticCacheManager staticCacheManager) : IHubDeviceService
{

   #region Methods

   #region Delete

   /// <summary>
   /// Delete entity 
   /// </summary>
   /// <param name="device">Entity</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteAsync(Device device)
   {
      ArgumentNullException.ThrowIfNull(device);

      var sensors = sensorRepository.Table.AsNoTracking().Where(x => x.DeviceId == device.Id);

      var sensorWidgets =
         from sw in sensorWidgetRepository.Table.AsNoTracking()
         join s in sensors on sw.SensorId equals s.Id
         select sw;

      var presentations = 
         from p in monitorSensorWidgetRepository.Table.AsNoTracking()
         join sw in sensorWidgets on p.SensorWidgetId equals sw.Id
         select p;
     
      await presentations.ExecuteDeleteAsync();
      await sensorWidgets.ExecuteDeleteAsync();

      //await _monitorSensorWidgetRepository.Table.ExecuteDeleteAsync(await monitorSensorWidgets.ToListAsync());
      //await _sensorWidgetRepository.DeleteAsync(await sensorWidgets.ToListAsync());

      // soft delete
      await sensorRepository.DeleteAsync(await sensors.ToListAsync());   
      await deviceRepository.DeleteAsync(device);
   }

   /// <summary>
   /// Delete entities
   /// </summary>
   /// <param name="devices">Entities</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteAsync(IEnumerable<Device> devices)
   {
      if (devices == null)
         throw new ArgumentNullException(nameof(devices));

      await deviceRepository.DeleteAsync(devices.ToList());
   }

   #endregion

   #region Get

   #region by Entities property

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
   public virtual async Task<IPagedList<Device>> GetAllPagedDevicesAsync(long[] ids = null, long[] ownerIds = null, long[] userIds = null, string systemName = null,
      string name = null, DateTime? createdOnFrom = null, DateTime? createdOnTo = null, DateTime? updatedOnFrom = null, DateTime? updatedOnTo = null,
      string ipAddress = null, bool? isEnabled = null, bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false,
      bool includeDeleted = false)
   {
      var devices = await deviceRepository.GetAllPagedAsync(query =>
      {
         if (includeDeleted)
            query = query.IgnoreQueryFilters();

         if (ids != null)
            query = query.Where(x => ids.Contains(x.Id));

         if (ownerIds != null)
            query = query.Where(x => ownerIds.Contains(x.OwnerId));

         if (userIds != null)
         {
            query = from d in query
                    join du in deviceUserRepository.Table on d.Id equals du.DeviceId
                    where userIds.Contains(du.UserId)
                    select d;
         }

         if (!string.IsNullOrWhiteSpace(systemName))
            query = query.Where(x => EF.Functions.Like(x.SystemName, $"%{systemName}%"));

         if (!string.IsNullOrWhiteSpace(name))
            //query = query.Where(x => names.Contains(x.Name));
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{name}%")); // systemNames.Contains(x.SystemName));

         if (createdOnFrom != null)
            query = query.Where(x => x.CreatedOnUtc >= createdOnFrom);

         if (createdOnTo != null)
            query = query.Where(x => x.CreatedOnUtc <= createdOnTo);

         if (updatedOnFrom != null)
            query = query.Where(x => x.UpdatedOnUtc >= updatedOnFrom);

         if (updatedOnTo != null)
            query = query.Where(x => x.UpdatedOnUtc <= updatedOnTo);

         if (!string.IsNullOrWhiteSpace(ipAddress))
         {
            query = from d in query
                    join r in activityLogRepository.Table on d.Id equals r.SubjectId
                    where r.SubjectName == nameof(Device) && EF.Functions.Like(r.IpAddress, $"%{ipAddress}%")
                    select d;
         }

         if (isEnabled.HasValue)
            query = query.Where(x => x.Enabled == isEnabled.Value);

         if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

         query = query.OrderByDescending(x => x.CreatedOnUtc);

         return query;

      }, pageIndex, pageSize, getOnlyTotalCount);

      return devices;
   }

   /// <summary>
   /// Gets online devices
   /// </summary>
   /// <param name="systemName">Device system name</param>
   /// <param name="lastActivityFromUtc">device last activity date (from)</param>
   /// <param name="lastActivityToUtc">device frist activity date (to)</param>
   /// <param name="email">Email; null to load all devices</param>
   /// <param name="company">Company; null to load all devices</param>
   /// <param name="ipAddress">IP address; null to load all devices</param>
   /// <param name="onlineIds"> Online device identifiers</param>
   /// <param name="beenRecently">Show been recently devices</param>
   /// <param name="online">Show online devices</param>
   /// <param name="offline">Show offline devices</param>
   /// <param name="noActivities">Show offline devices without any activities</param>
   /// <param name="utcNow">Request date time</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the devices
   /// </returns>
   public virtual async Task<IPagedList<Device>> GetOnlineDevicesAsync(long[] onlineIds, string systemName = null, string email = null, string company = null, string ipAddress = null,
      DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null, bool online = false, bool beenRecently = false, bool offline = false, bool noActivities = false,
      DateTime? utcNow = null, int pageIndex = 0, int pageSize = int.MaxValue)
   {
      var now = utcNow ?? DateTime.UtcNow;

      // https://stackoverflow.com/questions/2691392/enumerable-emptyt-equivalent-for-iqueryable
      // https://stackoverflow.com/questions/11067226/how-should-i-initialize-iqueryable-variables-before-use-a-union-expression
      var query = deviceRepository.Table.AsNoTracking().Take(0);

      var deviceActivityQuery =
         (from al in activityLogRepository.Table.AsNoTracking()
          where al.SubjectName == nameof(Device)
          select al).OrderByDescending(x => x.CreatedOnUtc);

      // include devices with no activity
      if (noActivities)
      {
         var noActivityDeviceQuery =
            (from d in deviceRepository.Table.AsNoTracking()
            join alr in deviceActivityQuery on d.Id equals alr.SubjectId into attributes
            from alr in attributes.DefaultIfEmpty()
            where alr == default && !onlineIds.Contains(d.Id)
            select d).Distinct();

         query = query.Union(noActivityDeviceQuery);
      }

      // include offline evices
      if (offline)
      {
         // exclude online devices
         var offlineDeviceQuery =
           (from d in deviceRepository.Table.AsNoTracking()
           join ga in deviceActivityQuery on d.Id equals ga.SubjectId
           //where (DateTime)(object)ga.Value < now.AddMinutes(-_deviceSettings.BeenRecentlyMinutes) && !onlineIds.Contains(d.Id)
           where ga.CreatedOnUtc < now.AddMinutes(-deviceSettings.BeenRecentlyMinutes) && !onlineIds.Contains(d.Id)
           select d).Distinct();

         query = query.Union(offlineDeviceQuery);
      }

      // include "been recently" devices
      if (beenRecently)
      {
         var beenRecentlyQuery =
            (from d in deviceRepository.Table.AsNoTracking()
            join ga in deviceActivityQuery on d.Id equals ga.SubjectId
            //where (DateTime)(object)ga.Value >= now.AddMinutes(-_deviceSettings.BeenRecentlyMinutes) && !onlineIds.Contains(d.Id)
            where ga.CreatedOnUtc >= now.AddMinutes(-deviceSettings.BeenRecentlyMinutes) && !onlineIds.Contains(d.Id)
            select d).Distinct();

         query = query.Union(beenRecentlyQuery);
      }

      // include online users
      if (online)
      {
         var onlineUserQuery =
            from u in deviceRepository.Table.AsNoTracking()
            where onlineIds.Contains(u.Id)
            select u;

         query = query.Union(onlineUserQuery);
      }

      // by device system name
      if (!string.IsNullOrWhiteSpace(systemName))
         query = query.Where(x => x.SystemName.Contains(systemName));

      // by user owner email
      if (!string.IsNullOrWhiteSpace(email))
      {
         query = from d in query
                 join u in userRepository.Table.AsNoTracking() on d.OwnerId equals u.Id
                 where u.Email.Contains(email)
                 select d;
      }

      // search by user owmer company
      if (company != null)
      {
         query = from d in query
                 join u in userRepository.Table.AsNoTracking() on d.OwnerId equals u.Id
                 join ga in gaRepository.Table.AsNoTracking() on new { u.Id, KeyGroup = nameof(User) } equals new { Id = ga.EntityId, ga.KeyGroup }
                 where ga.Key == AppUserDefaults.CompanyAttribute && ga.Value.Contains(company)
                 select d;
      }

      //search by IpAddress
      if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
      {
         query = from d in query
                 join r in activityLogRepository.Table.AsNoTracking() on d.Id equals r.SubjectId
                 where r.SubjectName == nameof(Device) && r.IpAddress == ipAddress
                 select d;
      }

      query = query.OrderBy(c => c.SystemName);
      var devices = await Data.Extensions.AsyncIQueryableExtensions.ToPagedListAsync(query, pageIndex, pageSize);

      return devices;
   }


   /// <summary>
   /// Get device entites by its id for specified user
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>Device entity (async operation)</returns>
   public virtual async Task<Device> GetDeviceByIdAsync(long id)
   {
      if (id < 0)
         throw new ArgumentOutOfRangeException(nameof(id));

      var query = from u in deviceRepository.Table
                  where u.Id == id
                  select u;

      var key = staticCacheManager.PrepareKeyForShortTermCache(ClientCacheDefaults<Device>.ByIdCacheKey, id);
      return await staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }

   /// <summary>
   /// Get device entites by its ids for specified user
   /// </summary>
   /// <param name="ids">Collection of device ids</param>
   /// <returns>Collection of device entites (async operation)</returns>
   public async Task<IList<Device>> GetDevicesByIdsAsync(IList<long> ids)
   {
      return await deviceRepository.GetByIdsAsync(ids, default);
   }

   /// <summary>
   /// Gets a device entity by its systemname for specified user
   /// </summary>
   /// <param name="systemName">System name</param>
   /// <returns>Device entity</returns>
   public async Task<Device> GetDeviceBySystemNameAsync(string systemName)
   {
      if (string.IsNullOrEmpty(systemName))
         return null;

      var query = from d in deviceRepository.Table
                  where d.SystemName == systemName
                  orderby d.Id
                  select d;

      // we don't use common GetAll cache becase this method uses by incomming device methods
      var key = staticCacheManager.PrepareKeyForShortTermCache(ClientCacheDefaults<Device>.BySystemNameCacheKey, systemName);

      return await staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }


   /// <summary>
   /// Gets own devices for a user by its identifier
   /// </summary>
   /// <param name="userId">user identifier</param>
   /// <returns>Device collection</returns>
   public virtual async Task<IList<Device>> GetOwnDevicesByUserIdAsync(long userId)
   {
      var query = from d in deviceRepository.Table.AsNoTracking()
                  where d.OwnerId == userId
                  select d;

      var key = staticCacheManager.PrepareKeyForShortTermCache(ClientCacheDefaults<Device>.OwnDevicesByUserCacheKey, userId);
      return await staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
   }


   /// <summary>
   /// Gets own devices for a user by its identifier
   /// </summary>
   /// <param name="userId">user identifier</param>
   /// <returns>Device collection</returns>
   public virtual async Task<IList<Device>> GetSharedDeviceByUserIdAsync(long userId)
   {
      var query =
         from d in deviceRepository.Table.AsNoTracking()
         join du in deviceUserRepository.Table.AsNoTracking() on d.Id equals du.DeviceId
         where du.UserId == userId
         select d;

      return await query.ToListAsync();
   }



   #endregion

   #endregion

   #region Insert

   /// <summary>
   /// Insert collection of the device entities to DB
   /// </summary>
   /// <param name="entities">Device entities</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task InsertDeviceAsync(IEnumerable<Device> entities)
   {
      await deviceRepository.InsertAsync(entities.ToList());
   }

   /// <summary>
   /// Insert a device entity to DB
   /// </summary>
   /// <param name="entity">A device entity</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task<long> InsertDeviceAsync(Device entity)
   {
      await deviceRepository.InsertAsync(entity);
      return entity.Id; 
   }

   #endregion

   #region Mapping

   /// <summary>
   /// Map a device to a user 
   /// </summary>
   /// <param name="deviceId">Device entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task MapDeviceToUserAsync(long deviceId, long userId)
   {
      if (deviceId < 1)
         throw new ArgumentOutOfRangeException(nameof(deviceId));

      if (userId < 1)
         throw new ArgumentOutOfRangeException(nameof(userId));

      await deviceUserRepository.InsertAsync(new UserDevice() { DeviceId = deviceId, UserId = userId });
   }

   /// <summary>
   /// Map devices to a user 
   /// </summary>
   /// <param name="deviceIds">The device collection</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task MapDeviceToUserAsync(IEnumerable<long> deviceIds, long userId)
   {
      if (userId < 1)
         throw new ArgumentOutOfRangeException(nameof(userId));

      ArgumentNullException.ThrowIfNull(deviceIds);

      if (!deviceIds.Any())
         return;

      var maps = deviceIds.Select(x => new UserDevice() { UserId = userId, DeviceId = x });

      await deviceUserRepository.InsertAsync(maps.ToList());
   }


   /// <summary>
   /// Map devices to a user 
   /// </summary>
   /// <param name="deviceId">The device identifier</param>
   /// <param name="userIds">User identifier colection</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task MapDeviceToUserAsync(long deviceId, IEnumerable<long> userIds)
   {
      if (deviceId < 1)
         throw new ArgumentOutOfRangeException(nameof(deviceId));

      ArgumentNullException.ThrowIfNull(userIds);

      if (!userIds.Any())
         return;

      var maps = userIds.Select(x => new UserDevice() { UserId = x, DeviceId = deviceId });

      await deviceUserRepository.InsertAsync(maps.ToList());
   }

   /// <summary>
   /// Unmap a device from a user
   /// </summary>
   /// <param name="deviceId">Device entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task UnmapDeviceFromUserAsync(long deviceId, long userId)
   {
      var map = deviceUserRepository.Table.FirstOrDefault(x=>x.DeviceId == deviceId && x.UserId == userId);

      if (map != null)
         await deviceUserRepository.DeleteAsync(map);
   }

   #endregion

   #region Update

   /// <summary>
   /// Update collection of the device entity
   /// </summary>
   /// <param name="entities">A device entity collection</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task UpdateDeviceAsync(IEnumerable<Device> entities)
   {
      await deviceRepository.UpdateAsync(entities.ToList());
   }

   /// <summary>
   /// Update the device entity
   /// </summary>
   /// <param name="entity">A device entity</param>
   /// <returns>Task (async operation)</returns>
   public virtual async Task UpdateDeviceAsync(Device entity)
   {
      await deviceRepository.UpdateAsync(entity);
   }

   #endregion

   #region Credentials

   /// <summary>
   /// Gets device credentials (passwords)
   /// </summary>
   /// <param name="deviceId">Device identifier; pass null to load all records</param>
   /// <param name="passwordFormat">Password format; pass null to load all records</param>
   /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of user passwords
   /// </returns>
   public virtual async Task<IList<DeviceCredential>> GetDeviceCredentialsAsync(long? deviceId = null,
       PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
   {
      var query = deviceCredentialRepository.Table;

      //filter by user
      if (deviceId.HasValue)
         query = query.Where(password => password.DeviceId == deviceId.Value);

      //filter by password format
      if (passwordFormat.HasValue)
         query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

      //get the latest passwords
      if (passwordsToReturn.HasValue)
         query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

      return await query.ToListAsync();
   }

   /// <summary>
   /// Get current device password
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device password
   /// </returns>
   public virtual async Task<DeviceCredential> GetCurrentPasswordAsync(long deviceId)
   {
      if (deviceId == 0)
         return null;

      //return the latest password
      return (await GetDeviceCredentialsAsync(deviceId, passwordsToReturn: 1)).FirstOrDefault();
   }

   /// <summary>
   /// Get current device password
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user password
   /// </returns>
   public virtual async Task<DeviceCredential> GetCurrentDeviceCredentialAsync(long deviceId)
   {
      if (deviceId == 0)
         return null;

      //return the latest password
      return (await GetDeviceCredentialsAsync(deviceId, passwordsToReturn: 1)).FirstOrDefault();
   }


   /// <summary>
   /// Insert a device credential
   /// </summary>
   /// <param name="credential">User password</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertDeviceCredentialsAsync(DeviceCredential credential)
   {
      await deviceCredentialRepository.InsertAsync(credential);
   }


   /// <summary>
   /// Devie a user password
   /// </summary>
   /// <param name="credential">Device password</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateDeviceCredentialAsync(DeviceCredential credential)
   {
      await deviceCredentialRepository.UpdateAsync(credential);
   }

   #endregion

   #endregion
}