using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SensorEmulator.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SensorEmulator;

/// <summary>
/// Worker service
/// </summary>
public class Worker : BackgroundService
{
   #region fields

   private readonly ILogger<Worker> _logger;
   private readonly IConfiguration _configuration;
   private readonly IHttpClientFactory _httpClientFactory;

   private HttpClient httpClient;
   private long lastConfiguration;
   private Guid dispatherProcessGuid;

   private int handlingGnssRecord;
   private static List<GNSS> gnssData;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
   {
      _logger = logger;
      _configuration = configuration;
      _httpClientFactory = httpClientFactory;
      httpClient = _httpClientFactory.CreateClient("default");
      var gnssDataFile = _configuration["GNSSData"];
      gnssData = ParseNMEAFile(gnssDataFile).Where(x => x.Speed > 5).ToList();


   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      while (!stoppingToken.IsCancellationRequested)
      {
         try
         {
            // in appsetting.json
            var installedSensorGroups = _configuration.GetSection("Sensors").Get<SensorGroup[]>();

            // in the system
            var query = string.Join("&", installedSensorGroups.Select(x => $"systemNames={HttpUtility.UrlEncode(x.SystemName)}"));
            var allowedSensors = await httpClient.GetFromJsonAsync<List<Sensor>>($"GetConfiguration?{query}", cancellationToken: stoppingToken);

            // intersection
            var observableSensors = allowedSensors.IntersectBy(installedSensorGroups.Select(x => x.SystemName.ToLower()), x => x.SystemName.ToLower());

            foreach (var observableSensor in observableSensors)
               observableSensor.Group = installedSensorGroups.FirstOrDefault(x => x.SystemName == observableSensor.SystemName).Group;

            if (observableSensors.Any())
               await SensorHandling(observableSensors, stoppingToken);

            throw new Exception($"There are no subjects to watch.");
         }
         catch (Exception ex)
         {
            _logger.Log(LogLevel.Error, ex.Message);
         }

         await Task.Delay(10_000, stoppingToken);
      }
   }

   #endregion

   #region Utils

   /// <summary>
   /// Sensor handling
   /// </summary>
   /// <param name="sensors">Sensor collection</param>
   /// <param name="cancellationToken">Cancelation token</param>
   /// <returns></returns>
   private async Task SensorHandling(IEnumerable<Sensor> sensors, CancellationToken cancellationToken)
   {
      var sensorWatchTasks = sensors.GroupBy(x => x.Group)
          .Select(x => Task.WhenAll(x.AsEnumerable().Select(x => HandleSensorAsync(x, cancellationToken))))
          .ToList();

      while (!cancellationToken.IsCancellationRequested && sensorWatchTasks.Count() > 0)
      {
         using var finishedTask = await Task.WhenAny(sensorWatchTasks);
         sensorWatchTasks.Remove(finishedTask);
         if (finishedTask.Status == TaskStatus.RanToCompletion)
         {
            var sensorMessages = finishedTask.Result;
            await SensorMessageHandler(sensorMessages, cancellationToken);
            sensorWatchTasks.Add(Task.WhenAll(sensorMessages.Select(x => HandleSensorAsync(x.Sensor, cancellationToken))));
         }
         else if (finishedTask.Status == TaskStatus.Faulted)
         {
            // Task.WhenAny doesn't rethrow exceptions from the individual tasks
            // https://stackoverflow.com/questions/31544684/why-does-the-task-whenany-not-throw-an-expected-timeoutexception
            var exMessage = $"{nameof(TaskStatus.Faulted)} ";

            foreach (var exception in finishedTask.Exception?.InnerExceptions ?? new(Array.Empty<Exception>()))
               exMessage += exception.Message;

            _logger.LogError(exMessage);

            sensorWatchTasks.Remove(finishedTask);
         }
      }
   }

   /// <summary>
   /// Sensor message handler
   /// </summary>
   /// <param name="sensorMessages">Sensor message</param>
   /// <param name="cancellationToken">Cancelation token/param>
   /// <returns></returns>
   private async Task SensorMessageHandler(IEnumerable<SensorMessage> sensorMessages, CancellationToken cancellationToken)
   {
      var eventTimestamp = DateTime.UtcNow.Ticks;
      var dataSet = sensorMessages.Select(x => new SensorRecord
      {
         Bytes = x.Bytes,
         Metadata = string.Empty,
         EventTimestamp = eventTimestamp,
         SensorSystemName = x.Sensor.SystemName,
         Timestamp = x.Timestamp.Ticks,
         Value = x.Value,
      });

      var response = await httpClient.PostAsJsonAsync($"InsertMany", dataSet, cancellationToken: cancellationToken);
      EnsureSyncConfiguration(response);
      EnsureDispatcherProcessId(response);
      response.EnsureSuccessStatusCode();
   }

