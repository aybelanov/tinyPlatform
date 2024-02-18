using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Services.Clients.Reports;
using Hub.Services.Logging;
using Hub.Services.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Clients;
using Shared.Clients.SignalR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hub.Services.Hosted;

/// <summary>
/// Represents a hosted service that exports sensor data reords to a file
/// </summary>
public class ExportDataHostedService : BackgroundService
{
   #region fields

   private readonly IServiceScopeFactory _scopeFactory;
   private readonly ICommunicator _communuicator;

   private static DateTime _lastClear = new();
   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ExportDataHostedService(IServiceScopeFactory scopeFactory, ICommunicator communuicator)
   {
      _scopeFactory = scopeFactory;
      _communuicator = communuicator;
   }
   #endregion

   #region Methods

   /// <inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      while (!stoppingToken.IsCancellationRequested)
      {
         var expiredTime = DateTime.UtcNow.AddMinutes(-ClientDefaults.ReportFileExpired);
         if (_lastClear <= expiredTime)
         {
            await ClearExpiredFilesAsync(expiredTime);
            _lastClear = DateTime.UtcNow;
         }

         // some trick to use cache, see download task service, 
         // GetNextDownloadTaskAsync-method
         var task = await GetNextDownloadTaskAsync();
         if (task != null && task.Id > 0)
         {
            await ExportDataAsync(task);
         }
         await Task.Delay(10_000, stoppingToken);
      }
   }

   /// <summary>
   /// Start export data
   /// </summary>
   /// <param name="task"></param>
   /// <returns></returns>
   private async Task ExportDataAsync(DownloadTask task)
   {
      try
      {
         using var scope = _scopeFactory.CreateScope();
         var exportDataProcess = scope.ServiceProvider.GetRequiredService<ExportDataFileService>();
         await exportDataProcess.ExportDataToFileAsync(task);
      }
      catch (Exception ex)
      {
         await CancelDownloadTaskAsync(task);
         await LogErrorAsync(task, ex);
      }
   }
  
   /// <summary>
   /// Clears expired report files
   /// </summary>
   /// <returns></returns>
   private async Task ClearExpiredFilesAsync(DateTime expiredTime)
   {
      using (var scope = _scopeFactory.CreateScope())
      {
         var repository = scope.ServiceProvider.GetRequiredService<IRepository<DownloadTask>>();
         var expiredTasks = repository.Table
            .Where(x => x.TaskDateTimeUtc <= expiredTime && x.CurrentState != DownloadFileState.Expired)
            .ToList();

         foreach (var task in expiredTasks)
         {
            task.CurrentState = DownloadFileState.Expired;
            await _communuicator.ClientsNotifyAsync($"{nameof(DownloadTask)}_{task.Id}", SignalRDefaults.DownloadTaskStatusChanged, task);
         }

         await repository.UpdateAsync(expiredTasks);
      }

      var uploadDir = new DirectoryInfo(CommonHelper.DefaultFileProvider.GetAbsolutePath(ClientDefaults.ReportFilleDirectory));
      var files = uploadDir.GetFiles("*.*", SearchOption.AllDirectories)
         .Where(x => x.CreationTimeUtc <= expiredTime && x.Extension != ".htm")
         .ToList();

      foreach(var file in files)
      {
         try
         {
            file.Delete();
         }
         catch { }
      }
   }

   /// <summary>
   /// Cancels a download task by a identifuer
   /// </summary>
   /// <param name="task">Download task</param>
   /// <returns></returns>
   private async Task CancelDownloadTaskAsync(DownloadTask task)
   {
      try
      {
         using var scope = _scopeFactory.CreateScope();
         var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
         task.CurrentState = DownloadFileState.Canceled;
         dbContext.Update(task);
         await dbContext.SaveChangesAsync();
         await _communuicator.ClientsNotifyAsync($"{nameof(DownloadTask)}_{task.Id}", SignalRDefaults.DownloadTaskStatusChanged, task);
      }
      catch { }
   }

   
   /// <summary>
   /// Gets next download task
   /// </summary>
   /// <returns>Download task</returns>
   private async Task<DownloadTask> GetNextDownloadTaskAsync()
   {
      using var scope = _scopeFactory.CreateScope();
      var downloadTaskService = scope.ServiceProvider.GetRequiredService<IDownloadTaskService>();
      var task = await downloadTaskService.GetNextDownloadTaskAsync();

      // some tricks, see download task service
      if (task != null && task.Id > 0)
      {
         task.CurrentState = DownloadFileState.Processing;
         await downloadTaskService.UpdateDownloadTaskAsync(task);
         await _communuicator.ClientsNotifyAsync($"{nameof(DownloadTask)}_{task.Id}", SignalRDefaults.DownloadTaskStatusChanged, task);
      }

      return task; 
   }

   /// <summary>
   /// Logs download errors
   /// </summary>
   /// <param name="downloadTask">Download task</param>
   /// <param name="exception">Current exception</param>
   /// <returns></returns>
   private async Task LogErrorAsync(DownloadTask downloadTask, Exception exception)
   {
      try
      {
         using var scope = _scopeFactory.CreateScope();
         var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
         var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
         var user = await userService.GetUserByIdAsync(downloadTask.UserId);
         await logger.ErrorAsync("Task was cancelled by the server.", exception, user);
      }
      catch { }
   }

   #endregion
}
