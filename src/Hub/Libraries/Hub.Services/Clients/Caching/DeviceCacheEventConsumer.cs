using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Caching;

/// <summary>
/// Represents the sensor cache event consumer
/// </summary>
public class DeviceCacheEventConsumer : CacheEventConsumer<Device>
{

   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(Device entity, EntityEventType entityEventType)
   {
      await base.ClearCacheAsync(entity, entityEventType);
   }
}
