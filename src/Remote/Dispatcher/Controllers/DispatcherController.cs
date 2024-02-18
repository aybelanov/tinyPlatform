using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Domain;
using Devices.Dispatcher.Infrastructure;
using Devices.Dispatcher.Services.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;

namespace Devices.Dispatcher.Controllers;

/// <summary>
/// Common dispatcher controller
/// </summary>
[ApiController]
[OnlyLocalIP]
[Route("api/[controller]/[action]")]
[SetConfigurationVersion]
public partial class DispatcherController : ControllerBase
{
   #region Fields

   private readonly IWebHostEnvironment _environment;
   private readonly AppDbContext _dbContext;
   private readonly ISettingService _settingService;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DispatcherController(AppDbContext dbContext, IWebHostEnvironment environment, ISettingService settingService)
   {
      _dbContext = dbContext;
      _environment = environment;
      _settingService = settingService;
   }

   #endregion

   #region Utils

   /// <summary>
   /// Filter incomming sensor data collection due the current device configuration  
   /// </summary>
   /// <param name="sensorData"></param>
   /// <returns></returns>
   private IEnumerable<SensoRecord> FilterSensorData(IEnumerable<SensoRecord> sensorData)
   {
      var sensors = _dbContext.Sensors;

      foreach (var data in sensorData)
      {
         var sensor = sensors.FirstOrDefault(s => s.SystemName.ToLower() == data.SensorSystemName.ToLower());

         if (sensor == null)
            continue;

         data.SensorId = sensor.Id;
         yield return data;
      }
   }

   #endregion

   #region Methods

   /// <summary>
   /// Inserts incomming sensor data to the local database
   /// </summary>
   /// <param name="sensorRecords">Sensor data records collection</param>
   /// <returns></returns>
   [HttpPost]
   public async Task InsertMany(IEnumerable<SensoRecord> sensorRecords)
   {
      ArgumentNullException.ThrowIfNull(sensorRecords);

      if (sensorRecords.Any())
      {
         await _dbContext.AddRangeAsync(FilterSensorData(sensorRecords));
         await _dbContext.SaveChangesAsync();
      }
   }

   /// <summary>
   /// Uploads the video file to the local file storage
   /// </summary>
   /// <param name="segmentFileName"></param>
   /// <returns></returns>
   [HttpPut("{segmentFileName}")]
   public async Task UploadVideoSegment(string segmentFileName)
   {
      var sensorId = int.Parse(segmentFileName.Split('-')[0]);
      var sensor = _dbContext.Sensors.FirstOrDefault(x => x.Id == sensorId);
      if (sensor == null)
         return;
     
      var filePath = Path.Combine(Defaults.VideoSegmentsPath, segmentFileName + ".tmp");

      if (Defaults.SegmentTypes.Contains(Path.GetExtension(segmentFileName)))
      {
         var segmentName = Path.GetFileNameWithoutExtension(segmentFileName);
         var segmentNumber = int.Parse(segmentName.Split("out").Last());
         await _settingService.SetSettingAsync(sensor.SystemName + ".lastsegmentnumber", segmentNumber);
      }

      await using (Request.Body)
      {
         await using (var fs = System.IO.File.Create(filePath))
         {
            await Request.Body.CopyToAsync(fs);
            await Request.Body.FlushAsync();
            await fs.FlushAsync();
            fs.Close();
            Request.Body.Close();
         }
      }

      System.IO.File.Copy(filePath, filePath.Replace(".tmp", string.Empty), true);
      System.IO.File.Delete(filePath);
   }

   /// <summary>
   /// Gets setting from the local data base.
   /// It is used by the sensor handlers.
   /// </summary>
   /// <param name="key">Setting key</param>
   /// <returns>Setting value as a string</returns>
   [HttpGet("{key}")]
   public async Task<string> GetSettingByKey(string key)
   {
      key = HttpUtility.UrlDecode(key);
      var result = await _settingService.GetSettingByKeyAsync<string>(key);
      return result;
   }

   /// <summary>
   /// Save setting of the sensor handler service in the local database
   /// </summary>
   /// <param name="data">Saving data</param>
   /// <returns></returns>
   [HttpPost]
   public async Task SaveSettingByKey(JsonObject data)
   {
      await _settingService.SetSettingAsync(data["key"].ToString(), data["value"].ToString());
   }

   /// <summary>
   /// Gets a sensor configurtaion.
   /// It is used by the sensor handlers.
   /// </summary>
   /// <param name="systemNames">Sensor systemname collection</param>
   /// <returns>Sensor collection</returns>
   [HttpGet]
   public IEnumerable<Sensor> GetConfiguration([FromQuery(Name = "systemNames")] string[] systemNames)
   {
      systemNames = systemNames.Select(x => HttpUtility.UrlDecode(x)).ToArray();
      var sensors = _dbContext.Sensors.Where(x => systemNames.Select(x => x.ToLower()).Contains(x.SystemName.ToLower()));
      return sensors;
   }

   #endregion
}
