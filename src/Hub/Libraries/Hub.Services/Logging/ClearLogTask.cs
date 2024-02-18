using System;
using Hub.Core.Domain.Common;
using Hub.Services.ScheduleTasks;

namespace Hub.Services.Logging
{
   /// <summary>
   /// Represents a task to clear [Log] table
   /// </summary>
   public partial class ClearLogTask : IScheduleTask
   {
      #region Fields

      private readonly CommonSettings _commonSettings;
      private readonly ILogger _logger;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      public ClearLogTask(CommonSettings commonSettings,
          ILogger logger)
      {
         _commonSettings = commonSettings;
         _logger = logger;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Executes a task
      /// </summary>
      public virtual async System.Threading.Tasks.Task ExecuteAsync()
      {
         var utcNow = DateTime.UtcNow;

         await _logger.ClearLogAsync(_commonSettings.ClearLogOlderThanDays == 0 ? null : utcNow.AddDays(-_commonSettings.ClearLogOlderThanDays));
      }

      #endregion
   }
}