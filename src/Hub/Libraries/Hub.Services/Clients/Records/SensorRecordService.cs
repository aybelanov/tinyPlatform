using Google.Protobuf;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Clients.Devices;
using Hub.Services.Clients.Reports;
using Hub.Services.Clients.Sensors;
using Hub.Services.Devices;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Records;


/// <summary>
/// Represents a dashboard client sensor data record service 
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class SensorRecordService(IRepository<Device> deviceRepository,
   IRepository<Sensor> sensorRepository,
   ISensorService sensorService,
   IDeviceService deviceService,
   IRepository<VideoSegment> videoSegmentRepository,
   IRepository<VideoSegmentBinary> videoSegmentBinaryRepository,
   IRepository<User> userRepository,
   IRepository<UserDevice> deviceUserRepository,
   IRepository<SensorRecord> sensorDataRepository)
   : HubSensorRecordService(deviceRepository, sensorRepository, userRepository, sensorDataRepository), ISensorRecordService
{
   #region Fields

   private readonly IRepository<Device> _deviceRepository = deviceRepository;
   private readonly IRepository<Sensor> _sensorRepository = sensorRepository;
   private readonly IRepository<SensorRecord> _sensorRecordRepository = sensorDataRepository;
   private readonly IRepository<VideoSegment> _videoSegmentRepository = videoSegmentRepository;
   private readonly IRepository<VideoSegmentBinary> _videoSegmentBinaryRepository = videoSegmentBinaryRepository;
   private readonly IRepository<UserDevice> _userDeviceRepository = deviceUserRepository;
   private readonly ISensorService _sensorService = sensorService;
   private readonly IDeviceService _deviceService = deviceService;

   #endregion

   #region Ctor

   #endregion

   #region Methods

   #region Scalar sensors

   /// <summary>
   /// User sensor record scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Sensor record user scope query</returns>
   public IQueryable<SensorRecord> UserScope(long? userId)
   {
      var query = _sensorRecordRepository.Table.AsNoTracking();

      if (userId.HasValue)
      {
         query =
            from r in query
            join s in _sensorService.UserScope(userId) on r.SensorId equals s.Id
            join d in _deviceService.UserScope(userId) on s.DeviceId equals d.Id
            select r;
      }

      return query;
   }

   /// <summary>
   /// Delete entities by the dynamoc filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Async operation</returns>
   public Task DeleteRecordsByFilterAsync(DynamicFilter filter)
   {
      throw new NotImplementedException();
   }

   /// <summary>
   /// Gets chart set of the sensor records
   /// </summary>
   /// <param name="request">Chart request</param>
   /// <returns>Chart series</returns>
   public async Task<IList<ChartSet>> GetChartSeriesAsync(ChartRequest request)
   {
      ArgumentNullException.ThrowIfNull(request.SensorIds);

      if (!request.SensorIds.Any())
         throw new ArgumentException(nameof(request.SensorIds));

      if (request.ChartWidth < 1)
         throw new ArgumentException(nameof(request.ChartWidth));

      var from = request.From.ToUniversalTime().Ticks;
      var to = request.To.ToUniversalTime().Ticks;
      var sensorIds = request.SensorIds.ToList();
      var range = to - from;

      if (range < 0)
         throw new ArgumentException(nameof(ChartRequest.From) + " " + nameof(ChartRequest.To));

      var rangePerPixel = range / request.ChartWidth;
      rangePerPixel = rangePerPixel < 1L ? 1L : rangePerPixel;

      var chartsQuery =
      from r in _sensorRecordRepository.Table.AsNoTracking()
      where r.EventTimestamp >= @from && r.EventTimestamp <= to && sensorIds.Contains(r.SensorId)
      orderby r.EventTimestamp
      group r by r.SensorId into sensorGroup
      select new ChartSet()
      {
         EntityId = sensorGroup.Key,

         Data =
            from r in sensorGroup
            group r by r.EventTimestamp / rangePerPixel into rangeGroup
            select new ChartPoint()
            {
               MaxY = rangeGroup.Max(x => x.Value),
               MinY = rangeGroup.Min(x => x.Value),
               Y = rangeGroup.Average(x => x.Value),
               X = rangeGroup.Average(x => x.EventTimestamp)
            }
      };

      return await chartsQuery.ToListAsync();
   }



   /// <summary>
   /// Gets data statistics for the interval
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Data statistics</returns>
   public async Task<IList<DataStaticticsItem>> GetDataStatisticsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var now = DateTime.UtcNow;
      var (interval, subInterval) = filter.TimeInterval.ToTimeIntervals(now);
      var from = now.Ticks - interval;
      var rem = @from % subInterval;

      var intervalCount = interval / subInterval;
      var remGroup = (@from - rem) / subInterval;

      var deviceQuery = _deviceRepository.Table.AsNoTracking();

      if (filter.UserId.HasValue)
         deviceQuery = deviceQuery.Where(x => x.OwnerId == filter.UserId);

      if (filter.DeviceId.HasValue)
         deviceQuery = deviceQuery.Where(x => x.Id == filter.DeviceId);

      var sensorQuery =
         from s in _sensorRepository.Table.AsNoTracking()
         join d in deviceQuery on s.DeviceId equals d.Id
         select s;

      var recordQuery =
         from r in _sensorRecordRepository.Table.AsNoTracking()
         join s in sensorQuery on r.SensorId equals s.Id
         where r.EventTimestamp >= @from
         select r;

      var query =
         from r in recordQuery
         group r by (r.EventTimestamp - rem) / subInterval into gr
         orderby gr.Key
         select new
         {
            gr.Key,
            Moment = gr.Max(x => x.EventTimestamp),
            RecordCount = gr.Count()
         };

      var dbResult = await query.ToListAsync();

      var points = new List<DataStaticticsItem>() { new() { Moment = @from, RecordCount = 0 } };
      for (var p = @from + subInterval; p <= now.Ticks; p += subInterval)
      {
         points.Add(new()
         {
            Moment = p,
            RecordCount = dbResult.FirstOrDefault(x => x.Moment > p - subInterval && x.Moment <= p)?.RecordCount ?? 0
         });
      }

      // var test1 = dbResult.Select(x => new { Date = new DateTime(x.Moment, DateTimeKind.Utc), Count = x.RecordCount }).ToList();
      // var test2 = points.Select(x => new { Date = new DateTime(x.Moment, DateTimeKind.Utc), Count = x.RecordCount }).ToList();

      return points;
   }

   /// <summary>
   /// Gets GNSS track (points) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Geo point collection</returns>
   public async Task<ByteString> GetTrackAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter?.SensorId);

      var sensorQuery = _sensorService.UserScope(filter.UserId).Where(x => x.Id == filter.SensorId);

      // query
      var recordQuery =
         from r in _sensorRecordRepository.Table.AsNoTracking()
         join s in sensorQuery on r.SensorId equals s.Id
         select r;

      if (filter.From.HasValue)
         recordQuery = recordQuery.Where(x => x.EventTimestamp >= filter.From.Value.Ticks);

      if (filter.To.HasValue)
         recordQuery = recordQuery.Where(x => x.EventTimestamp <= filter.To.Value.Ticks);

      var query =
         from r in recordQuery
         select r.Bytes;

      var set = await query.ToListAsync();

      if (set.Count < 1)
      {
         set =
            await (from r in _sensorRecordRepository.Table.AsNoTracking()
                   join s in sensorQuery on r.SensorId equals s.Id
                   orderby r.EventTimestamp descending
                   select r.Bytes).Take(1).ToListAsync();
      }

      // zip data to transfer for clients app
      var baseTicks = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
      var res = set.SelectMany(r => new[]
      {
            BitConverter.GetBytes((int)((BitConverter.ToInt64(r, 0) - baseTicks) / TimeSpan.TicksPerSecond)),
            BitConverter.GetBytes((int)(BitConverter.ToDouble(r, 8) * 1_000_000)),
            BitConverter.GetBytes((int)(BitConverter.ToDouble(r, 16) * 1_000_000)),
            BitConverter.GetBytes((short)BitConverter.ToDouble(r, 24)),

        }).SelectMany(x => x);

      return UnsafeByteOperations.UnsafeWrap(res.ToArray());
   }

   /// <summary>
   /// Get sensor records by a dynamoc filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Collection of sensor records (async operation)</returns>
   public async Task<IFilterableList<SensorRecord>> GetRecordsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter?.SensorId);

      var sensorQuery = _sensorService.UserScope(filter.UserId).Where(x => x.Id == filter.SensorId);

      var query =
         from r in _sensorRecordRepository.Table.AsNoTracking()
         join s in sensorQuery on r.SensorId equals s.Id
         select r;

      if (filter.SensorId.HasValue)
      {
         query = query.Where(x => x.SensorId == filter.SensorId);
      }

      query = query.ApplyClientQuery(filter);
      var res = await query.FilterAsync(filter);

      return res;
   }


   #endregion

   #region Ip cams

   /// <summary>
   /// Deletes video segment info by identifiers
   /// </summary>
   /// <param name="ids">Segment identifiers</param>
   /// <returns></returns>
   public async Task DeleteVideoSegmentsAsync(IEnumerable<long> ids)
   {
      ArgumentNullException.ThrowIfNull(ids);
      await _videoSegmentRepository.DeleteAsync(x => ids.Contains(x.Id));
   }

   /// <summary>
   /// Gets wideo segment data by the segment name
   /// </summary>
   /// <param name="segmentName">Videosegment file name</param>
   /// <returns>Videosegment</returns>
   public async Task<VideoSegment> GetVideoSegmentByFileName(string segmentName)
   {
      if (string.IsNullOrWhiteSpace(segmentName))
         throw new ArgumentException(segmentName);

      var res = await _videoSegmentRepository.Table.FirstOrDefaultAsync(x => x.InboundName == segmentName);
      return res;
   }

   /// <summary>
   /// Inserts a inbound video segment info to data base
   /// </summary>
   /// <param name="segment">Video segment</param>
   /// <param name="binary">Binarycontent</param>
   /// <returns>Videosegment identifier</returns>
   public async Task<long> InsertVideoSegmentAsync(VideoSegment segment, VideoSegmentBinary binary)
   {
      ArgumentNullException.ThrowIfNull(segment);
      await _videoSegmentRepository.InsertAsync(segment);
      binary.VideoSegmentId = segment.Id;
      await _videoSegmentBinaryRepository.InsertAsync(binary);

      return segment.Id;
   }

   /// <summary>
   /// Update videsegment info into databse
   /// </summary>
   /// <param name="segment">Videosegment</param>
   /// <returns></returns>
   public async Task UpdateVideoSegmentAsync(VideoSegment segment)
   {
      ArgumentNullException.ThrowIfNull(segment);
      await _videoSegmentRepository.UpdateAsync(segment);
   }

   /// <summary>
   /// Gets video segment data by the dynamic filter (without binary data)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Video segment collection</returns>
   public async Task<IList<VideoSegment>> GetSegmentsByFilterAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter.SensorId);

      var sensorQuery = _sensorService.UserScope(filter.UserId).Where(x => x.Id == filter.SensorId);

      var segmentQuery =
         from v in _videoSegmentRepository.Table.AsNoTracking()
         join s in sensorQuery on v.IpcamId equals s.Id
         select v;

      if (filter.From.HasValue)
         segmentQuery = segmentQuery.Where(x => x.OnCreatedUtc >= filter.From.Value);

      if (filter.To.HasValue)
         segmentQuery = segmentQuery.Where(x => x.OnCreatedUtc <= filter.To.Value);

      return await segmentQuery.OrderBy(x => x.OnCreatedUtc).ToListAsync();
   }

   /// <summary>
   /// Gets videosegment by guid
   /// </summary>
   /// <param name="guid">video segment guid</param>
   /// <returns>Video segment binary</returns>
   public async Task<VideoSegmentBinary> GetSegmentByGuidAsync(Guid guid)
   {
      if (guid == default)
         throw new ArgumentException(nameof(guid));

      var segment = await (

         from s in _videoSegmentRepository.Table.AsNoTracking()
         where s.Guid == guid
         join b in _videoSegmentBinaryRepository.Table.AsNoTracking() on s.Id equals b.VideoSegmentId
         select b

         ).FirstOrDefaultAsync();

      return segment;
   }

   #endregion

   #endregion
}