using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Devices;
using Clients.Dash.Services.Security;
using Microsoft.Extensions.Caching.Memory;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Reresents the device entity service
/// </summary>
public class DeviceGrpcService : IDeviceService
{
   #region fields

   private readonly DeviceRpc.DeviceRpcClient _grpcClient;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IMemoryCache _memoryCache;
   private readonly ClearCacheService _clearCacheService;
   private readonly PermissionService _permissionService;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DeviceGrpcService(DeviceRpc.DeviceRpcClient grpcClient,
      IStaticCacheManager staticCacheManager,
      PermissionService permissionService,
      IMemoryCache memoryCache,
      ClearCacheService clearCacheService)
   {
      _grpcClient = grpcClient;
      _staticCacheManager = staticCacheManager;
      _memoryCache = memoryCache;
      _clearCacheService = clearCacheService;
      _permissionService = permissionService;
   }

   #endregion

   #region Methods

   #region Get

   /// <summary>
   /// Get own devices by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   public async Task<IFilterableList<Device>> GetAllDevicesAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByDynamicFilterCacheKey, "all", filter);

      async Task<FilterableList<Device>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetAllDevicesAsync(filterProto);

         var devices = Auto.Mapper.Map<FilterableList<Device>>(query.Devices);
         devices.TotalCount = query.TotalCount ?? 0;

