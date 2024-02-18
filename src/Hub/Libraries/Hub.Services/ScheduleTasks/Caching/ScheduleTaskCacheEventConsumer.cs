using Hub.Core.Domain.ScheduleTasks;
using Hub.Services.Caching;

namespace Hub.Services.ScheduleTasks.Caching
{
   /// <summary>
   /// Represents a schedule task cache event consumer
   /// </summary>
   public partial class ScheduleTaskCacheEventConsumer : CacheEventConsumer<ScheduleTask>
   {
   }
}