   /// <summary>
   /// Handles a sensor
   /// </summary>
   /// <param name="sensor">handling sensor</param>
   /// <param name="token">Cancelation token</param>
   /// <returns>Sensor message</returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="Exception"></exception>
   private async Task<SensorMessage> HandleSensorAsync(Sensor sensor, CancellationToken token)
   {
      if (sensor == null)
         throw new ArgumentNullException(nameof(sensor));
      else if (sensor.SystemName.StartsWith("GNSS"))
         return await HandleGnssSensorAsync(sensor, token);
      else if (sensor.SystemName.StartsWith("HeartBeat"))
         return await HandleHeartBeatSensorAsync(sensor, token);
      else
         return await HandleScalarSensorAsync(sensor, token);
   }

   /// <summary>
   /// Handles a scalar sensor
   /// </summary>
   /// <param name="sensor">Handling sensor</param>
   /// <param name="token">Cancelation token</param>
   /// <returns>Sensor message</returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="Exception"></exception>
   private async Task<SensorMessage> HandleScalarSensorAsync(Sensor sensor, CancellationToken token)
   {
      if (sensor == null)
         throw new ArgumentNullException(nameof(sensor));

      if (sensor.Configuration == null)
         throw new Exception($"A {nameof(sensor.Configuration)} of the sensor {sensor.SystemName} is empty.");

      var setting = JsonSerializer.Deserialize<EmulationSettings>(sensor.Configuration);

      if (setting == null)
         throw new Exception($"There are no settings for sensor {sensor.SystemName}.");

      var minDelay = Convert.ToInt32(Math.Round(setting.AverageDelay * 1000d * (1d - setting.DeviationDelay / 100d), 0));
      var maxDelay = Convert.ToInt32(Math.Round(setting.AverageDelay * 1000d * (1d + setting.DeviationDelay / 100d), 0));
      var delay = Random.Shared.Next(minDelay, maxDelay);

      await Task.Delay(delay, token);

      var minValue = Convert.ToInt32(Math.Round(setting.AverageValue * (1d - setting.DeviationValue / 100d), 0));
      var maxValue = Convert.ToInt32(Math.Round(setting.AverageValue * (1d + setting.DeviationValue / 100d), 0));
      var value = Random.Shared.Next(minValue, maxValue);

      return new SensorMessage() { Sensor = sensor, Value = value, Timestamp = DateTime.UtcNow, Bytes = [] /*Description = $"Info:{sensor.Name} is {value}"*/};
   }

   /// <summary>
   /// Handle a GNSS sensor
   /// </summary>
   /// <param name="sensor">Handling sensor</param>
   /// <param name="token">Cancelation token</param>
   /// <returns>Sensor message</returns>
   private async Task<SensorMessage> HandleGnssSensorAsync(Sensor sensor, CancellationToken token)
   {
      var delay = 5;
      var skipParking = _configuration.GetValue<bool>("SkipParking");

      if (handlingGnssRecord > 0)
      {
         var span = Convert.ToInt32(new TimeSpan(gnssData[handlingGnssRecord].Ticks - gnssData[handlingGnssRecord - 1].Ticks).TotalSeconds);
         delay = skipParking ? (span > 10 ? 10 : span) : span;
      }
      await Task.Delay(delay * 1000, token);

      var dateTime = DateTime.UtcNow;

      var ticks = BitConverter.GetBytes(dateTime.Ticks);
      var lon = BitConverter.GetBytes(gnssData[handlingGnssRecord].Lon);
      var lat = BitConverter.GetBytes(gnssData[handlingGnssRecord].Lat);
      var speed = BitConverter.GetBytes(gnssData[handlingGnssRecord].Speed);
      var height = BitConverter.GetBytes(gnssData[handlingGnssRecord].Height);
      var reliable = BitConverter.GetBytes(gnssData[handlingGnssRecord].Reliable);
      var course = BitConverter.GetBytes(gnssData[handlingGnssRecord].Course);

      var byteArray = new[] { ticks, lon, lat, speed, height, reliable, course }.SelectMany(x => x).ToArray();

      if (handlingGnssRecord >= gnssData.Count - 1)
         handlingGnssRecord = 0;
      else
         handlingGnssRecord++;

      return new SensorMessage() { Sensor = sensor, Timestamp = dateTime, Bytes = byteArray /*Metadata = $"Info:{sensor.Name} is {value}"*/};
   }

