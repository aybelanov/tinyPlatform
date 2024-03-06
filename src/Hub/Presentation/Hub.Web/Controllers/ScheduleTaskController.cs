using Hub.Services.ScheduleTasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
[AutoValidateAntiforgeryToken]
public partial class ScheduleTaskController : Controller
{
   private readonly IScheduleTaskService _scheduleTaskService;
   private readonly IScheduleTaskRunner _taskRunner;

   public ScheduleTaskController(IScheduleTaskService scheduleTaskService,
       IScheduleTaskRunner taskRunner)
   {
      _scheduleTaskService = scheduleTaskService;
      _taskRunner = taskRunner;
   }

   [HttpPost]
   [IgnoreAntiforgeryToken]
   public virtual async Task<IActionResult> RunTask(string taskType)
   {
      var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(taskType);
      if (scheduleTask == null)
         //schedule task cannot be loaded
         return NoContent();

      await _taskRunner.ExecuteAsync(scheduleTask);

      return NoContent();
   }
}