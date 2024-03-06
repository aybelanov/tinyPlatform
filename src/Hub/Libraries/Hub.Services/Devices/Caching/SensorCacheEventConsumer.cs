using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using Hub.Services.Clients;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Hub.Services.Devices.Caching
{
   /// <summary>
   /// Represents the monitor cache event consumer
   /// </summary>
   public class SensorCacheEventConsumer : CacheEventConsumer<Sensor>
   {
      private readonly IWorkContext _workContext;
      private readonly IMemoryCache _memoryCache;

      /// <summary>
      /// IoC Ctor
      /// </summary>
      /// <param name="workContext"></param>
      /// <param name="memoryCache"></param>
      public SensorCacheEventConsumer(IWorkContext workContext, IMemoryCache memoryCache) : base()
      {
         _workContext = workContext;
         _memoryCache = memoryCache;
      }

      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <param name="entityEventType">Entity event type</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(Sensor entity, EntityEventType entityEventType)
      {
         await RemoveAsync(ClientCacheDefaults<Sensor>.ByDeviceIdCacheKey, entity.DeviceId);
         await RemoveAsync(ClientCacheDefaults<Sensor>.BySensorSystemNameCacheKey, entity.SystemName, entity.DeviceId);
         await RemoveAsync(ClientCacheDefaults<Sensor>.ByIdCacheKey, entity.Id);

         if (entityEventType != EntityEventType.Insert)
            await RemoveAsync(AppEntityCacheDefaults<Sensor>.ByIdCacheKey, entity);
      }
   }
}
