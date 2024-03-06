﻿using Hub.Data.Extensions;
using Hub.Services.Helpers;
using Hub.Services.ScheduleTasks;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Tasks;
using Hub.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the schedule task model factory implementation
/// </summary>
public partial class ScheduleTaskModelFactory : IScheduleTaskModelFactory
{
   #region Fields

   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IScheduleTaskService _scheduleTaskService;

   #endregion

   #region Ctor

   public ScheduleTaskModelFactory(IDateTimeHelper dateTimeHelper,
       IScheduleTaskService scheduleTaskService)
   {
      _dateTimeHelper = dateTimeHelper;
      _scheduleTaskService = scheduleTaskService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare schedule task search model
   /// </summary>
   /// <param name="searchModel">Schedule task search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the schedule task search model
   /// </returns>
   public virtual Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged schedule task list model
   /// </summary>
   /// <param name="searchModel">Schedule task search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the schedule task list model
   /// </returns>
   public virtual async Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get schedule tasks
      var scheduleTasks = (await _scheduleTaskService.GetAllTasksAsync(true)).ToPagedList(searchModel);

      //prepare list model
      var model = await new ScheduleTaskListModel().PrepareToGridAsync(searchModel, scheduleTasks, () =>
      {
         return scheduleTasks.SelectAwait(async scheduleTask =>
            {
               //fill in model values from the entity
               var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

               //convert dates to the user time
               if (scheduleTask.LastStartUtc.HasValue)
                  scheduleTaskModel.LastStartUtc = (await _dateTimeHelper
                         .ConvertToUserTimeAsync(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc)).ToString("G");

               if (scheduleTask.LastEndUtc.HasValue)
                  scheduleTaskModel.LastEndUtc = (await _dateTimeHelper
                         .ConvertToUserTimeAsync(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc)).ToString("G");

               if (scheduleTask.LastSuccessUtc.HasValue)
                  scheduleTaskModel.LastSuccessUtc = (await _dateTimeHelper
                         .ConvertToUserTimeAsync(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc)).ToString("G");

               return scheduleTaskModel;
            });
      });
      return model;
   }

   #endregion
}