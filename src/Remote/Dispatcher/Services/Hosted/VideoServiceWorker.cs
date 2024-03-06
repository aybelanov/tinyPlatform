using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Services.Settings;
using Google.Protobuf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Devices.Proto;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services.Hosted;

/// <summary>
/// Video service worker
/// </summary>
public class VideoServiceWorker : BackgroundService
{
   #region fields

   private readonly ILogger<VideoServiceWorker> _logger;
   private readonly IServiceScopeFactory _scopeFactory;
   private readonly IWebHostEnvironment _environment;
   private readonly HubConnections _hubConnections;

   #endregion

   #region Properties

   private DirectoryInfo UploadDirectory => new(Defaults.VideoSegmentsPath);
   private string SentDirectoryPath => Path.Combine(UploadDirectory.FullName, "sent");

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public VideoServiceWorker(
      ILogger<VideoServiceWorker> logger,
      IServiceScopeFactory scopeFactory,
      IWebHostEnvironment environment,
      HubConnections hubConnection)
   {
      _logger = logger;
      _scopeFactory = scopeFactory;
      _environment = environment;
      _hubConnections = hubConnection;
   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      using var scope = _scopeFactory.CreateScope();
      var deviceSettings = await scope.ServiceProvider.GetRequiredService<ISettingService>().LoadSettingAsync<DeviceSettings>();
      var dataSendingDelay = deviceSettings.DataSendingDelay < 1000 ? 1000 : deviceSettings.DataSendingDelay;

      while (_hubConnections.Enabled && !stoppingToken.IsCancellationRequested)
      {
         using var scope2 = _scopeFactory.CreateScope();
         var client = scope2.ServiceProvider.GetRequiredService<DeviceCalls.DeviceCallsClient>();

         FileInfo sendingFile;
         if ((sendingFile = UploadDirectory.GetFiles()?.Where(CheckFile).OrderByDescending(x => x.LastWriteTimeUtc).FirstOrDefault()) is not null)
         {
            using var call = client.VideoCall(cancellationToken: stoppingToken);
            try
            {
               do
               {
                  var segment = new VideoSegmentProto()
                  {
                     SegmentName = sendingFile.Name,
                     Timestamp = sendingFile.CreationTimeUtc.Ticks,
                     Bytes = UnsafeByteOperations.UnsafeWrap(await File.ReadAllBytesAsync(sendingFile.FullName, stoppingToken))
                  };

                  if (Defaults.SegmentTypes.Contains(sendingFile.Extension))
                  {
                     using var process = new Process();
                     try
                     {
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.EnableRaisingEvents = false;
                        process.StartInfo.FileName = "ffprobe";
                        var args = string.Format("-i {0} -v error -select_streams v -show_entries stream -of json=compact=1", sendingFile.FullName);
                        process.StartInfo.Arguments = args;
                        process.Start();
                        await process.WaitForExitAsync();
                        var output = process.StandardOutput.ReadToEnd();
                        var json = JsonNode.Parse(output);
                        var data = json["streams"].AsArray().First();
                        var height = data["height"].ToString();
                        var width = data["width"].ToString();
                        var duration = data["duration"].ToString();
                        segment.Duration = double.Parse(duration, NumberStyles.Any, CultureInfo.InvariantCulture);
                        segment.Resolution = width + "x" + height;
                     }
                     catch (Exception ex)
                     {
                        _logger.LogInformation(ex, "Video segments getting duration error.");
                     }
                  }

                  await call.RequestStream.WriteAsync(segment, stoppingToken);

                  try { sendingFile.Delete(); } catch { }
               }
               while ((sendingFile = UploadDirectory.GetFiles().Where(CheckFile).OrderByDescending(x => x.LastWriteTimeUtc).FirstOrDefault()) is not null);

               await call.RequestStream.CompleteAsync();
               _ = await call;
            }
            catch (Exception ex)
            {
               _logger.LogInformation(ex, "Video segments sending error");
            }
         }

         await Task.Delay(dataSendingDelay, stoppingToken);
      }
   }

   #endregion

   #region Utilities

   private static bool CheckFile(FileInfo file)
   {
      var result = file is not null && file.Exists && file.Length > 0 && Defaults.SegmentTypes.Contains(file.Extension);
      return result;
   }

   #endregion
}
