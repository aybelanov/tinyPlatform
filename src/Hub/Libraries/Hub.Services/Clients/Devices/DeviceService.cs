using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Hub.Services.Users;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Devices;

/// <summary>
/// Represents a dashboard client device service implementation 
/// </summary>
public class DeviceService : HubDeviceService, IDeviceService
{

   #region fields

   private readonly IWorkContext _workContext;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IRepository<Device> _deviceRepository;
   private readonly IRepository<User> _userRepository;
   private readonly IRepository<UserDevice> _userDeviceRepository;
   private readonly IRepository<GenericAttribute> _gaRepository;
   private readonly IRepository<ActivityLog> _activityLogRepository;
   private readonly IRepository<LocalizedProperty> _localizedRepository;
   private readonly ICommunicator _communicator;
   private readonly DeviceSettings _deviceSettings;
   private readonly UserSettings _userSettings;
   private readonly IUserService _userService;
   private readonly ILocalizer _localizer;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Cotr
   /// </summary>
   public DeviceService(IWorkContext workContext,
      IRepository<Device> deviceRepository,
      IRepository<User> userRepository,
      IRepository<DeviceCredential> deviceCredentialRepository,
      IRepository<GenericAttribute> gaRepository,
      IRepository<UserDevice> deviceUserRepository,
      IRepository<Sensor> sensorRepository,
      IRepository<SensorWidget> sensorWidgetRepository,
      IRepository<Presentation> monitorSensorWidgetRepository,
      IRepository<ActivityLog> activityLogRepository,
      DeviceSettings deviceSettings,
      IStaticCacheManager staticCacheManager,
      IRepository<LocalizedProperty> localizedRepository,
      IRepository<UserDevice> userDeviceRepository,
      IUserService userService,
      ILocalizer localizer,
      ICommunicator communicator,
      UserSettings userSettings)
      : base(deviceRepository, userRepository, deviceCredentialRepository, gaRepository, deviceUserRepository, sensorRepository, sensorWidgetRepository,
         monitorSensorWidgetRepository, activityLogRepository, deviceSettings, staticCacheManager)
   {
      _workContext = workContext;
      _staticCacheManager = staticCacheManager;
      _deviceRepository = deviceRepository;
      _userRepository = userRepository;
      _userDeviceRepository = userDeviceRepository;
      _gaRepository = gaRepository;
      _activityLogRepository = activityLogRepository;
      _localizedRepository = localizedRepository;
      _communicator = communicator;
      _userSettings = userSettings;
      _deviceSettings = deviceSettings;
      _userService = userService;
      _localizer = localizer;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Adds online status query
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <param name="query">Income query</param>
   /// <returns>Result query</returns>
   private async Task<IQueryable<Device>> StatusQueryAsync(DynamicFilter filter, IQueryable<Device> query)
   {
      query =
      from d in query
      where filter.ConnectionStatuses.Contains(d.ConnectionStatus)
      select d;

      return await Task.FromResult(query);
   }

   /// <summary>
   /// Required for client query
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <param name="query">Income query</param>
   /// <returns>Result query</returns>
   private async Task<IQueryable<Device>> ProjectionQuery(DynamicFilter filter, IQueryable<Device> query)
   {
      // user query
      var userQuery = _userRepository.Table.AsNoTracking();
      
      // filter user by owner name column
      if (filter.AdditionalQueries.TryGetValue(nameof(User), out var additionalUserQuery) && !string.IsNullOrEmpty(additionalUserQuery))
      {
         additionalUserQuery = additionalUserQuery.Replace("OwnerName", _userSettings.UsernamesEnabled ? nameof(User.Username) : nameof(User.Email));
         userQuery = userQuery.Where(additionalUserQuery);
      }

      var lang = await _workContext.GetWorkingLanguageAsync();

      var localeQuery =
      from lp in _localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id
      select new { lp.EntityId, lp.LocaleKey, lp.LocaleValue };

      var onlineDevices = await _communicator.GetOnlineDeviceIdsAsync();
      var beenRecenlyLimit = DateTime.UtcNow.AddMinutes(-_deviceSettings.BeenRecentlyMinutes);
      var defVal = new DateTime(1L, DateTimeKind.Utc);
      // security: projection only required data fields
      query =
      from d in query
      join u in userQuery on d.OwnerId equals u.Id
      select new Device()
      {
         Id = d.Id,
         ClearDataDelay = d.ClearDataDelay,
         Configuration = d.Configuration,
         CountDataRows = d.CountDataRows,
         CreatedOnUtc = d.CreatedOnUtc,
         DataflowReconnectDelay = d.DataflowReconnectDelay,
         DataPacketSize = d.DataPacketSize,
         DataSendingDelay = d.DataSendingDelay,
         DisplayOrder = d.DisplayOrder,
         LastIpAddress = d.LastIpAddress,
         Enabled = d.Enabled,
         IsActive = d.IsActive,
         OwnerId = d.OwnerId,
         PictureId = d.PictureId,
         SystemName = d.SystemName,
         UpdatedOnUtc = d.UpdatedOnUtc,
         LastActivityOnUtc = d.LastActivityOnUtc,
         OwnerName = _userSettings.UsernamesEnabled ? u.Username : u.Email,
         Lon = d.Lon,
         Lat = d.Lat,
         IsMobile = d.IsMobile,
         ShowOnMain = d.ShowOnMain,

         ConnectionStatus = onlineDevices.Contains(d.Id)
           ? OnlineStatus.Online
           : d.LastActivityOnUtc == null 
              ? OnlineStatus.NoActivities
              : d.LastActivityOnUtc >= beenRecenlyLimit
                 ? OnlineStatus.BeenRecently
                 : OnlineStatus.Offline,

         Name = localeQuery
         .Where(x => x.EntityId == d.Id && x.LocaleKey == nameof(Device.Name))
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),

         Description = localeQuery
         .Where(x => x.EntityId == d.Id && x.LocaleKey == nameof(Device.Description))
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),
      };

