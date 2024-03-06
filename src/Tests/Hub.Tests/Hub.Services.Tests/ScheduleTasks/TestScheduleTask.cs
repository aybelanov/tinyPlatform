using Hub.Services.ScheduleTasks;
using System.Threading.Tasks;

namespace Hub.Services.Tests.ScheduleTasks
{
   public class TestScheduleTask : IScheduleTask
   {
      public TestScheduleTask()
      {
         IsInit = true;
      }

      public Task ExecuteAsync()
      {
         throw new System.NotImplementedException();
      }

      public static bool IsInit { get; protected set; }

      public static void ResetInitFlag()
      {
         IsInit = false;
      }
   }
}
