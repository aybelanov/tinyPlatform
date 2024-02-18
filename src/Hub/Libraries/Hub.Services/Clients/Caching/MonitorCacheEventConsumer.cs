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
/// <remarks>
/// IoC Ctor
/// </remarks>
/// <param name="workContext"></param>
public class MonitorCacheEventConsumer(IWorkContext workContext) : CacheEventConsumer<Monitor>()
{
   private readonly IWorkContext _workContext = workContext;

   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(Monitor entity, EntityEventType entityEventType)
   {
      var user = await _workContext.GetCurrentUserAsync();
      await RemoveByPrefixAsync(ClientCacheDefaults<Monitor>.UserPrefix, user.Id);

      await base.ClearCacheAsync(entity, entityEventType);
   }
}