      return query;
   }

   #endregion

   #region Methods

   #region Delete

   /// <summary>
   /// Deletes device
   /// </summary>
   /// <param name="device">Device entity</param>
   /// <returns>deleted devices</returns>
   public override async Task DeleteAsync(Device device)
   {
      ArgumentNullException.ThrowIfNull(device);

      // it's soft deleting entity. So we don't delete entity locales
      //await _localizer.DeleteLocaleAsync<Device>(devices.Select(x => x.Id));

      await base.DeleteAsync(device);
   }

   #endregion

   #region Get 

   /// <summary>
   /// Gets all devices by filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   public async Task<IFilterableList<Device>> GetAllDevicesAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      if (!filter.ConnectionStatuses.Any())
         return new FilterableList<Device>();

      var query = _deviceRepository.Table.AsNoTracking();

      if (filter.UserId.HasValue)
         query = query.Where(x => x.OwnerId == filter.UserId);

      if (filter.DeviceId.HasValue)
         query = query.Where(x => x.Id == filter.DeviceId);

      if (filter.DeviceIds is not null && filter.DeviceIds.Any())
         query = query.Where(x => filter.DeviceIds.Contains(x.Id));

      query = await ProjectionQuery(filter, query);
      query = query.ApplyClientQuery(filter);

      query = await StatusQueryAsync(filter, query);
      //query = query.OrderBy(x => x.Id);

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets own (by user) devices  by filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   public async Task<IFilterableList<Device>> GetOwnDevicesAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var query =
      from m in _deviceRepository.Table.AsNoTracking()
      where m.OwnerId == filter.UserId
      select m;

      query = await ProjectionQuery(filter, query);

      if (filter.DeviceId.HasValue)
         query = query.Where(x => x.Id == filter.DeviceId);

      if (filter.DeviceIds is not null && filter.DeviceIds.Any())
         query = query.Where(x => filter.DeviceIds.Contains(x.Id));

      query = query.ApplyClientQuery(filter);
      query = await StatusQueryAsync(filter, query);

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets shared (granted) devices  by filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   public async Task<IFilterableList<Device>> GetSharedDevicesAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var query =
      from d in _deviceRepository.Table.AsNoTracking()
      join ud in _userDeviceRepository.Table.AsNoTracking() on d.Id equals ud.DeviceId
      where ud.UserId == filter.UserId
      select d;

      if (filter.DeviceId.HasValue)
         query = query.Where(x => x.Id == filter.DeviceId);

      if (filter.DeviceIds is not null && filter.DeviceIds.Any())
         query = query.Where(x => filter.DeviceIds.Contains(x.Id));

      query = query.ApplyClientQuery(filter);

      query = await ProjectionQuery(filter, query);
      query = await StatusQueryAsync(filter, query);
      //query = query.OrderBy(x => x.Id);

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets device map item collection (for admin mode) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device map item collection</returns>
   public async Task<IFilterableList<DeviceMapItem>> GetAllDeviceMapItemsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var deviceQuery =
         from d in _deviceRepository.Table.AsNoTracking()
         where d.ShowOnMain
         select d;

      deviceQuery = deviceQuery.ApplyClientQuery(filter);

      if (filter.UserId.HasValue)
      {
         deviceQuery =
            from d in deviceQuery
            where d.OwnerId == filter.UserId
            select d;
      }

      var lang = await _workContext.GetWorkingLanguageAsync();
      var onlineDevices = await _communicator.GetOnlineDeviceIdsAsync();
      var beenRecenlyLimit = DateTime.UtcNow.AddMinutes(-_deviceSettings.BeenRecentlyMinutes);

      var query =
      from d in deviceQuery
      select new DeviceMapItem()
      {
         Id = d.Id,
         IsMobile = d.IsMobile,
         Lat = d.Lat,
         Lon = d.Lon,

         Name =
           (from lp in _localizedRepository.Table.AsNoTracking()
            where lp.EntityId == d.Id
               && lp.LanguageId == lang.Id
               && lp.LocaleKeyGroup == nameof(Device)
               && lp.LocaleKey == nameof(Device.Name)
            select lp.LocaleValue).FirstOrDefault(),

         Status = onlineDevices.Contains(d.Id)
           ? OnlineStatus.Online
           : d.LastActivityOnUtc == null
              ? OnlineStatus.NoActivities
              : d.LastActivityOnUtc >= beenRecenlyLimit
                 ? OnlineStatus.BeenRecently
                 : OnlineStatus.Offline,
      };

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// User scope for devices
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Device user scope query</returns>
   public IQueryable<Device> UserScope(long? userId)
   {
      var deviceQuery = _deviceRepository.Table;
      if (userId.HasValue)
      {
         deviceQuery =
         from d in deviceQuery
         join ud in _userDeviceRepository.Table.AsNoTracking() on d.Id equals ud.DeviceId into udgroup
         from g in udgroup.DefaultIfEmpty()
         where g != default && g.UserId == userId || d.OwnerId == userId
         select d;

         //deviceQuery = 
         //   (from d in deviceQuery
         //   where d.OwnerId == userId.Value
         //   select d)
         //   .Union
         //   (from d in deviceQuery
         //    join ud in _userDeviceRepository.Table.AsNoTracking() on d.Id equals ud.DeviceId
         //    wher ud.UserId == userId
         //    select d);

      }
      return deviceQuery;
   }

   /// <summary>
   /// Gets device map item collection (for current user) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device map item collection</returns>
   public async Task<IFilterableList<DeviceMapItem>> GetUserDeviceMapItemsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter?.UserId);

      var lang = await _workContext.GetWorkingLanguageAsync();
      var onlineDevices = await _communicator.GetOnlineDeviceIdsAsync();
      var beenRecenlyLimit = DateTime.UtcNow.AddMinutes(-_deviceSettings.BeenRecentlyMinutes);

      var deviceQuery = UserScope(filter.UserId);
      deviceQuery = deviceQuery.Where(x => x.ShowOnMain);
      deviceQuery = deviceQuery.ApplyClientQuery(filter);

      var query =
      from d in deviceQuery
      select new DeviceMapItem()
      {
         Id = d.Id,
         IsMobile = d.IsMobile,
         Lat = d.Lat,
         Lon = d.Lon,
         IsShared = d.OwnerId != filter.UserId,

         Name =
           (from lp in _localizedRepository.Table.AsNoTracking()
            where lp.EntityId == d.Id
               && lp.LanguageId == lang.Id
               && lp.LocaleKeyGroup == nameof(Device)
               && lp.LocaleKey == nameof(Device.Name)
            select lp.LocaleValue).FirstOrDefault(),

         Status = onlineDevices.Contains(d.Id)
           ? OnlineStatus.Online
           : d.LastActivityOnUtc == null
              ? OnlineStatus.NoActivities
              : d.LastActivityOnUtc >= beenRecenlyLimit
                 ? OnlineStatus.BeenRecently
                 : OnlineStatus.Offline
      };

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets device select items for client selectable UI elements like a "dropdown list" 
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item colection</returns>
   public async Task<IFilterableList<DeviceSelectItem>> GetAllDeviceSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var lang = await _workContext.GetWorkingLanguageAsync();

      var localeQuery =
      from lp in _localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id
      select new { lp.EntityId, lp.LocaleKey, lp.LocaleValue };

      var deviceQuery = _deviceRepository.Table.AsNoTracking();
      deviceQuery = deviceQuery.ApplyClientQuery(filter);

      if (filter.UserId != null)
         deviceQuery = deviceQuery.Where(x => x.OwnerId == filter.UserId);

      var query =
      from d in deviceQuery
      select new DeviceSelectItem()
      {
         Id = d.Id,
         SystemName = d.SystemName,

         Name = localeQuery
          .Where(x => x.EntityId == d.Id && x.LocaleKey == nameof(Device.Name))
          .Select(x => x.LocaleValue)
          .FirstOrDefault()
      };

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets device select items for client selectable UI elements like a "dropdown list" 
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item colection</returns>
   public async Task<IFilterableList<DeviceSelectItem>> GetUserDeviceSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var lang = await _workContext.GetWorkingLanguageAsync();

      var localeQuery =
      from lp in _localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id
      select new { lp.EntityId, lp.LocaleKey, lp.LocaleValue };

      var deviceQuery = UserScope(filter.UserId);
      deviceQuery = deviceQuery.ApplyClientQuery(filter);

      var query =
      from d in deviceQuery
      select new DeviceSelectItem()
      {
         Id = d.Id,
         SystemName = d.SystemName,
         IsShared = d.OwnerId != filter.UserId,

         Name = localeQuery
          .Where(x => x.EntityId == d.Id && x.LocaleKey == nameof(Device.Name))
          .Select(x => x.LocaleValue)
          .FirstOrDefault()
      };

      var result = await query.FilterAsync(filter);
      return result;
   }

   #endregion

   #region Update

   /// <summary>
   /// Update monitor entity
   /// </summary>
   /// <param name="device">Monitor entity</param>
   /// <returns>Async operation</returns>
   public override async Task UpdateDeviceAsync(Device device)
   {
      ArgumentNullException.ThrowIfNull(device);

      await base.UpdateDeviceAsync(device);

      // update/insert locales
      await _localizer.SaveLocaleAsync(device, await _workContext.GetWorkingLanguageAsync());
   }

   #endregion

   #region Insert

   /// <summary>
   /// Inserts device to database
   /// </summary>
   /// <param name="device">Adding device entity</param>
   /// <returns>Device identifier</returns>
   public override async Task<long> InsertDeviceAsync(Device device)
   {
      ArgumentNullException.ThrowIfNull(device);

      await base.InsertDeviceAsync(device);

      // update/insert locales
      await _localizer.SaveLocaleAsync(device, await _workContext.GetWorkingLanguageAsync());

      return device.Id;
   }

   #endregion

   #region Common

   /// <summary>
   /// Is the device into the user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>result: true - into the scope, false - not into the scope</returns>
   public async Task<bool> IsUserDeviceAsync(long userId, long deviceId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(deviceId, 1);

      var query = _deviceRepository.Table.AsNoTracking().Where(x => x.OwnerId == userId && x.Id == deviceId);
      return await query.AnyAsync();
   }

   /// <summary>
   /// Shares device to user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   public async Task ShareDeviceAsync(long userId, long deviceId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(deviceId, 1);

      await _userDeviceRepository.InsertAsync(new UserDevice() { DeviceId = deviceId, UserId = userId });
   }

   /// <summary>
   /// Stop sharing device to user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   public async Task UnshareDeviceAsync(long userId, long deviceId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(deviceId, 1);

      var map = await _userDeviceRepository.Table.FirstOrDefaultAsync(x => x.DeviceId == deviceId && x.UserId == userId);
      if (map == null)
         return;

      await _userDeviceRepository.DeleteAsync(map);
   }

   #endregion

   #endregion
}
