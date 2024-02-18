using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Services.ExportImport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients;
using Shared.Clients.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Hub.Services.Hosted;

/// <summary>
/// Represents a export report data file creating process unit
/// </summary>
public sealed class ExportDataFileService
{
   #region fields

   private readonly IServiceScopeFactory _scopeFactory;
   private readonly ICommunicator _communuicator;

   private DownloadTask _downloadTask;
   private DownloadRequest _downloadRequest;
   private Device _device;
   private IList<Sensor> _sensors;
   private CancellationToken _stoppingToken;
   private string _temporaryFilePath;
   private string _dataFormatFilePath;
   private string _readyFilePath;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ExportDataFileService(ICommunicator communuicator, IServiceScopeFactory scopeFactory)
   {
      _scopeFactory = scopeFactory;
      _communuicator = communuicator;
      CancellationTokenSource cancelTokenSource = new();
      _stoppingToken = cancelTokenSource.Token;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Tries to init process of export data to a file  
   /// </summary>
   /// <returns></returns>
   private async Task<bool> TryProcessInitAsync(DownloadTask task)
   {
      _downloadTask = task;
      if (_downloadTask == null)
         return false;

      try { _downloadRequest = JsonSerializer.Deserialize<DownloadRequest>(_downloadTask.QueryString); }
      catch { return false; }

      _device = await GetDeviceAsync(_downloadRequest.DeviceId);
      if (_device == null)
         return false;

      _sensors = await GetSensorsAsync(_downloadRequest.SensorIds.ToList());
      if (!(_sensors?.Any() ?? false))
         return false;

      var uploadDir = CommonHelper.DefaultFileProvider.GetAbsolutePath(ClientDefaults.ReportFilleDirectory); //"files\\exportimport"
      var fileNameWithoutExtension = _downloadRequest.Compression switch
      {
         FileCompressionType.None => _downloadTask.FileName,
         FileCompressionType.ZIP => _downloadTask.FileName.Replace(".zip", string.Empty),
         FileCompressionType.GZIP => _downloadTask.FileName.Replace(".gzip", string.Empty),
         _ => _downloadTask.FileName
      };

      _dataFormatFilePath = Path.Combine(uploadDir, "temp", fileNameWithoutExtension);
      _temporaryFilePath = _dataFormatFilePath + ".tmp";
      _readyFilePath = Path.Combine(uploadDir, _downloadTask.FileName);

      return true;
   }

   /// <summary>
   /// Exports sensor data to a file
   /// </summary>
   /// <returns></returns>
   public async Task ExportDataToFileAsync(DownloadTask task)
   {
      if (!await TryProcessInitAsync(task))
      {
         await CancelDownloadTaskAsync(); 
         return;
      }

      await (_downloadRequest.Format switch
      {
         ExportFileType.CSV => UploadDataToCsvAsync(),
         ExportFileType.JSON => UploadDataToJsonAsync(),
         ExportFileType.XML => UploadDataToXmlAsync(),
         ExportFileType.TXT => UploadDataToTxtAsync(),
         _ => throw new NotImplementedException()
      });

      await (_downloadRequest.Compression switch
      {
         FileCompressionType.None => CompressNoneAsync(),
         FileCompressionType.ZIP => CompressToZipAsync(),
         FileCompressionType.GZIP => CompressToGzipAsync(),
         _ => throw new NotImplementedException()
      });

      await SetDownloadTaskReadyAsync();
   }

   /// <summary>
   /// None-compession file handler
   /// </summary>
   /// <returns></returns>
   private async Task CompressNoneAsync()
   {
      File.Move(_temporaryFilePath, _readyFilePath, true);
      await Task.CompletedTask;
   }

   private async Task CompressToZipAsync()
   {
      File.Move(_temporaryFilePath, _dataFormatFilePath, true);
      await ExportFile.CompressionFileToZipAsync(_dataFormatFilePath);
      File.Move(_dataFormatFilePath + ".zip", _readyFilePath, true);
      File.Delete(_dataFormatFilePath);
   }

   private async Task CompressToGzipAsync()
   {
      File.Move(_temporaryFilePath, _dataFormatFilePath, true);
      await ExportFile.CompressionFileToGzipAsync(_dataFormatFilePath);
      File.Move(_dataFormatFilePath + ".gzip", _readyFilePath, true);
      File.Delete(_dataFormatFilePath);
   }

   /// <summary>
   /// Upload data to a CSV-file
   /// </summary>
   /// <returns></returns>
   private async Task UploadDataToJsonAsync()
   {
      using (var fw = File.Create(_temporaryFilePath))
      {
         using (var jwr = new Utf8JsonWriter(fw, new() { Indented = false }))
         {
            jwr.WriteStartObject();
            jwr.WritePropertyName(nameof(Device));
            jwr.WriteStringValue(_device.SystemName);
            jwr.WriteStartArray("Data");
            using (var scope = _scopeFactory.CreateScope())
            {
               using var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
               dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(600));
               var reportQuery = PrepareDataReportQuery(dbContext.Set<SensorRecord>().AsNoTracking(), _downloadRequest);

               foreach (var @event in reportQuery)
               {
                  jwr.WriteStartObject();
                  jwr.WritePropertyName("EventTimestamp");
                  jwr.WriteStringValue(new DateTime(@event.EventTimestamp));
                  jwr.WriteStartArray("Sensors");
                  foreach (var record in @event.Records)
                  {
                     jwr.WriteStartObject();
                     jwr.WritePropertyName("SystemName");
                     jwr.WriteStringValue(_sensors.FirstOrDefault(x => x.Id == record.SensorId)?.SystemName ?? "none");
                     jwr.WritePropertyName("Value");
                     jwr.WriteNumberValue(record.Value);
                     jwr.WriteEndObject();
                  }
                  jwr.WriteEndArray();
                  jwr.WriteEndObject();
               }
            }
            jwr.WriteEndArray();
            jwr.WriteEndObject();
            await jwr.FlushAsync(_stoppingToken);
         }
      }
   }