         foreach (var device in devices)
         {
            var deviceCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, device.Id);
            await _staticCacheManager.SetAsync(deviceCacheKey, device);
         }

         return devices;
      }

      var result = await _staticCacheManager.GetAsync(cacheKey, acquire);
      return result;
   }


   /// <summary>
   /// Get own devices by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   public async Task<IFilterableList<Device>> GetOwnDevicesAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByDynamicFilterCacheKey, "own", filter);

      async Task<FilterableList<Device>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetOwnDevicesAsync(filterProto);

         var devices = Auto.Mapper.Map<FilterableList<Device>>(query.Devices);
         devices.TotalCount = query.TotalCount ?? 0;   

         foreach (var device in devices)
         {
            var deviceCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, device.Id);
            await _staticCacheManager.SetAsync(deviceCacheKey, device);
         }

         return devices;
      }

      var result = await _staticCacheManager.GetAsync(cacheKey, acquire);
      return result;
   }

   /// <summary>
   /// Get shared devices by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device entity collection</returns>
   public async Task<IFilterableList<Device>> GetSharedDevicesAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByDynamicFilterCacheKey, "shared", filter);

      async Task<FilterableList<Device>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetSharedDevicesAsync(filterProto);

         var devices = Auto.Mapper.Map<FilterableList<Device>>(query.Devices);
         devices.TotalCount = query.TotalCount ?? 0;
         
         foreach (var device in devices)
         {
            var monitorCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, device.Id);
            await _staticCacheManager.SetAsync(monitorCacheKey, device);
         }

         return devices;
      }

      var result = await _staticCacheManager.GetAsync(cacheKey, acquire);
      return result;
   }

   /// <summary>
   /// Gets device map item collection (for admin mode) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device map item collection</returns>
   public async Task<IFilterableList<DeviceMapItem>> GetAllDeviceMapItemsAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<DeviceMapItem>.ByDynamicFilterCacheKey, "all", filter);

      async Task<FilterableList<DeviceMapItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetAllMapDeviceAsync(filterProto);

         var devices = Auto.Mapper.Map<FilterableList<DeviceMapItem>>(query.Items);
         devices.TotalCount = query.TotalCount ?? 0;

         return devices;
      }

      var result = await _staticCacheManager.GetAsync(cacheKey, acquire);
      return result;
   }

   /// <summary>
   /// Gets device map item collection (for current user) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device map item collection</returns>
   public async Task<IFilterableList<DeviceMapItem>> GetUserDeviceMapItemsAsync(DynamicFilter filter)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<DeviceMapItem>.ByDynamicFilterCacheKey, "own&shared", filter);

      async Task<FilterableList<DeviceMapItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetUserMapDeviceAsync(filterProto);

         var devices = Auto.Mapper.Map<FilterableList<DeviceMapItem>>(query.Items);
         devices.TotalCount = query.TotalCount ?? 0;

         return devices;
      }

      var result = await _staticCacheManager.GetAsync(cacheKey, acquire);
      return result;
   }


   /// <summary>
   /// Gets all device select item list (for the admin mode)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   public async Task<IFilterableList<DeviceSelectItem>> GetAllDeviceSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<DeviceSelectItem>.ByDynamicFilterCacheKey, "all", filter);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<DeviceSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetAllDeviceSelectListAsync(filterProto);
         var devices = Auto.Mapper.Map<FilterableList<DeviceSelectItem>>(query.Devices);
         devices.TotalCount = query.TotalCount ?? 0;

         return devices;
      }
   }

   /// <summary>
   /// Gets device select item list for the current user
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   public async Task<IFilterableList<DeviceSelectItem>> GetUserDeviceSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<DeviceSelectItem>.ByDynamicFilterCacheKey, "own", filter);
      return await _staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<DeviceSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await _grpcClient.GetUserDeviceSelectListAsync(filterProto);
         var devices = Auto.Mapper.Map<FilterableList<DeviceSelectItem>>(query.Devices);
         devices.TotalCount = query.TotalCount ?? 0;

         return devices;
      }
   }

   /// <summary>
   /// Gets a device entity by the identifier
   /// </summary>
   /// <param name="id">Identifier</param>
   /// <returns>Device instance</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task<Device> GetByIdAsync(long id)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
     
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, id);

      Func<Task<Device>> acquire = async () =>
      {
         var filter = new DynamicFilter() { Query = $"query => query.Where(x => x.Id == {id}).Take(1)" };
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);

         var query = await _permissionService.IsAdminModeAsync()
         ? await _grpcClient.GetAllDevicesAsync(filterProto)
         : await _grpcClient.GetOwnDevicesAsync(filterProto);

         var device = Auto.Mapper.Map<List<Device>>(query.Devices).FirstOrDefault();

         return device;
      };

      return await _staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets device entities by the identifiers
   /// </summary>
   /// <param name="ids">Identifiers</param>
   /// <returns>Device collection</returns>
   /// <exception cref="ArgumentNullException"></exception>
   public async Task<IFilterableList<Device>> GetByIdsAsync(IEnumerable<long> ids)
   {
      ArgumentNullException.ThrowIfNull(ids);

      if (!ids.Any())
         return new FilterableList<Device>();

      var filter = new DynamicFilter() { Query = $"query => query.Where(x => @0.Contains(x.Id))", Ids = ids.ToArray() };

      var devices = await GetOwnDevicesAsync(filter);

      foreach (var device in devices)
      {
         var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, device.Id);
         await _staticCacheManager.SetAsync(cacheKey, device);
      }

      return devices;
   }

   /// <summary>
   /// Ensures that the system name is not occupied 
   /// </summary>
   /// <param name="systemName">System name</param>
   /// <returns>Validation result</returns>
   public async Task<CommonResponse> CheckSystemNameAvailabilityAsync(string systemName)
   {
      ArgumentNullException.ThrowIfNullOrWhiteSpace(systemName);
      var result = await _grpcClient.CheckSystemNameAvailabilityAsync(new() { SystemName = systemName });
      return result;
   }

   /// <summary>
   /// Checks password format
   /// </summary>
   /// <param name="password">System name</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Validation result</returns>
   public async Task<CommonResponse> CheckPasswordFormatAsync(string password, long deviceId)
   {
      ArgumentNullException.ThrowIfNullOrWhiteSpace(password);
      var result = await _grpcClient.CheckPasswordFormatAsync(new() { Password = password, DeviceId = deviceId });
      return result;
   }

   #endregion

   #region Update

   /// <summary>
   /// Updates a device entity from the device model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task UpdateAsync(DeviceModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var proto = Auto.Mapper.Map<DeviceProto>(model);
      var reply = await _grpcClient.UpdateAsync(proto);

      // update models
      var device = Auto.Mapper.Map<Device>(reply);
      Auto.Mapper.Map(device, model);

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, model.Id);
      if (_memoryCache.TryGetValue(cacheKey.Key, out object value) && value is not null && value is Device savedDevice)
         Auto.Mapper.Map(model, savedDevice);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);
   }


   /// <summary>
   /// Updates a shared device entity 
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task UpdateSharedAsync(DeviceModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var proto = Auto.Mapper.Map<DeviceProto>(model);
      await _grpcClient.UpdateSharedAsync(proto);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Device>.ByDynamicFilterPrefix, "shared");

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);
   }

   /// <summary>
   /// Changes a device password
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="password">new password</param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task ChangePassword(long deviceId, string password) 
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(password);
      ArgumentOutOfRangeException.ThrowIfLessThan(deviceId, 1);

      await _grpcClient.ChangePasswordAsync(new() { DeviceId = deviceId, Password = password });
   }

   #endregion

   #region Insert

   /// <summary>
   /// Adds a device entity by the sensor model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task InsertAsync(DeviceModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfNotEqual(model.Id, 0);
      
      // insert in data base
      var proto = Auto.Mapper.Map<DeviceProto>(model);
      var reply = await _grpcClient.InsertAsync(proto);

      // update models
      var device = Auto.Mapper.Map<Device>(reply);
      Auto.Mapper.Map(device, model);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Device>.ByDynamicFilterPrefix, "own");
      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Device>.ByDynamicFilterPrefix, "all");
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Device>.ByIdCacheKey, device.Id);
      await _staticCacheManager.SetAsync(cacheKey, device);
   }

   #endregion

   #region Delete

   /// <summary>
   /// Deletes a device entity by the sensor model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeleteAsync(DeviceModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      _ = await _grpcClient.DeleteAsync(new() { Id = model.Id });

      await _clearCacheService.DeviceClearCache();
   }

   /// <summary>
   /// Deletes a device entity by the sensor model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeleteSharedAsync(DeviceModel model)
   {
      ArgumentNullException.ThrowIfNull(model);

      if (model.Id < 1)
         throw new ArgumentOutOfRangeException(nameof(model.Id));

      var deviceProto = Auto.Mapper.Map<DeviceProto>(model);
      _ = await _grpcClient.DeleteSharedAsync(deviceProto);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Device>.ByDynamicFilterPrefix, "shared");
   }



   #endregion

   #region Sharing

   /// <summary>
   /// Shares a device to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   public async Task ShareDeviceAsync(string userName, long deviceId)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(userName);
      ArgumentOutOfRangeException.ThrowIfLessThan(deviceId, 1);

      await _grpcClient.ShareDeviceAsync(new() { EntityId = deviceId, UserName = userName });
   }

   /// <summary>
   /// Unshares a device to a specified user
   /// </summary>
   /// <param name="userName">Username</param>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   public async Task UnshareDeviceAsync(string userName, long deviceId)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(userName);
      ArgumentOutOfRangeException.ThrowIfLessThan(deviceId, 1);

      await _grpcClient.UnshareDeviceAsync(new() { EntityId = deviceId, UserName = userName });
   }

   #endregion

   #endregion
}
