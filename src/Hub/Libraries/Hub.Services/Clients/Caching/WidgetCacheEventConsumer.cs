using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Caching;

/// <summary>
/// Represents the widget cache event consumer
/// </summary>
public class WidgetCacheEventConsumer : CacheEventConsumer<Widget>
{
   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(Widget entity, EntityEventType entityEventType)
   {
      await base.ClearCacheAsync(entity, entityEventType);
   }
}
