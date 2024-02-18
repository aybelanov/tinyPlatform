using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using Hub.Services.Clients;
using System.Threading.Tasks;

namespace Hub.Services.Devices.Caching;

/// <summary>
/// Represents the monitor cache event consumer
/// </summary>
public class DeviceCacheEventConsumer : CacheEventConsumer<Device>
{
   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity evet type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(Device entity, EntityEventType entityEventType)
   {
      await RemoveAsync(ClientCacheDefaults<Device>.BySystemNameCacheKey, entity.SystemName);
      await RemoveAsync(ClientCacheDefaults<Device>.ByIdCacheKey, entity.Id);
      await RemoveAsync(ClientCacheDefaults<Device>.OwnDevicesByUserCacheKey, entity.OwnerId);
      await RemoveByPrefixAsync(ClientCacheDefaults<Device>.DevicePrefix, entity.OwnerId);

      if (entityEventType == EntityEventType.Delete)
         await RemoveAsync(ClientCacheDefaults<Sensor>.ByDeviceIdCacheKey, entity.Id);
   }
}
