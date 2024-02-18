using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Caching;

/// <summary>
/// Represents the monitor cache event consumer
/// </summary>
public class DownloadTaskEventConsumer : CacheEventConsumer<DownloadTask>
{
   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(DownloadTask entity, EntityEventType entityEventType)
   {
      await RemoveByPrefixAsync(ClientCacheDefaults<DownloadTask>.Prefix);
      await base.ClearCacheAsync(entity, entityEventType);
   }
}