   /// <summary>
   /// NMEA parser
   /// </summary>
   /// <param name="filePath">File path</param>
   /// <returns>Parsed GNSS records</returns>
   private static IEnumerable<GNSS> ParseNMEAFile(string filePath)
   {
      using var stream = File.OpenText("Data/" + filePath);
      while (!stream.EndOfStream)
      {
         var record = stream.ReadLine();
         var array = record.Split(',');

         // data reliable
         var reliable = array[2].Equals("A");

         // ticks
         var year = int.Parse(array[9][4..6]);
         var month = int.Parse(array[9][2..4]);
         var day = int.Parse(array[9][0..2]);
         var hour = int.Parse(array[1][0..2]);
         var minute = int.Parse(array[1][2..4]);
         var second = int.Parse(array[1][4..6]);
         var dateTime = new DateTime(year, month, day, hour, minute, second);

         // latitude
         var latIntegerPart = double.Parse(array[3][0..2]);
         var latFractionalPart = double.Parse(array[3][2..], CultureInfo.InvariantCulture) / 60;
         var lat = (latIntegerPart + latFractionalPart) * (array[4].Equals("N") ? 1 : -1);

         // lon
         var lonIntegerPart = double.Parse(array[5][0..3]);
         var lonFractionalPart = double.Parse(array[5][3..], CultureInfo.InvariantCulture) / 60;
         var lon = (lonIntegerPart + lonFractionalPart) * (array[6].Equals("E") ? 1 : -1);

         // speed (from knot/h to km/h)
         var speed = double.Parse(array[7], CultureInfo.InvariantCulture) * 1.852;

         var course = double.Parse(array[8], CultureInfo.InvariantCulture);

         yield return new GNSS { Ticks = dateTime.Ticks, Reliable = reliable, Lat = lat, Lon = lon, Speed = speed, Course = course };
      }
   }

   int i = 25;
   private async Task<SensorMessage> HandleHeartBeatSensorAsync(Sensor sensor, CancellationToken token)
   {
      var minBeatRate = _configuration.GetValue<double>("MinBeatRate");
      var maxBeatRate = _configuration.GetValue<double>("MaxBeatRate");
      var beatRateUpdate = _configuration.GetValue<int>("BeatRateUpdate");
      var beatRateArg = _configuration.GetValue<double>("BeatRateArg");

      var now = DateTime.UtcNow;
      var grow = Math.Sin(i++ * Math.PI / beatRateArg);
      var beatRate = minBeatRate + (maxBeatRate - minBeatRate) / 2 * (1 + grow);
      await Task.Delay(beatRateUpdate, token);

      return new SensorMessage() { Sensor = sensor, Value = beatRate, Timestamp = now };
   }

   #endregion

   #region Control communication with Dispatcher

   /// <summary>
   /// Ensures the configuration version
   /// </summary>
   /// <param name="response">Dispatcher response</param>
   /// <exception cref="Exception"></exception>
   private void EnsureSyncConfiguration(HttpResponseMessage response)
   {
      // control configuration
      if (!response.Headers.TryGetValues("X-Configuration-Version", out var values)
          || !long.TryParse(values.FirstOrDefault(), out var deviceModifiedTicks)
          || deviceModifiedTicks == default)
      {
         response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
         throw new Exception($"Dispatcher configuration version is not set.");
      }

      if (lastConfiguration == 0)
         lastConfiguration = deviceModifiedTicks;

      else if (lastConfiguration != deviceModifiedTicks)
      {
         lastConfiguration = deviceModifiedTicks;
         throw new Exception($"Dispatcher configuration has been changed.");
      }
   }

   /// <summary>
   /// Ensures dispatcher process identifier
   /// </summary>
   /// <param name="response">Dispatcher response</param>
   /// <exception cref="Exception"></exception>
   private void EnsureDispatcherProcessId(HttpResponseMessage response)
   {
      // control if device dispather was rebooted then need to reboot it's watchers
      if (!response.Headers.TryGetValues("X-ProcessGuid", out var values)
          || !Guid.TryParse(values.FirstOrDefault(), out var processGuid)
          || processGuid == default)
      {
         response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
         throw new Exception($"Dispatcher process guid is not set.");
      }

      if (dispatherProcessGuid == default)
         dispatherProcessGuid = processGuid;

      else if (dispatherProcessGuid != processGuid)
      {
         dispatherProcessGuid = processGuid;
         throw new Exception($"Dispatcher has been restarted.");
      }
   }

   #endregion
}