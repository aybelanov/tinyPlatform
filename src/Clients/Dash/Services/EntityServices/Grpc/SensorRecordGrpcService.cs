using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Infrastructure;
using Clients.Widgets;
using Clients.Widgets.Core;
using Microsoft.Extensions.Logging;
using Shared.Clients;
using Shared.Clients.Proto;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Clients.Widgets.VideoPlayer;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents the sensor record service
/// </summary>
public class SensorRecordGrpcService : ISensorRecordService
{
   #region fields

   private readonly ILogger<SensorRecordGrpcService> _logger;
   private readonly SensorRecordRpc.SensorRecordRpcClient _grpcClient;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public SensorRecordGrpcService(SensorRecordRpc.SensorRecordRpcClient grpcClient, ILogger<SensorRecordGrpcService> logger, IStaticCacheManager staticCacheManager)
   {
      _grpcClient = grpcClient;
      _logger = logger;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Deletes sensor data by the filter
   /// </summary>
   /// <param name="filter">Sensor filter</param>
   /// <returns></returns>
   public async Task DeleteRecordsAsync(DynamicFilter filter)
   {
      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      _ = await _grpcClient.DeleteAsync(filterProto);

      await _staticCacheManager.RemoveByPrefixAsync(CacheDefaults<SensorRecord>.Prefix);
   }

   /// <summary>
   /// Gets a chart sensor data set
   /// </summary>
   /// <param name="request">Chart request</param>
   /// <returns>Chart point data set</returns>
   public async Task<IList<ChartSet>> GetChartSeriesAsync(ChartRequest request)
   {
      var requestProto = Auto.Mapper.Map<ChartRequestProto>(request);
      var seriesProto = await _grpcClient.GetChartDataAsync(requestProto);
      var series = Auto.Mapper.Map<List<ChartSet>>(seriesProto.Series);

      return series;
   }

   /// <summary>
   /// Gets sensor data by the filter
   /// </summary>
   /// <param name="filter">Sensor filter</param>
   /// <returns>Sensor record collection</returns>
   public async Task<IFilterableList<SensorRecord>> GetRecordsAsync(DynamicFilter filter)
   {
      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var setProto = await _grpcClient.GetRecordsAsync(filterProto);
      var set = Auto.Mapper.Map<FilterableList<SensorRecord>>(setProto.Records);

      return set;
   }

   /// <summary>
   /// Gets data staistics for all users (admins only) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Chart points</returns>
   public async Task<IList<TimelineChart.Point>> GetAllDataStatistics(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var dataProtos = await _grpcClient.GetAllDataStatisticsAsync(filterProto);
      var data = Auto.Mapper.Map<List<TimelineChart.Point>>(dataProtos.Data);

      return data;
   }

   /// <summary>
   /// Gets data staistics for current user by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Chart points</returns>
   public async Task<IList<TimelineChart.Point>> GetUserDataStatistics(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var dataProtos = await _grpcClient.GetUserDataStatisticsAsync(filterProto);
      var data = Auto.Mapper.Map<List<TimelineChart.Point>>(dataProtos.Data);
      
      return data;
   }

   /// <summary>
   /// Gets GNSS track
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Geo points</returns>
   public async Task<IList<OpenLayerBase.GeoPoint>> GetTrackAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var baseKey = new CacheKey($"GeoPoints.ByFilter.{{0}}", "TrackerMapPrefix");
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(baseKey, filter);
      
      return await _staticCacheManager.GetAsync(cacheKey, aquire);

      async Task<IList<OpenLayerBase.GeoPoint>> aquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var dataProtos = await _grpcClient.GetTrackAsync(filterProto);

         var bytes = dataProtos.Data.ToByteArray();

         var points = GetGeoData(bytes).OrderBy(x => x.Ticks).ToList();
         return points;
      }

      static IEnumerable<OpenLayerBase.GeoPoint> GetGeoData(byte[] dataArray)
      {
         var unixTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
         var baseTicks = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks - unixTicks;
         for (int i = 0; i < dataArray.Length; i += 14)
         {
            var point = new OpenLayerBase.GeoPoint()
            {
               Ticks = (BitConverter.ToInt32(dataArray[i..(i + 4)]) * TimeSpan.TicksPerSecond + baseTicks) / TimeSpan.TicksPerMillisecond,
               Lon = BitConverter.ToInt32(dataArray[(i + 4)..(i + 8)]) / 1_000_000D,
               Lat = BitConverter.ToInt32(dataArray[(i + 8)..(i + 12)]) / 1_000_000D,
               Speed = BitConverter.ToInt16(dataArray[(i + 12)..(i + 14)])
            };

            yield return point;
         }
      }
   }

   /// <summary>
   /// Gets the last rcord of the sensor
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <returns>Geo point</returns>
   public async Task<OpenLayerBase.GeoPoint> GetLastRecordAsync(long sensorId)
   {
      var records = await GetRecordsAsync(new() { SensorId = sensorId, OrderBy= "EventTimestamp desc", Top = 1 });
      var geopoint = ClientHelper.ParseGeoRecord(records.FirstOrDefault());
      return geopoint;
   }


   /// <summary>
   /// Gets video segments by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Video segment collection</returns>
   public async Task<IList<Segment>> GetVideoSegmentsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      
      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var protos = await _grpcClient.GetVideoSegmentsAsync(filterProto);
      var segments = protos.Segments.Select(x => new Segment()
      {
         Id = x.Id,
         Extinf = x.Extinf,
         IpcamId = x.IpcamId,
         SegmentName = new Guid(x.SegmentName.ToByteArray()).ToString("N"),
         OnCreatedUtc = x.OnCreatedUtc.ToDateTimeFromUinxEpoch()

      }).ToList();

      return segments;  
   }

   #endregion
}
