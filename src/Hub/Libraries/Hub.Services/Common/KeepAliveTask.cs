using Hub.Services.ScheduleTasks;

namespace Hub.Services.Common;

/// <summary>
/// Represents a task for keeping the site alive
/// </summary>
/// <remarks>
/// Ctor
/// </remarks>
/// <param name="httpClient"></param>
public partial class KeepAliveTask(PlatformHttpClient httpClient) : IScheduleTask
{
   #region Methods

   /// <summary>
   /// Executes a task
   /// </summary>
   public async System.Threading.Tasks.Task ExecuteAsync()
   {
      await httpClient.KeepAliveAsync();
   }

   #endregion
}