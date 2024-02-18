using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CamStreamingExample;

/// <summary>
/// Worker of the IP camera handlers
/// </summary>
public class Worker : BackgroundService
{

   #region fields

   private readonly ILogger<Worker> _logger;
   private readonly IConfiguration _configuration;
   private readonly HttpClient _httpClient;
   private readonly IHostApplicationLifetime _applicationLifetime;

   private static readonly Dictionary<Sensor, Process> currentStreams = new Dictionary<Sensor, Process>();

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IHostApplicationLifetime applicationLifetime)
   {
      _logger = logger;
      _configuration = configuration;
      _httpClient = httpClientFactory.CreateClient("default");
      _applicationLifetime = applicationLifetime;
   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      ForceKillFFmpegProcess();

      while (!stoppingToken.IsCancellationRequested)
      {
         try
         {
            var installedIPCamSystemNames = _configuration.GetSection("Cams").Get<string[]>().Select(x => x.ToLower());

            var queryString = string.Join("&", installedIPCamSystemNames.Select(x => $"systemNames={HttpUtility.UrlEncode(x)}"));
            var allowedSensors = await _httpClient.GetFromJsonAsync<List<Sensor>>($"GetConfiguration?{queryString}", cancellationToken: stoppingToken);

            var observableIPCams = allowedSensors.IntersectBy(installedIPCamSystemNames, x => x.SystemName.ToLower());

            await HandleStreams(observableIPCams);
         }
         catch (Exception ex) when (ex.Message.StartsWith("No connection could be made because the target machine actively refused it"))
         {
            KillFFmpegProcess();
            _logger.Log(LogLevel.Warning, ex.Message);
         }
         catch (Exception ex)
         {
            _logger.Log(LogLevel.Warning, ex.Message);
         }

         await Task.Delay(10_000, stoppingToken);
      }
   }

   #endregion

   #region Utils

   /// <summary>
   /// Hndles a IP camera streams
   /// </summary>
   /// <param name="ipCams">IP camera collection</param>
   /// <returns></returns>
   private async Task HandleStreams(IEnumerable<Sensor> ipCams)
   {
      // check and remove terminated processes
      foreach (var process in currentStreams)
         if (process.Value.HasExited)
            currentStreams.Remove(process.Key);

      var streamsToKill = currentStreams.ExceptBy(ipCams.Select(x => x.Id), x => x.Key.Id).ToDictionary(k => k.Key, v => v.Value);
      var camToStart = ipCams.ExceptBy(currentStreams.Select(x => x.Key.Id), x => x.Id);

      foreach (var stream in streamsToKill)
      {
         try
         {
            stream.Value?.Kill();
            currentStreams.Remove(stream.Key);
            //stream.Value?.Dispose();
         }
         catch (Exception ex)
         {
            _logger.LogWarning($"{stream.Key} stream has stop exeptions.", ex);
         }
      }

      foreach (var ipcam in camToStart)
      {
         try
         {
            var stream = await PrepareStremProcess(ipcam);

            stream.Start();
            currentStreams.Add(ipcam, stream);
         }
         catch (Exception ex)
         {
            _logger.LogWarning($"{ipcam.SystemName} stream has start exeptions.", ex);
         }
      }
   }

   /// <summary>
   /// Prepares a video stream process
   /// </summary>
   /// <param name="ipCam">Handling IP camera</param>
   /// <returns>FFmpeg process</returns>
   private async Task<Process> PrepareStremProcess(Sensor ipCam)
   {
      var setting = JsonSerializer.Deserialize<VideoStreamConvertationSettings>(ipCam.Configuration);
      var destinationUrl = $"{_configuration["DispatcherUrl"]}UploadVideoSegment/{ipCam.Id}-out.m3u8";
      var segmentStartNumber = await GetStartSegmentNumber(ipCam);

      var process = new Process();
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.FileName = "ffmpeg";//ffMpegPath;

      var ffmpegArgs = string.Empty;
      var configFfmpegArgs = _configuration.GetSection("FFmpegArgs").Get<string[]>().Where(x => !string.IsNullOrEmpty(x)).ToArray();

      if (setting.FFmpegArgs != null)
      {
         SetParams(setting.FFmpegArgs);
         ffmpegArgs = string.Format(string.Join(' ', setting.FFmpegArgs), setting.SourceUrl, setting.VideoSize, segmentStartNumber, destinationUrl);
      }
      else if (configFfmpegArgs.Any())
      {
         SetParams(configFfmpegArgs);
         ffmpegArgs = string.Format(string.Join(' ', configFfmpegArgs), setting.SourceUrl, setting.VideoSize, segmentStartNumber, destinationUrl);
      }
      else
      {
         ffmpegArgs =
             $" -loglevel error" +
             $" -rtsp_transport tcp" +
             $" -i {setting.SourceUrl}" +
             $" -flags low_delay" +
             $" -fflags nobuffer" +
             $" -tune zerolatency" +
             $" -preset ultrafast" +
             $" -c:v h264" +
             $" -r 15" +
             //$" -crf 21" +
             $" -pix_fmt yuv420p" +
             $" -b:v 100k" +
             $" -b:a 64k" +
             $" -s {setting.VideoSize}" +
             $" -f hls" +
             //$" -hls_flags append_list" +
             //$" -hls_flags omit_endlist" +
             //$" -hls_flags independent_segments" + 
             $" -hls_time 4" +
             //$" -hls_init_time 4" +
             //$" -hls_playlist_type event" +
             $" -hls_list_size 100" +
             //$" -strftime_mkdir 1 -hls_segment_filename \"video/data%03d.ts\"" + 
             //$" -hls_start_number_source datetime" +
             //$" -hls_delete_threshold 10" +
             $" -hls_flags delete_segments" +
             $" -start_number {segmentStartNumber}" +
             //$" -y" + // owerwrites the files witout ask permission (for saving outputs files into filesytem not http/tcp/udp stream)
             $" -method PUT -http_persistent 1 {destinationUrl}";
      }

      process.StartInfo.Arguments = ffmpegArgs;
      process.Exited += (o, e) => { };
      return process;


      void SetParams(string[] args)
      {
         for (var i = 0; i < args.Length; i++)
         {
            if (args[i].Trim(' ').StartsWith("-i "))
               args[i] = args[i].Replace("$data", setting.SourceUrl);

            if (args[i].Trim(' ').StartsWith("-start_number "))
               args[i] = args[i].Replace("$data", segmentStartNumber.ToString());


            if (args[i].Trim(' ').StartsWith("-s "))
               args[i] = args[i].Replace("$data", setting.VideoSize);

            if (args[i].Trim(' ').StartsWith("-method PUT "))
               args[i] = args[i].Replace("$data", destinationUrl);
         }
      }
   }

   /// <inheritdoc/>
   public override void Dispose()
   {
      KillFFmpegProcess();
      base.Dispose();
   }


   // https://stackoverflow.com/questions/13952635/what-are-the-differences-between-kill-process-and-close-process
   // https://stackoverflow.com/questions/38034318/kill-process-in-ffmpeg-in-c-sharp
   // https://stackoverflow.com/questions/48361166/kill-all-ffmpeg-processes-which-open-repeatedly-c-sharp
   // https://serverfault.com/questions/375984/how-can-i-stop-ffmpeg
   /// <summary>
   /// Kills a FFmpeg process
   /// </summary>
   private void KillFFmpegProcess()
   {
      currentStreams.ToList().ForEach(x => x.Value?.Kill());
      currentStreams.Clear();
      ForceKillFFmpegProcess();
   }

   /// <summary>
   /// Forces the kill of the FFmpeg process
   /// </summary>
   private void ForceKillFFmpegProcess()
   {
      using var killFFMpeg = new Process();
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
         // Do: pkill ffmpeg
         killFFMpeg.StartInfo.Arguments = "pkill ffmpeg";
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
         // Do: taskkill /IM audiodg.exe /F
         killFFMpeg.StartInfo.FileName = "taskkill";
         killFFMpeg.StartInfo.Arguments = "/IM ffmpeg.exe /F";
      }

      try
      {
         killFFMpeg.Start();
      }
      catch (Exception ex)
      {
         _logger.LogWarning(message: "Cannot stop ffmpeg processes.", exception: ex);
      }
   }

   /// <summary>
   /// gets start segment number
   /// </summary>
   /// <param name="sensor">Starting sensor</param>
   /// <returns>Segment start number</returns>
   private async Task<int> GetStartSegmentNumber(Sensor sensor)
   {
      int startSegmentNumber = 0;
      using var response = await _httpClient.GetAsync($"GetSettingByKey/{HttpUtility.UrlEncode(sensor.SystemName + ".lastsegmentnumber")}");
      response.EnsureSuccessStatusCode();

      using var stream = await response.Content.ReadAsStreamAsync();
      using var reader = new StreamReader(stream);
      var content = await reader.ReadToEndAsync();

      if (!string.IsNullOrWhiteSpace(content))
      {
         var lastSegmentNumber = JsonSerializer.Deserialize<int>(content);
         startSegmentNumber = lastSegmentNumber + 1;
      }

      return startSegmentNumber;
   }

   #endregion
}