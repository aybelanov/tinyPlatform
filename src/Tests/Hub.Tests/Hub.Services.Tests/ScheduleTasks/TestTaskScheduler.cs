using Hub.Core;
using Hub.Core.Configuration;
using Hub.Services.ScheduleTasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;

namespace Hub.Services.Tests.ScheduleTasks;

public class TestTaskScheduler : TaskScheduler
{
   public TestTaskScheduler(AppSettings appSettings, IHttpClientFactory httpClientFactory, IScheduleTaskService scheduleTaskService,
      IServiceScopeFactory serviceScopeFactory, IWebHelper webHelper)
    : base(appSettings, httpClientFactory, scheduleTaskService, serviceScopeFactory, webHelper)
   {
   }

   public bool IsInit => _taskThreads.Any();

   public bool IsRun => _taskThreads.All(p => p.IsStarted && !p.IsDisposed);
}
