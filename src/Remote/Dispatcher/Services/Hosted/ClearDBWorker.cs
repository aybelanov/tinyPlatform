using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Services.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services.Hosted;

/// <summary>
/// Clear database worker
/// </summary>
public class ClearDBWorker : BackgroundService
{
   #region fields

   private readonly ILogger<ClearDBWorker> _logger;
   private readonly IServiceScopeFactory _scopeFactory;
   private readonly IWebHostEnvironment _environment;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ClearDBWorker(ILogger<ClearDBWorker> logger, IServiceScopeFactory scopeFactory, IWebHostEnvironment environment)
   {
      _logger = logger;
      _scopeFactory = scopeFactory;
      _environment = environment;
   }

   #endregion

   #region Methods

   ///<inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      using var scope = _scopeFactory.CreateScope();
      var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();
      var settings = await settingService.LoadSettingAsync<DeviceSettings>();

      while (!stoppingToken.IsCancellationRequested)
      {
         var delayTask = Task.Delay((settings.ClearDataDelay < 600 ? 600 : settings.ClearDataDelay) * 1000, stoppingToken);
         ClearSensorRecord(settings.CountDataRows);
         ClearVideoSegments(settings.VideoSegmentsExpiration);
         await delayTask;
      }
   }


   private void ClearSensorRecord(int recordCount)
   {
      using var scope = _scopeFactory.CreateScope();
      using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
      dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

      try
      {
         using var connection = dbContext.Database.GetDbConnection();
         connection.Open();

         using var transaction = connection.BeginTransaction();
         using var command = connection.CreateCommand();

         command.CommandText =
             // deleting all sent records
             $"DELETE FROM SensorRecords WHERE IsSent = '1'; " +
             // delete all records older than one month
             $"DELETE FROM SensorRecords WHERE EventTimestamp < {DateTime.UtcNow.AddMonths(-1).Ticks}; " +
             // delete all old records more than the table count limit records are
             $"DELETE FROM SensorRecords WHERE Id IN (SELECT Id FROM SensorRecords ORDER BY EventTimestamp DESC LIMIT 1000000000  OFFSET {recordCount});";

         command.ExecuteNonQuery();
         transaction.Commit();

         // clear db service file and disk space
         using var command2 = connection.CreateCommand();
         command2.CommandText = "VACUUM;";
         command2.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Database has not cleared.");
      }
   }

   private void ClearVideoSegments(int videoSegmentsExpiration)
   {
      try
      {
         var filesInfo = Directory.GetFiles(Defaults.VideoSegmentsPath, "*.*", SearchOption.AllDirectories)
             .Select(x => new FileInfo(x)).ToList();

         var expirationDate = DateTime.UtcNow.AddHours(-videoSegmentsExpiration);
         var expiredFilesInfo = filesInfo.Where(x => x.Exists && x.LastWriteTimeUtc < expirationDate).ToList();

         expiredFilesInfo.ForEach(x =>
         {
            try
            {
               if (x.Exists)
                  File.Delete(x.FullName);
            }
            catch { }
         });
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Video segments have not cleared.");
      }
   }

   #endregion
}