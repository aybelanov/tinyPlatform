using Hub.Core.Domain.Clients;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Services.Logging;
using Hub.Services.ScheduleTasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace Hub.Services.Common;

/// <summary>
/// Represents a clear sensor data task
/// </summary>
public class ClearSensorDatasTask(ILogger logger, AppDbContext dataProvider, IAppFileProvider fileProvider, DeviceSettings deviceSettings) : IScheduleTask
{
   #region Methods

   /// <summary>
   /// Executes a task
   /// </summary>
   public virtual async System.Threading.Tasks.Task ExecuteAsync()
   {
      if (deviceSettings.MaxSensorDatasInDb > 0)
      {
         try
         {
            var count = await dataProvider.GetTable<SensorRecord>().AsNoTracking()
               .OrderByDescending(x => x.EventTimestamp)
               .Skip(deviceSettings.MaxSensorDatasInDb)
               .ExecuteDeleteAsync();

            await logger.InformationAsync($"{count} surplus sensor data records were deleted successfully.");
         }
         catch (Exception ex)
         {
            await logger.WarningAsync($"Error of clearing sensor data records.", ex);
         }
      }

      if (deviceSettings.VideoFileExpiration > 0)
      {
         try
         {
            var expirationDate = DateTime.UtcNow.AddMinutes(-deviceSettings.VideoFileExpiration);

            // clear database
            var count = await dataProvider.GetTable<VideoSegment>().AsNoTracking()
               .Where(x => x.OnCreatedUtc < expirationDate)
               .ExecuteDeleteAsync();

            await logger.InformationAsync($"{count} videosegments were deleted successfully.");

            // clear cached files
            var destDir = fileProvider.GetAbsolutePath(Clients.ClientDefaults.VideoStorageDirectory);

            var expiredFiles = System.IO.Directory.GetFiles(destDir, "*.*", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x)).Where(x => x.LastWriteTimeUtc < expirationDate && !x.Name.Equals("Index.htm", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            expiredFiles.ForEach(x =>
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
            await logger.WarningAsync("Video segment clearing is failed.", ex);
         }
      }
   }

   #endregion
}