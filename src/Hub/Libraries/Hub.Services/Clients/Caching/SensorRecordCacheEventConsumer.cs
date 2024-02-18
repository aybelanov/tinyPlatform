using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Caching;

/// <summary>
/// Represents the monitor cache event consumer
/// </summary>
public class DashSensorDataCacheEventConsumer : CacheEventConsumer<SensorRecord>
{
   private readonly IWorkContext _workContext;

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DashSensorDataCacheEventConsumer(IWorkContext workContext) : base()
   {
      _workContext = workContext;
   }

   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <param name="entityEventType">Entity event type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(SensorRecord entity, EntityEventType entityEventType)
   {
      var user = await _workContext?.GetCurrentUserAsync();
      await RemoveAsync(ClientCacheDefaults<SensorRecord>.ByUserAndLangCacheKey, user?.Id ?? 0);

      await base.ClearCacheAsync(entity, entityEventType);
   }
}