   /// <summary>
   /// Upload data to a XML-file
   /// </summary>
   /// <returns></returns>
   private async Task UploadDataToXmlAsync()
   {
      using (var fw = File.Create(_temporaryFilePath))
      {
         using (var xw = XmlWriter.Create(fw, new() { Async = true, ConformanceLevel = ConformanceLevel.Auto }))
         {
            await xw.WriteStartDocumentAsync();
            await xw.WriteStartElementAsync("DataReport");
            await xw.WriteAttributeStringAsync(nameof(Device), _device.SystemName);
            await xw.WriteAttributeStringAsync("ReportDateUtc", DateTime.UtcNow.ToString("G"));
            using (var scope = _scopeFactory.CreateScope())
            {
               using var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
               dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(600));
               var reportQuery = PrepareDataReportQuery(dbContext.Set<SensorRecord>().AsNoTracking(), _downloadRequest);

               foreach (var @event in reportQuery)
               {
                  await xw.WriteStartElementAsync("Event");
                  await xw.WriteAttributeStringAsync("Date", new DateTime(@event.EventTimestamp));
                  foreach (var record in @event.Records)
                  {
                     await xw.WriteStartElementAsync("Sensor");
                     await xw.WriteAttributeStringAsync("SystemName", _sensors.FirstOrDefault(x => x.Id == record.SensorId)?.SystemName ?? "none");
                     xw.WriteValue(record.Value);
                     await xw.WriteEndElementAsync();
                  }
                  await xw.WriteEndElementAsync();
               }
            }
            await xw.WriteEndElementAsync();
            await xw.WriteEndDocumentAsync();
            await xw.FlushAsync();
         }
      }
   }

   /// <summary>
   /// Upload data to a TXT-file
   /// </summary>
   /// <returns></returns>
   private async Task UploadDataToTxtAsync()
   {
      // overwrite
      if (File.Exists(_temporaryFilePath))
         File.Delete(_temporaryFilePath);

      using (var fileStream = File.AppendText(_temporaryFilePath))
      {
         fileStream.WriteLine($"DateTimeUtc={DateTime.UtcNow};");
         fileStream.WriteLine($"Device={_device.SystemName};");
         fileStream.WriteLine($"Sensors=[{string.Join(',', _sensors.Select(x => x.SystemName))}];");
         using (var scope = _scopeFactory.CreateScope())
         {
            using var dbContext = scope.ServiceProvider.GetService<AppDbContext>(); 
            dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(600));
            var reportQuery = PrepareDataReportQuery(dbContext.Set<SensorRecord>().AsNoTracking(), _downloadRequest);

            foreach (var @event in reportQuery)
            {
               var str = $"Event={new DateTime(@event.EventTimestamp):O};";
               foreach (var record in @event.Records)
               {
                  var sensorSystemName = _sensors.FirstOrDefault(x => x.Id == record.SensorId)?.SystemName ?? "none";
                  str += $"{sensorSystemName}={record.Value};";
               }

               await fileStream.WriteLineAsync(str);
            }
         }
         await fileStream.FlushAsync();
      }
   }

   /// <summary>
   /// Upload data to a CSV-file
   /// </summary>
   /// <returns></returns>
   private async Task UploadDataToCsvAsync()
   {
      // overwrite
      if (File.Exists(_temporaryFilePath))
         File.Delete(_temporaryFilePath);

      // header
      var titles = new List<string>() { $"{_device.SystemName}_{nameof(SensorRecord.EventTimestamp)}" };
      titles.AddRange(_sensors.Select(x => x.SystemName));
      var header = string.Join('\t', titles);
      using (var fileStream = File.AppendText(_temporaryFilePath))
      {
         fileStream.WriteLine(header);
         await fileStream.FlushAsync();
      }

      // data
      using (var fileStream = File.AppendText(_temporaryFilePath))
      {
         using (var scope = _scopeFactory.CreateScope())
         {
            using var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(600));
            var reportQuery = PrepareDataReportQuery(dbContext.Set<SensorRecord>().AsNoTracking(), _downloadRequest);

            foreach (var @event in reportQuery)
            {
               var values = new List<string>() { new DateTime(@event.EventTimestamp).ToString("O") };
               foreach (var sensor in _sensors)
               {
                  values.Add(@event.Records.FirstOrDefault(x => x.SensorId == sensor.Id)?.Value.ToString() ?? string.Empty);
               }

               await fileStream.WriteLineAsync(string.Join('\t', values));
            }
         }
         await fileStream.FlushAsync();
      }
   }


   /// <summary>
   /// Gets a device by the identifier 
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Device entity</returns>
   private async Task<Device> GetDeviceAsync(long deviceId)
   {
      using var scope = _scopeFactory.CreateScope();
      using var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
      var device = await dbContext.Set<Device>().FirstOrDefaultAsync(x => x.Id == deviceId);

      return device;
   }

   /// <summary>
   /// Gets report sensors
   /// </summary>
   /// <param name="sensorIds">Sensor identifier</param>
   /// <returns>Sensor colection</returns>
   private async Task<IList<Sensor>> GetSensorsAsync(IList<long> sensorIds)
   {
      using var scope = _scopeFactory.CreateScope();
      using var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
      var sensors = await dbContext.Set<Sensor>().AsNoTracking()
         .Where(x => sensorIds.Contains(x.Id))
         .OrderBy(x => x.Id)
         .ToListAsync(_stoppingToken);

      return sensors;
   }

   /// <summary>
   /// Prepares the data record query
   /// </summary>
   /// <param name="query">Inbound query</param>
   /// <param name="request">DownloadRequest</param>
   /// <returns>Data report query</returns>
   private static IQueryable<EventGroupRecord> PrepareDataReportQuery(IQueryable<SensorRecord> query, DownloadRequest request)
   {
      var sensorIds = request.SensorIds.ToList();
      var from = request.From.Ticks;
      var to = request.To.Ticks;

      var eventGroupQuery =
         from r in query
         where r.EventTimestamp >= @from & r.EventTimestamp <= to && sensorIds.Contains(r.SensorId)
         orderby r.EventTimestamp descending, r.SensorId
         group r by r.EventTimestamp into gr
         select new EventGroupRecord
         {
            EventTimestamp = gr.Key,
            Records =
               from r in gr
               select new ReportRecord()
               {
                  Bytes = r.Bytes,
                  SensorId = r.SensorId,
                  Value = r.Value,
               }
         };

      return eventGroupQuery;
   }

   /// <summary>
   /// Cancels a download task by a identifuer
   /// </summary>
   /// <returns></returns>
   private async Task CancelDownloadTaskAsync()
   {
      try
      {
         if (_downloadTask != null)
         {
            using var scope = _scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _downloadTask.CurrentState = DownloadFileState.Canceled;
            dbContext.Update(_downloadTask);
            await dbContext.SaveChangesAsync();
            await _communuicator.ClientsNotifyAsync($"{nameof(DownloadTask)}_{_downloadTask.Id}", SignalRDefaults.DownloadTaskStatusChanged, _downloadTask);
         }
      }
      catch { }
   }

   /// <summary>
   /// Sets download task's status to "Ready" 
   /// </summary>
   /// <returns></returns>
   private async Task SetDownloadTaskReadyAsync()
   {
      using var scope = _scopeFactory.CreateScope();
      using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      _downloadTask.CurrentState = DownloadFileState.Ready;
      _downloadTask.Size = new FileInfo(_readyFilePath).Length;
      _downloadTask.ReadyDateTimeUtc = DateTime.UtcNow;
      dbContext.Update(_downloadTask);
      await dbContext.SaveChangesAsync();
      await _communuicator.ClientsNotifyAsync($"{nameof(DownloadTask)}_{_downloadTask.Id}", SignalRDefaults.DownloadTaskStatusChanged, _downloadTask);
   }

   #endregion
}
