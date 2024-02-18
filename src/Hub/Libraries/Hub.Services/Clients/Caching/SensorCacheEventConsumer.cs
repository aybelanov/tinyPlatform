using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Caching;

/// <summary>
/// Represents the sensor cache event consumer
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
/// <param name="workContext"></param>
public class SensorCacheEventConsumer(IWorkContext workContext) : CacheEventConsumer<Sensor>()
{
   private readonly IWorkContext _workContext = workContext;

   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(Sensor entity, EntityEventType entityEventType)
   {
      await RemoveAsync(ClientCacheDefaults<Sensor>.BySystemNameCacheKey, entity.SystemName);
      await RemoveByPrefixAsync(ClientCacheDefaults<Sensor>.DevicePrefix, entity.DeviceId);

      await base.ClearCacheAsync(entity, entityEventType);
   }
}
